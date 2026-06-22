#nullable enable
using System;
using System.Collections;
using System.Globalization;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Viroo.Api;
using Virtualware.Networking.Client.Variables;

namespace VirooLab
{
    public class Countdown : MonoBehaviour
    {
        private const string VarName = "CountdownNetworkedTime";

        [SerializeField]
        private TextMeshProUGUI remainingTimeText = default!;

        [SerializeField]
        private float countdownSeconds = 0f;

        private NetworkVariable<float>? currentNetworkedTime = default;

        private float currentTime;
        private bool isCountingDown;

        protected void Awake()
        {
            VirooApi.Instance.OnInitialized += OnVirooInitialized;

            SetInitialTime();
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

            if (currentNetworkedTime.Value > 0)
            {
                currentTime = currentNetworkedTime.Value;

                if (!isCountingDown)
                {
                    StartCoroutine(CountdownCoroutine());
                }
            }
        }

        private void SetInitialTime()
        {
            UpdateTime(countdownSeconds);
            currentTime = countdownSeconds;
        }

        public void StartCountdown()
        {
            if (!isCountingDown)
            {
                StartCoroutine(CountdownCoroutine());
            }
        }

        private IEnumerator CountdownCoroutine()
        {
            isCountingDown = true;

            while (currentTime > 0)
            {
                currentTime -= Time.deltaTime;

                VirooApi.Instance.Networking().ConditionalRunner.RunIfLeader(() => currentNetworkedTime!.Value = currentTime);

                UpdateTime(currentTime);

                yield return null;
            }

            currentTime = 0f;
            UpdateTime(currentTime);
            OnCountdownCompleted();

            isCountingDown = false;
        }

        private async void OnCountdownCompleted()
        {
            await UniTask.Delay(1000, cancellationToken: System.Threading.CancellationToken.None);

            SetInitialTime();
        }

        private void UpdateTime(float time)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(time);

            remainingTimeText.text = timeSpan.ToString(@"mm\:ss", CultureInfo.InvariantCulture);
        }
    }
}
