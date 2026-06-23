#nullable enable
using System;
using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;
using Viroo.Api;
using Virtualware.Networking.Client.Variables;

namespace VirooLab
{
    public class ElapsedTime : MonoBehaviour
    {
        private const string VarName = "ElapsedNetworkedTime";

        [SerializeField]
        private TextMeshProUGUI elapsedTimeText = default!;

        private NetworkVariable<float>? currentNetworkedTime = default;

        private float currentTime;
        private bool runningTime;

        protected void Awake()
        {
            VirooApi.Instance.OnInitialized += OnVirooInitialized;
            runningTime = true;
        }

        private void OnVirooInitialized(object sender, EventArgs e)
        {
            VirooApi.Instance.OnInitialized -= OnVirooInitialized;

            string networkedVariableName = $"{VarName}_{name}";

            currentNetworkedTime = VirooApi.Instance.Networking().CreateNetworkVariable<float>(networkedVariableName, 0);

            currentNetworkedTime.OnInitialized += OnNetworkVariableInitialized;
        }

        private void OnNetworkVariableInitialized(object sender, float e)
        {
            currentNetworkedTime!.OnInitialized -= OnNetworkVariableInitialized;

            currentTime = currentNetworkedTime.Value;

            StartCoroutine(CountdownCoroutine());
        }

        private IEnumerator CountdownCoroutine()
        {
            while (runningTime)
            {
                currentTime += Time.deltaTime;

                VirooApi.Instance.Networking().ConditionalRunner.RunIfLeader(() => currentNetworkedTime!.Value = currentTime);

                UpdateTime(currentTime);

                yield return null;
            }
        }

        private void UpdateTime(float time)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(time);

            elapsedTimeText.text = timeSpan.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture);
        }

        protected void OnDestroy()
        {
            runningTime = false;
        }
    }
}
