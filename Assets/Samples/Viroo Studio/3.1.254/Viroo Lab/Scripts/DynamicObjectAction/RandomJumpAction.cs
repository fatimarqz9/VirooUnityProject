#nullable enable
using System;
using System.Globalization;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using Viroo.Interactions;
using Random = UnityEngine.Random;

namespace VirooLab
{
    public class RandomJumpAction : BroadcastObjectAction
    {
        [SerializeField]
        private float minHeight = 1;

        [SerializeField]
        private float maxHeight = 3;

        private MotionHandle? motionHandle;

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (motionHandle?.IsActive() == true)
            {
                motionHandle.Value!.Cancel();
            }
        }

        public override void Execute(string data)
        {
            float targetHeight = Random.Range(minHeight, maxHeight);

            if (motionHandle?.IsActive() == true)
            {
                return;
            }

            base.Execute(targetHeight.ToString(CultureInfo.InvariantCulture));
        }

        protected override async void LocalExecuteImplementation(string data)
        {
            float height = float.Parse(data, CultureInfo.InvariantCulture);

            Vector3 initialPosition = transform.localPosition;
            Vector3 targetPosition = initialPosition + (Vector3.up * height);

            try
            {
                motionHandle = LMotion.Create(initialPosition, targetPosition, 1).BindToLocalPosition(transform);
                await motionHandle.Value.ToAwaitable(cancellationToken: destroyCancellationToken);

                motionHandle = LMotion.Create(targetPosition, initialPosition, 1).BindToLocalPosition(transform);
                await motionHandle.Value.ToAwaitable(cancellationToken: destroyCancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Empty
            }
        }
    }
}
