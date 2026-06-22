#nullable enable
using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Viroo.Api;
using Viroo.Networking;

namespace VirooLab
{
    public class ModeDependantButton : MonoBehaviour
    {
        public enum PlayMode
        {
            RoomPlayer = 0,
            SinglePlayer = 1,
        }

        public enum Mode
        {
            Desktop = 0,
            Vr = 1,
            Both = 2,
        }

        [Tooltip("The button will be disabled in this mode")]
        [SerializeField]
        private PlayMode disableMode = default!;

        [SerializeField]
        private Mode mode = default!;

        [SerializeField]
        private GameObject disabledButton = default!;

        [SerializeField]
        private GameObject enabledButton = default!;

        [SerializeField]
        private XRSimpleInteractable interactable = default!;

        protected void Awake()
        {
            VirooApi.Instance.OnInitialized += OnVirooInitialized;
        }

        private void OnVirooInitialized(object sender, EventArgs e)
        {
            VirooApi.Instance.OnInitialized -= OnVirooInitialized;

            IPlayer localPlayer = VirooApi.Instance.Players().GetLocalPlayer();

            PlayMode currentPlayMode = localPlayer!.PlayerData!.IsRoomPlayer ? PlayMode.RoomPlayer : PlayMode.SinglePlayer;
            Mode currentMode = VirooApi.Instance.Context().IsVr ? Mode.Vr : Mode.Desktop;

            bool shouldDisable = disableMode == currentPlayMode && (mode == currentMode || mode == Mode.Both);

            SetButtonState(shouldDisable);
        }

        private void SetButtonState(bool shouldDisable)
        {
            enabledButton.SetActive(!shouldDisable);
            disabledButton.SetActive(shouldDisable);
            interactable.enabled = !shouldDisable;
        }
    }
}
