#nullable enable
using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using Viroo.Api;
using Viroo.Interactions.Hand;

namespace VirooLab
{
    public class ForceUngrab : MonoBehaviour
    {
        [SerializeField]
        private XRSocketInteractor socketInteractor = default!;

        [SerializeField]
        private XRGrabInteractable grabInteractable = default!;

        private IHandInteractorProvider? handInteractorProvider;

        protected void Awake()
        {
            VirooApi.Instance.OnInitialized += OnVirooInitialized;
        }

        protected void OnEnable()
        {
            socketInteractor.hoverEntered.AddListener(OnHoverEntered);
        }

        protected void OnDisable()
        {
            socketInteractor.hoverEntered.RemoveListener(OnHoverEntered);
        }

        private void OnVirooInitialized(object sender, EventArgs e)
        {
            VirooApi.Instance.OnInitialized -= OnVirooInitialized;

            handInteractorProvider = VirooApi.Instance.Interactions().Hands;
        }

        private async void OnHoverEntered(HoverEnterEventArgs args)
        {
            if (args.interactableObject == (IXRHoverInteractable)grabInteractable)
            {
                IHandInteractor? interactor = handInteractorProvider!.GetInteractor(grabInteractable);

                if (interactor != null)
                {
                    await interactor.ForceDeselect();

                    socketInteractor.interactionManager.SelectEnter(socketInteractor as IXRSelectInteractor, grabInteractable);
                }
            }
        }
    }
}
