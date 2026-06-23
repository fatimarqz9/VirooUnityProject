#nullable enable
using System;
using System.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

namespace VirooLab
{
    public class Target : MonoBehaviour
    {
        public event EventHandler OnHit = (sender, e) => { };

        [SerializeField]
        private float maxHeight = default;

        private Vector3 startPosition;
        private Quaternion startRotation;

        protected void Awake()
        {
            startPosition = transform.position;
            startRotation = transform.rotation;
        }

        public async Task AnimateShow()
        {
            MotionHandle motionHandle = LSequence.Create()
               .Append(LMotion.Create(transform.localPosition.y, maxHeight, 1)
               .BindToLocalPositionY(transform))
               .Append(LMotion.Create(transform.localRotation.eulerAngles, new Vector3(0, 90, 90), 1)
               .BindToLocalEulerAngles(transform))
               .Run();

            await motionHandle;
        }

        public async Task AnimateHide()
        {
            MotionHandle motionHandle = LSequence.Create()
                .Append(LMotion.Create(transform.rotation.eulerAngles, new Vector3(0, 360 * 4, 90), 2)
                .BindToEulerAngles(transform))
                .Append(LMotion.Create(transform.localRotation.eulerAngles, startRotation.eulerAngles, 0.5f)
                .BindToLocalEulerAngles(transform))
                .Append(LMotion.Create(transform.position.y, startPosition.y, 0.5f)
                .BindToPositionY(transform))
                .Run();
            await motionHandle;
        }

        protected void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Bullet _))
            {
                OnHit(this, EventArgs.Empty);
            }
        }
    }
}
