#nullable enable
using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using Viroo.Api;
using Viroo.Input.XR;

namespace VirooLab
{
    public class ControllerButtonInputData : MonoBehaviour
    {
        private XRControllerInput? controllerInput;

        [SerializeField]
        private bool isLeft = default;

        [SerializeField]
        private TextMeshProUGUI triggerValue = default!;

        [SerializeField]
        private TextMeshProUGUI gripValue = default!;

        [SerializeField]
        private TextMeshProUGUI primaryValue = default!;

        [SerializeField]
        private TextMeshProUGUI secondaryValue = default!;

        [SerializeField]
        private TextMeshProUGUI axisValue = default!;

        protected void Awake()
        {
            VirooApi.Instance.OnInitialized += OnVirooInitialized;
        }

        private void OnVirooInitialized(object sender, EventArgs e)
        {
            VirooApi.Instance.OnInitialized -= OnVirooInitialized;

            if (isLeft)
            {
                controllerInput = VirooApi.Instance.Input().LeftController;
            }
            else
            {
                controllerInput = VirooApi.Instance.Input().RightController;
            }
        }

        protected void Update()
        {
            if (controllerInput == null)
            {
                return;
            }

            triggerValue.text = controllerInput.Trigger().ToString(CultureInfo.InvariantCulture);
            gripValue.text = controllerInput.Grip().ToString(CultureInfo.InvariantCulture);
            primaryValue.text = controllerInput.PrimaryButtonValue().ToString(CultureInfo.InvariantCulture);
            secondaryValue.text = controllerInput.SecondaryButtonValue().ToString(CultureInfo.InvariantCulture);

            axisValue.text = string.Format(
                CultureInfo.InvariantCulture,
                "{0},{1}",
                controllerInput.Primary2DAxis().x.ToString("0.00", CultureInfo.InvariantCulture),
                controllerInput.Primary2DAxis().y.ToString("0.00", CultureInfo.InvariantCulture));
        }
    }
}
