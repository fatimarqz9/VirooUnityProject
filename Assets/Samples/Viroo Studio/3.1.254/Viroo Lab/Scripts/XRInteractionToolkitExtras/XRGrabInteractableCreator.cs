#nullable enable
using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using UnityEngine;
using Viroo.Api;
using Viroo.Interactions.Grab;
using Virtualware.Networking.Client;

namespace VirooLab
{
    public class XRGrabInteractableCreator : MonoBehaviour, IRecipient<NetworkObjectInstantiatedData>
    {
        [SerializeField]
        private string objectId = default!;

        [SerializeField]
        private VirooXRGrabInteractable grabInteractable = default!;

        private IMessenger? messenger;

        protected void Inject(IMessenger messenger)
        {
            this.messenger = messenger;

            messenger.RegisterAll(this);
        }

        protected void Awake()
        {
            this.QueueForInject();
        }

        protected void Start()
        {
            grabInteractable.OnGrabStateChanged += OnGrabStateChanged;
        }

        protected void OnDestroy()
        {
            messenger?.UnregisterAll(this);
        }

        public void Receive(NetworkObjectInstantiatedData message)
        {
            if (!message.ResourceIdentifier.Equals(objectId, StringComparison.Ordinal))
            {
                return;
            }

            VirooXRGrabInteractable interactable = message.GameObject.GetComponent<VirooXRGrabInteractable>();
            interactable.OnGrabStateChanged += OnGrabStateChanged;
        }

        private async void OnGrabStateChanged(object sender, NetworkGrabEventArgs e)
        {
            VirooXRGrabInteractable interactable = (VirooXRGrabInteractable)sender;
            interactable.OnGrabStateChanged -= OnGrabStateChanged;

            if (interactable.NetworkObject!.Authority)
            {
                await CreateInteractable();
            }
        }

        private async Task CreateInteractable()
        {
            await VirooApi.Instance.Networking().CreateDynamicObject(
                    objectId,
                    transform.position,
                    transform.rotation,
                    requestAuthority: true,
                    isPersistent: true);
        }
    }
}
