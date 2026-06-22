#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using Viroo.Api;
using Viroo.Api.Locomotion;
using Viroo.Interactions.XRInteractionToolkit.Locomotion;
using Viroo.SceneLoader.Api;
using Virtualware.Networking.Client.SceneManagement;

namespace VirooLab
{
    public class TogglePlayerMovement : MonoBehaviour
    {
        [SerializeField]
        private GameObject activatePlayerMovementText = default!;

        [SerializeField]
        private GameObject deactivatePlayerMovementText = default!;

        private readonly List<ILocomotionProviderWrapper> locomotionProviderWrappers = new();

        private bool locomotionEnabled = true;

        protected void Awake()
        {
            VirooApi.Instance.OnInitialized += OnVirooInitialized;

            UpdateText();
        }

        protected void OnDestroy()
        {
            if (VirooApi.Instance.IsInitialized)
            {
                VirooApi.Instance.Scenes().OnLocalClientSceneUnloadStarted -= OnSceneUnloadStarted;
            }
        }

        private void OnVirooInitialized(object sender, EventArgs e)
        {
            VirooApi.Instance.OnInitialized -= OnVirooInitialized;

            VirooApi.Instance.Scenes().OnLocalClientSceneUnloadStarted += OnSceneUnloadStarted;

            bool isVR = VirooApi.Instance.Context().IsVr;

            IVirooLocomotionApi locomotionProvider = VirooApi.Instance.Locomotion();

            if (isVR)
            {
                locomotionProviderWrappers.Add(locomotionProvider.GetLocomotionProvider(
                   LocomotionProviderTypes.Controller,
                   LocomotionTypes.Move));

                locomotionProviderWrappers.Add(locomotionProvider.GetLocomotionProvider(
                   LocomotionProviderTypes.Hand,
                   LocomotionTypes.Move));
            }
            else
            {
                locomotionProviderWrappers.Add(locomotionProvider.GetLocomotionProvider(
                    LocomotionProviderTypes.Desktop,
                    LocomotionTypes.Move));
            }
        }

        private void OnSceneUnloadStarted(object sender, NetworkSceneEventArgs e)
        {
            if (!locomotionEnabled)
            {
                ToggleMovement();
            }
        }

        public void ToggleMovement()
        {
            if (locomotionProviderWrappers.Count == 0)
            {
                return;
            }

            locomotionEnabled = !locomotionEnabled;

            foreach (ILocomotionProviderWrapper locomotionProviderWrapper in locomotionProviderWrappers)
            {
                locomotionProviderWrapper.LocomotionProvider.enabled = locomotionEnabled;
            }

            UpdateText();
        }

        private void UpdateText()
        {
            activatePlayerMovementText!.SetActive(value: !locomotionEnabled);
            deactivatePlayerMovementText!.SetActive(value: locomotionEnabled);
        }
    }
}
