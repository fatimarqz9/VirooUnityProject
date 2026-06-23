#nullable enable
using System;
using UnityEngine;
using Viroo.Api;
using Viroo.Input;
using Viroo.SceneLoader.Api;
using Virtualware.Networking.Client.SceneManagement;

namespace VirooLab
{
    public class ToggleFlyMode : MonoBehaviour
    {
        [SerializeField]
        private GameObject activateFlyModeText = default!;

        [SerializeField]
        private GameObject deactivateFlyModeText = default!;

        private bool flyModeEnabled = true;

        private IFlyPlayerInput? flyPlayerInput;

        protected void Awake()
        {
            VirooApi.Instance.OnInitialized += OnVirooInitialized;

            SetText();
        }

        protected void OnDestroy()
        {
            if (flyPlayerInput != null)
            {
                flyPlayerInput.OnFlyModeStateChanged -= UpdateText;
            }

            if (VirooApi.Instance.IsInitialized)
            {
                VirooApi.Instance.Scenes().OnLocalClientSceneUnloadStarted -= OnSceneUnloadStarted;
            }
        }

        private void OnVirooInitialized(object sender, EventArgs e)
        {
            VirooApi.Instance.OnInitialized -= OnVirooInitialized;

            VirooApi.Instance.Scenes().OnLocalClientSceneUnloadStarted += OnSceneUnloadStarted;

            flyPlayerInput = VirooApi.Instance.Locomotion().Fly;

            flyPlayerInput.OnFlyModeStateChanged += UpdateText;
        }

        private void OnSceneUnloadStarted(object sender, NetworkSceneEventArgs e)
        {
            if (!flyPlayerInput!.IsFlyingEnabled)
            {
                ToggleFly();
            }
        }

        public void ToggleFly()
        {
            if (flyPlayerInput == null)
            {
                return;
            }

            flyModeEnabled = !flyModeEnabled;

            flyPlayerInput.SetEnabled(this, flyModeEnabled);
        }

        private void UpdateText(object sender, FlyPlayerInputEventArgs e)
        {
            SetText();
        }

        private void SetText()
        {
            activateFlyModeText.SetActive(value: !flyModeEnabled);
            deactivateFlyModeText.SetActive(value: flyModeEnabled);
        }
    }
}
