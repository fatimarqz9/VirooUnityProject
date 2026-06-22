#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using Viroo.Api;
using Virtualware.Networking.Client;
using Virtualware.Networking.Client.Variables;
using Random = UnityEngine.Random;

namespace VirooLab
{
    public class TargetsController : MonoBehaviour
    {
        [SerializeField]
        private List<Target> targets = default!;

        [SerializeField]
        private Score score = default!;

        private NetworkVariable<bool>? isPlaying;
        private NetworkVariable<int>? currentTargetIndex;
        private NetworkObject? networkObject;

        private bool HasAuthority => networkObject!.Authority;

        protected void Awake()
        {
            VirooApi.Instance.OnInitialized += OnVirooInitialized;
        }

        protected void OnDestroy()
        {
            if (currentTargetIndex != null)
            {
                currentTargetIndex.OnInitialized -= OnCurrentIndexChanged;
                currentTargetIndex.OnValueChanged -= OnCurrentIndexChanged;

                currentTargetIndex.Remove();
            }

            isPlaying?.Remove();
        }

        private void OnVirooInitialized(object sender, EventArgs e)
        {
            VirooApi.Instance.OnInitialized -= OnVirooInitialized;

            networkObject = GetComponent<NetworkObject>();

            isPlaying = VirooApi.Instance.Networking().CreateNetworkVariable("TargetsControllerPlaying", defaultValue: false);

            currentTargetIndex = VirooApi.Instance.Networking().CreateNetworkVariable("TargetsControllerIndex", defaultValue: -1);

            currentTargetIndex.OnInitialized += OnCurrentIndexChanged;
            currentTargetIndex.OnValueChanged += OnCurrentIndexChanged;
        }

        private async void OnHit(object sender, EventArgs e)
        {
            score.Increment();

            Target target = (sender as Target)!;
            target.OnHit -= OnHit;
            await target.AnimateHide();

            SetTarget();
        }

        public void Play()
        {
            if (isPlaying!.Value)
            {
                return;
            }

            isPlaying.Value = true;

            SetTarget();
        }

        private void SetTarget()
        {
            if (!HasAuthority)
            {
                return;
            }

            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, targets.Count);
            }
            while (randomIndex == currentTargetIndex!.Value);

            currentTargetIndex.Value = randomIndex;
        }

        private async void OnCurrentIndexChanged(object sender, int value)
        {
            if (value < 0)
            {
                return;
            }

            Target target = targets[currentTargetIndex!.Value];

            await target.AnimateShow();
            target.OnHit += OnHit;
        }
    }
}
