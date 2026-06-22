#nullable enable
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Viroo.Api;
using Viroo.Api.Interactions;
using Viroo.Networking;
using Viroo.Player.Input;

namespace VirooLab
{
    public class CalloutGazeController : MonoBehaviour
    {
        private const string LeftHand = nameof(LeftHand);
        private const string RightHand = nameof(RightHand);

        private enum Hand
        {
            LeftHand = 0,
            RightHand = 1,
        }

        [SerializeField]
        private Hand currentHand = default!;

        [SerializeField]
        [Tooltip("Threshold for the dot product when determining if the Gaze Transform is facing this object. +" +
            "The lower the threshold, the wider the field of view.")]
        [Range(0.0f, 1.0f)]
        private float facingThreshold = 0.85f;

        [SerializeField]
        [Tooltip("Events fired when the Gaze Transform begins facing this game object")]
        private UnityEvent facingEntered = default!;

        [SerializeField]
        [Tooltip("Events fired when the Gaze Transform stops facing this game object")]
        private UnityEvent facingExited = default!;

        [SerializeField]
        [Tooltip("Distance threshold for movement in a single frame " +
            "that determines a large movement that will trigger Facing Exited events.")]
        private float largeMovementDistanceThreshold = 0.05f;

        [SerializeField]
        [Tooltip("Cool down time after a large movement for Facing Entered events to fire again.")]
        private float largeMovementCoolDownTime = 0.25f;

        [SerializeField]
        private Image[] disabledIconsIfRoomPlayer = default!;

        private Transform? gazeTransform;

        private bool isFacing;
        private float largeMovementCoolDown;
        private Vector3 lastPosition;

        protected IVirooInteractionsApi? virooInteractionsApi;

        protected void Awake()
        {
            VirooApi.Instance.OnInitialized += OnVirooInitialized;
        }

        protected void Update()
        {
            if (!gazeTransform)
            {
                return;
            }

            CheckLargeMovement();

            if (largeMovementCoolDown < largeMovementCoolDownTime)
            {
                return;
            }

            float dotProduct = Vector3.Dot(gazeTransform.forward, (transform.position - gazeTransform.position).normalized);

            if (dotProduct > facingThreshold && !isFacing)
            {
                FacingEntered();
            }
            else if (dotProduct < facingThreshold && isFacing)
            {
                FacingExited();
            }
        }

        private void OnVirooInitialized(object sender, EventArgs e)
        {
            VirooApi.Instance.OnInitialized -= OnVirooInitialized;

            if (!VirooApi.Instance.Context().IsVr)
            {
                gameObject.SetActive(value: false);
                return;
            }

            gazeTransform = VirooApi.Instance.Context().VirooCamera.OriginalCamera.transform;

            IPlayer localPlayer = VirooApi.Instance.Players().GetLocalPlayer();

            if (localPlayer.PlayerData!.IsRoomPlayer)
            {
                foreach (Image icn in disabledIconsIfRoomPlayer)
                {
                    icn.enabled = false;
                }
            }

            if (VirooApi.Instance.Interactions().CurrentInteractionMode.Equals(InteractionMode.XRController))
            {
                SetHandObject();
            }
            else
            {
                VirooApi.Instance.Interactions().OnInteractionModeChanged += OnInteractionModeChanged;
            }
        }

        private void OnInteractionModeChanged(object sender, InteractionModeChangedEventArgs e)
        {
            if (VirooApi.Instance.Interactions().CurrentInteractionMode.Equals(InteractionMode.XRController))
            {
                VirooApi.Instance.Interactions().OnInteractionModeChanged -= OnInteractionModeChanged;

                SetHandObject();
            }
        }

        private void SetHandObject()
        {
            Transform parent = currentHand == Hand.LeftHand
                ? VirooApi.Instance.Context().LeftHand.transform
                : VirooApi.Instance.Context().RightHand.transform;

            gameObject.transform.SetParent(parent, worldPositionStays: false);
        }

        private void CheckLargeMovement()
        {
            Vector3 currentPosition = transform.position;
            float positionDelta = Mathf.Abs(Vector3.Distance(lastPosition, currentPosition));

            if (positionDelta > largeMovementDistanceThreshold)
            {
                largeMovementCoolDown = 0.0f;
                FacingExited();
            }

            largeMovementCoolDown += Time.deltaTime;
            lastPosition = currentPosition;
        }

        private void FacingEntered()
        {
            isFacing = true;
            facingEntered?.Invoke();
        }

        private void FacingExited()
        {
            isFacing = false;
            facingExited?.Invoke();
        }
    }
}
