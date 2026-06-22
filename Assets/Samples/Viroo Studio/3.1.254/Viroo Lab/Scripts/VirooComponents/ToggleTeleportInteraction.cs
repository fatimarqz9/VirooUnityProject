#nullable enable
using System;
using UnityEngine;
using Viroo.Api;
using Viroo.SceneLoader.Api;
using Virtualware.Networking.Client.SceneManagement;

namespace VirooLab
{
    public class ToggleTeleportInteraction : MonoBehaviour
    {
        [SerializeField]
        private GameObject activateTeleportText = default!;

        [SerializeField]
        private GameObject deactivateTeleportText = default!;

        private bool teleportEnabled = true;

        private bool virooReady;

        protected void Awake()
        {
            VirooApi.Instance.OnInitialized += OnVirooInitialized;

            SetText();
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

            virooReady = true;
        }

        private void OnSceneUnloadStarted(object sender, NetworkSceneEventArgs e)
        {
            if (!virooReady)
            {
                return;
            }

            VirooApi.Instance.Teleport().RemoveSource(this);
        }

        public void ToggleTeleport()
        {
            if (!virooReady)
            {
                return;
            }

            teleportEnabled = !teleportEnabled;

            VirooApi.Instance.Teleport().SetEnabled(this, teleportEnabled);

            SetText();
        }

        private void SetText()
        {
            activateTeleportText.SetActive(value: !teleportEnabled);
            deactivateTeleportText.SetActive(value: teleportEnabled);
        }
    }
}
