#nullable enable
using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using Viroo.Api;
using Virtualware.Networking.Client;
using Virtualware.Networking.Client.Variables;

namespace VirooLab
{
    public class Score : MonoBehaviour
    {
        private const string ScoreFormat = "{0:D2}";

        [SerializeField]
        private TextMeshProUGUI scoreLabel = default!;

        private NetworkVariable<int>? scoreVariable;
        private NetworkObject? networkObject;

        private bool HasAuthority => networkObject!.Authority;

        protected void Awake()
        {
            VirooApi.Instance.OnInitialized += OnVirooInitialized;
        }

        protected void OnDestroy()
        {
            if (scoreVariable != null)
            {
                scoreVariable.Remove();

                scoreVariable.OnInitialized -= OnVariableChanged;
                scoreVariable.OnValueChanged -= OnVariableChanged;
            }
        }

        private void OnVirooInitialized(object sender, EventArgs e)
        {
            VirooApi.Instance.OnInitialized -= OnVirooInitialized;

            networkObject = GetComponent<NetworkObject>();

            scoreVariable = VirooApi.Instance.Networking().CreateNetworkVariable("ScoreVariable", 0);

            scoreVariable.OnInitialized += OnVariableChanged;
            scoreVariable.OnValueChanged += OnVariableChanged;
        }

        // This event is also called when the variable is initialized
        private void OnVariableChanged(object sender, int value)
        {
            scoreLabel.text = string.Format(CultureInfo.InvariantCulture, ScoreFormat, value);
        }

        public void Increment()
        {
            if (HasAuthority)
            {
                scoreVariable!.Value++;
            }
        }
    }
}
