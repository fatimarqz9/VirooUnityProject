#nullable enable
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace VirooLab
{
    public class HandsDetectionZone : MonoBehaviour
    {
        [SerializeField]
        private XRSimpleInteractable simpleInteractable = default!;

        [SerializeField]
        private GameObject noHandsHovering = default!;

        [SerializeField]
        private GameObject leftHandHovering = default!;

        [SerializeField]
        private GameObject rightHandHovering = default!;

        private readonly List<XRBaseInteractor> hoveringInteractors = new();

        protected void OnEnable()
        {
            simpleInteractable.hoverEntered.AddListener(OnHoverEntered);
            simpleInteractable.hoverExited.AddListener(OnHoverExited);
        }

        protected void OnDisable()
        {
            simpleInteractable.hoverEntered.RemoveListener(OnHoverEntered);
            simpleInteractable.hoverExited.RemoveListener(OnHoverExited);
        }

        private void Process()
        {
            noHandsHovering.SetActive(hoveringInteractors.Count == 0);
            leftHandHovering.SetActive(hoveringInteractors.Exists(i => i.handedness.Equals(InteractorHandedness.Left)));
            rightHandHovering.SetActive(hoveringInteractors.Exists(i => i.handedness.Equals(InteractorHandedness.Right)));
        }

        private void OnHoverEntered(HoverEnterEventArgs argument)
        {
            if (argument.interactorObject is NearFarInteractor interactor && !hoveringInteractors.Contains(interactor))
            {
                hoveringInteractors.Add(interactor);
            }

            Process();
        }

        private void OnHoverExited(HoverExitEventArgs argument)
        {
            if (argument.interactorObject is NearFarInteractor interactor)
            {
                hoveringInteractors.Remove(interactor);
            }

            Process();
        }
    }
}
