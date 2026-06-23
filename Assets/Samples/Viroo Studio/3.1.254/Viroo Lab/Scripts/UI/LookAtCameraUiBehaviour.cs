#nullable enable
using System;
using UnityEngine;
using Viroo.Api;

namespace VirooLab
{
    public class LookAtCameraUiBehaviour : MonoBehaviour
    {
        [SerializeField]
        private float distance = 1f;

        [SerializeField]
        private float smoothTime = 0.1f;

        private Transform? target;
        private Vector3 currentVelocity;

        protected void Awake()
        {
            VirooApi.Instance.OnInitialized += OnVirooInitialized;
        }

        protected void Update()
        {
            if (!target)
            {
                return;
            }

            Vector3 position = GetTargetPosition();
            transform.position = Vector3.SmoothDamp(transform.position, position, ref currentVelocity, smoothTime);

            transform.LookAt(target);
        }

        private void OnVirooInitialized(object sender, EventArgs e)
        {
            VirooApi.Instance.OnInitialized -= OnVirooInitialized;

            target = VirooApi.Instance.Context().VirooCamera.OriginalCamera.transform;

            transform.position = GetTargetPosition();
            transform.LookAt(target);
        }

        private Vector3 GetTargetPosition() => target!.position + (target.forward * distance);
    }
}
