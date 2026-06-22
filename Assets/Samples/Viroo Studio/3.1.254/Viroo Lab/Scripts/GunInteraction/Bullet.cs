#nullable enable
using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Viroo.Api;
using Virtualware.Networking.Client;

namespace VirooLab
{
    public class Bullet : MonoBehaviour
    {
        private const float DestroyTime = 10;

        protected void Awake()
        {
            VirooApi.Instance.OnInitialized += OnVirooInitialized;

            Rigidbody rigid = GetComponent<Rigidbody>();
            rigid.AddForce(transform.forward * 0.25f, ForceMode.Impulse);
        }

        private async void OnVirooInitialized(object sender, EventArgs e)
        {
            VirooApi.Instance.OnInitialized -= OnVirooInitialized;

            IgnoreCollisionWithTheGun();

            try
            {
                CancellationToken cancellationToken = this.GetCancellationTokenOnDestroy();

                await UniTask.Delay((int)(DestroyTime * 1000), cancellationToken: cancellationToken);

                if (TryGetComponent(out NetworkObject networkObject))
                {
                    await VirooApi.Instance.Networking().DestroyObject(networkObject);
                }
            }
            catch (OperationCanceledException)
            {
                // Scene changed, ignore exception
            }
        }

        private void IgnoreCollisionWithTheGun()
        {
            Collider bulletCollider = GetComponent<Collider>();

            foreach (Collider col in FindObjectsByType<VirooTag>(FindObjectsSortMode.None).Where(
                virooTag => virooTag.Tag.Equals("Gun", StringComparison.OrdinalIgnoreCase))
                .SelectMany(virooTag => virooTag.GetComponentsInChildren<Collider>()))
            {
                Physics.IgnoreCollision(bulletCollider, col, ignore: true);
            }
        }
    }
}
