#nullable enable
using System;
using LitMotion;
using UnityEngine;
using Viroo.Api;
using Viroo.Networking;

namespace VirooLab
{
    public class StartPointIndicator : MonoBehaviour
    {
        public enum Mode
        {
            RoomCenter = 0,
            RoomDesktop = 1,
            Player = 2,
        }

        [SerializeField]
        private Mode mode = Mode.RoomCenter;

        [SerializeField]
        private Transform rotateTarget = default!;

        [SerializeField]
        private Color color = Color.red;

        [SerializeField]
        private float animationTime = 1;

        [SerializeField]
        private Transform labelLookAt = default!;

        private Transform lookAtTarget = default!;

        private MotionHandle motionHandle;

        protected void Awake()
        {
            VirooApi.Instance.OnInitialized += OnVirooInitialized;
        }

        protected void Update()
        {
            if (!lookAtTarget)
            {
                return;
            }

            Vector3 projectedLookAt = Vector3.ProjectOnPlane(lookAtTarget.position - labelLookAt.position, labelLookAt.up);

            if (projectedLookAt.magnitude < 0.01f)
            {
                return;
            }

            labelLookAt.forward = projectedLookAt.normalized;
        }

        protected void OnDestroy()
        {
            if (motionHandle.IsActive())
            {
                motionHandle.Cancel();
            }
        }

        private void OnVirooInitialized(object sender, EventArgs e)
        {
            VirooApi.Instance.OnInitialized -= OnVirooInitialized;

            IPlayer localPlayer = VirooApi.Instance.Players().GetLocalPlayer();

            bool active = (mode == Mode.Player && !localPlayer.PlayerData!.IsRoomPlayer)
                || (localPlayer.PlayerData!.IsRoomPlayer && (mode == Mode.RoomCenter || mode == Mode.RoomDesktop));

            gameObject.SetActive(active);

            if (active)
            {
                motionHandle = LMotion
                    .Create(rotateTarget.rotation.eulerAngles.y, rotateTarget.rotation.eulerAngles.y + 360, animationTime)
                    .WithEase(Ease.Linear)
                    .WithLoops(-1, LoopType.Restart)
                    .Bind(value => rotateTarget.rotation = Quaternion.Euler(
                        rotateTarget.rotation.eulerAngles.x,
                        value,
                        rotateTarget.rotation.eulerAngles.z));
            }

            Renderer indicatorRenderer = rotateTarget.GetComponent<Renderer>();
            indicatorRenderer.material = new(indicatorRenderer.material)
            {
                color = color,
            };

            lookAtTarget = VirooApi.Instance.Context().VirooCamera.OriginalCamera.transform;
        }
    }
}
