#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UnityEngine;
using Viroo.Api;
using Virtualware.Networking.Client;
using Virtualware.Networking.Client.Variables;

namespace VirooLab
{
    public class NetworkNavMeshAgentDestinations : MonoBehaviour
    {
        [SerializeField]
        private NetworkNavMeshAgent agent = default!;

        [SerializeField]
        private List<Transform> destinations = default!;

        private NetworkVariable<int>? index;

        private bool virooInitialized;

        protected void Inject(ILogger<NetworkNavMeshAgentDestinations> logger)
        {
            InitializeAgent().Forget(logger);
        }

        protected void Awake()
        {
            this.QueueForInject();

            VirooApi.Instance.OnInitialized += OnVirooInitialized;
        }

        protected void OnDestroy()
        {
            if (index != null)
            {
                index.OnInitialized -= OnIndexChanged;
                index.OnValueChanged -= OnIndexChanged;

                index.Remove();
            }

            agent.NetworkObject!.OnOwnershipDataChanged -= OnOwnershipDataChanged;
        }

        private async Task InitializeAgent()
        {
            await UniTask.WaitUntil(() => !agent.ReceivingInitState && virooInitialized, cancellationToken: destroyCancellationToken);

            agent.OnDestinationReached += OnDestinationReached;

            NetworkObject networkObject = agent.GetComponent<NetworkObject>();
            networkObject.OnAuthorityInitialized += OnNetworkObjectAuthorityInitialized;

            agent.NetworkObject!.OnOwnershipDataChanged += OnOwnershipDataChanged;
        }

        private void OnVirooInitialized(object sender, EventArgs e)
        {
            VirooApi.Instance.OnInitialized -= OnVirooInitialized;

            index = VirooApi.Instance.Networking().CreateNetworkVariable(agent.NetworkObjectId + "_AgentDestinationIndex", defaultValue: 0);

            index.OnInitialized += OnIndexChanged;
            index.OnValueChanged += OnIndexChanged;

            virooInitialized = true;
        }

        private void OnNetworkObjectAuthorityInitialized(object sender, EventArgs args)
        {
            if (sender is not NetworkObject networkObject)
            {
                return;
            }

            networkObject.OnAuthorityInitialized -= OnNetworkObjectAuthorityInitialized;

            GoToDestination(index!.Value);
        }

        private void OnOwnershipDataChanged(object sender, EventArgs args) => GoToDestination(index!.Value);

        private void OnIndexChanged(object sender, int value) => GoToDestination(index!);

        private void SetNextDestination()
        {
            if (!agent.NetworkObject!.Authority)
            {
                return;
            }

            index!.Value = (index.Value + 1) % destinations.Count;
        }

        private void GoToDestination(int index)
        {
            if (!agent.NetworkObject!.Authority)
            {
                return;
            }

            agent.SetDestination(destinations[index].position);
        }

        private void OnDestinationReached(object sender, EventArgs e) => SetNextDestination();
    }
}
