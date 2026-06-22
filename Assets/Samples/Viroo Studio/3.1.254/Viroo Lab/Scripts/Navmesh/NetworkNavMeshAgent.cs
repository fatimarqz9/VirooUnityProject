#nullable enable
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Networking.Messages.Senders;
using UnityEngine;
using UnityEngine.AI;
using Viroo.Api;
using Viroo.Api.Players;
using Viroo.Networking;
using Virtualware.Networking.Client;
using Virtualware.Networking.Client.Components;
using Virtualware.Networking.Client.Variables;

namespace VirooLab
{
    [RequireComponent(typeof(NetworkObject))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class NetworkNavMeshAgent : NetworkBehaviour
    {
        public event EventHandler<EventArgs> OnDestinationReached = (sender, e) => { };

        private IForwardMessageSender? forwardMessageSender;
        private IVirooPlayersApi? virooPlayersApi;

        private NavMeshAgent? agent;

        private NetworkVariable<Vector3>? destination;
        private NetworkVariable<bool>? isRunning;

        private string? networkObjectId;

        public string NetworkObjectId
        {
            get
            {
                if (string.IsNullOrEmpty(networkObjectId))
                {
                    networkObjectId = GetComponent<NetworkObject>().ObjectId;
                }

                return networkObjectId;
            }
        }

        public bool IsRunning => isRunning?.Value == true;

        private bool arrivalNotified = false;

        protected void Inject(IForwardMessageSender forwardMessageSender)
        {
            this.forwardMessageSender = forwardMessageSender;
        }

        protected new void Awake()
        {
            VirooApi.Instance.OnInitialized += OnVirooInitialized;

            agent = GetComponent<NavMeshAgent>();

            base.Awake();
        }

        protected void Update()
        {
#pragma warning disable RCS1146 // Use conditional access
            if (NetworkObject == null || !NetworkObject.IsInitialized)
#pragma warning restore RCS1146 // Use conditional access
            {
                return;
            }

            if (isRunning?.Value == true)
            {
                agent!.SetDestination(destination!);

                CheckArrivedToDestination();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            destination?.Remove();

            isRunning?.Remove();

            if (virooPlayersApi != null)
            {
                virooPlayersApi.OnPlayerRegistered -= OnPlayerRegistered;
            }
        }

        private void OnVirooInitialized(object sender, EventArgs e)
        {
            VirooApi.Instance.OnInitialized -= OnVirooInitialized;

            destination = VirooApi.Instance.Networking()
                .CreateNetworkVariable(NetworkObjectId + "_AgentDestination", defaultValue: Vector3.zero);

            isRunning = VirooApi.Instance.Networking().CreateNetworkVariable(NetworkObjectId + "_AgentIsRunning", defaultValue: false);
        }

        protected override async void OnBehaviourInitializationCompleted()
        {
            base.OnBehaviourInitializationCompleted();

            if (!VirooApi.Instance.IsInitialized)
            {
                await UniTask.WaitUntil(() => VirooApi.Instance.IsInitialized, cancellationToken: destroyCancellationToken);
            }

            virooPlayersApi = VirooApi.Instance.Players();

            virooPlayersApi.OnPlayerRegistered += OnPlayerRegistered;
        }

        private void OnPlayerRegistered(object sender, PlayerRegisteredEventArgs args)
        {
            if (!HasAuthority)
            {
                return;
            }

            SetTransformMessage message = new(IntId, transform.position, transform.rotation, transform.localScale);

            _ = forwardMessageSender!.Forward(message, new List<string> { args.Player.PlayerData!.ClientId });
        }

        protected override void RegisterMessageCallbacks()
        {
            base.RegisterMessageCallbacks();

            RegisterMessageCallback<SetTransformMessage>(OnInitialPositionReceived);
        }

        protected override void UnregisterMessageCallbacks()
        {
            base.UnregisterMessageCallbacks();

            UnregisterMessageCallback<SetTransformMessage>(OnInitialPositionReceived);
        }

        private void OnInitialPositionReceived(SetTransformMessage message)
        {
            agent!.Warp(message.Position);
        }

        public void SetDestination(Vector3 destination)
        {
            arrivalNotified = false;

            if (!HasAuthority)
            {
                return;
            }

            this.destination!.Value = destination;
            isRunning!.Value = true;
        }

        public void Stop()
        {
            if (!HasAuthority)
            {
                return;
            }

            isRunning!.Value = false;
        }

        private void CheckArrivedToDestination()
        {
            if (!arrivalNotified && agent!.hasPath && agent.remainingDistance <= agent.stoppingDistance + 0.2f)
            {
                arrivalNotified = true;

                agent.ResetPath();

                OnDestinationReached(this, EventArgs.Empty);
            }
        }
    }
}
