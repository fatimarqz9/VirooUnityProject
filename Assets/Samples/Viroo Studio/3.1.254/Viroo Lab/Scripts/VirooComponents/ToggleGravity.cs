#nullable enable
using System;
using UnityEngine;
using Viroo.Api;
using Viroo.Interactions.XRInteractionToolkit.Locomotion;
using Viroo.SceneLoader.Api;
using Virtualware.Networking.Client.SceneManagement;

namespace VirooLab
{
    public class ToggleGravity : MonoBehaviour
    {
        [SerializeField]
        private GameObject disableGravityText = default!;

        [SerializeField]
        private GameObject enableGravityText = default!;

        private bool isGravityEnabled = true;

        private IVirooGravityProvider? gravityProvider;

        protected void Awake()
        {
            VirooApi.Instance.OnInitialized += OnVirooInitialized;

            UpdateText();
        }

        protected void OnDestroy()
        {
            if (!VirooApi.Instance.IsInitialized)
            {
                return;
            }

            VirooApi.Instance.Scenes().OnLocalClientSceneUnloadStarted -= OnSceneUnloadStarted;
        }

        private void OnVirooInitialized(object sender, EventArgs e)
        {
            VirooApi.Instance.OnInitialized -= OnVirooInitialized;

            VirooApi.Instance.Scenes().OnLocalClientSceneUnloadStarted += OnSceneUnloadStarted;

            gravityProvider = VirooApi.Instance.Locomotion().Gravity;
        }

        private void OnSceneUnloadStarted(object sender, NetworkSceneEventArgs e)
        {
            if (!isGravityEnabled)
            {
                ToggleGravityService();
            }
        }

        public void ToggleGravityService()
        {
            if (gravityProvider == null)
            {
                return;
            }

            isGravityEnabled = !isGravityEnabled;
            gravityProvider.SetEnabled(sourceContext: this, isEnabled: isGravityEnabled);

            UpdateText();
        }

        private void UpdateText()
        {
            disableGravityText.SetActive(isGravityEnabled);
            enableGravityText.SetActive(!isGravityEnabled);
        }
    }
}
