#nullable enable

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace VirooLab
{
    public class SocketDebug : MonoBehaviour
    {
        protected void Awake()
        {
            XRSocketInteractor socketInteractor = GetComponent<XRSocketInteractor>();

            socketInteractor.hoverEntered.AddListener(OnHoverEntered);
            socketInteractor.hoverExited.AddListener(OnHoverExited);
            socketInteractor.selectEntered.AddListener(OnSelectEntered);
            socketInteractor.selectExited.AddListener(OnSelectExited);
        }

        private void OnSelectExited(SelectExitEventArgs arg0)
            => Debug.Log($"{arg0.interactableObject.transform.name} has select exited from {gameObject.name}");

        private void OnSelectEntered(SelectEnterEventArgs arg0)
             => Debug.Log($"{arg0.interactableObject.transform.name} has select entered in {gameObject.name}");

        private void OnHoverExited(HoverExitEventArgs arg0)
             => Debug.Log($"{arg0.interactableObject.transform.name} has hover exited from {gameObject.name}");

        private void OnHoverEntered(HoverEnterEventArgs arg0)
             => Debug.Log($"{arg0.interactableObject.transform.name} has hover entered in {gameObject.name}");
    }
}
