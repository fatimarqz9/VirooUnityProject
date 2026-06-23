#nullable enable
using System;
using UnityEngine;

namespace VirooLab
{
    public class CubeTakePosition : MonoBehaviour
    {
        [SerializeField]
        private RobotArmController robotArmController = default!;

        protected void OnTriggerEnter(Collider other)
        {
            VirooTag virooTag = other.GetComponent<VirooTag>();

            if (virooTag && virooTag.Tag.Equals("RobotArmGrabbableCube", StringComparison.Ordinal))
            {
                robotArmController.AnimateRobot(other.gameObject);
            }
        }
    }
}
