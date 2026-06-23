#nullable enable
using System;
using UnityEngine;
using Viroo.Api;
using Virtualware.Networking.Client;

namespace VirooLab
{
    public class CubeDestroyPosition : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem fire = default!;

        protected async void OnTriggerEnter(Collider other)
        {
            VirooTag virooTag = other.GetComponent<VirooTag>();

            if (virooTag && virooTag.Tag.Equals("RobotArmGrabbableCube", StringComparison.Ordinal))
            {
                NetworkObject networkObject = other.GetComponent<NetworkObject>();

                fire.Play();

                await VirooApi.Instance.Networking().ConditionalRunner
                    .RunIfLeader(async () => await VirooApi.Instance.Networking().DestroyObject(networkObject));
            }
        }
    }
}
