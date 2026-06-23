#nullable enable
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Viroo.Interactions;

namespace VirooLab
{
    [RequireComponent(typeof(Button))]
    public class ChangeLanguageButton : MonoBehaviour
    {
        private const string TextFormat = "{0} ({1})";

        public event EventHandler OnClicked = (sender, e) => { };

        [SerializeField]
        private TextMeshProUGUI label = default!;

        [SerializeField]
        private ChangeLanguageAction action = default!;

        protected void Awake()
        {
            Button button = GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                action.LocalExecute();
                NotifyClicked();
            });
        }

        public void Initialize(CultureInfo cultureInfo)
        {
            Regex regex = new("(?<lang>.*?) \\(", RegexOptions.Compiled, TimeSpan.FromSeconds(1));
            Match match = regex.Match(cultureInfo.DisplayName);

            string languageName = match.Groups[1].ToString().ToUpper(CultureInfo.InvariantCulture);
            string languageCode = cultureInfo.Name;

            label.text = string.Format(TextFormat, languageName, languageCode);
            action.Locale = cultureInfo.Name;
        }

        private void NotifyClicked()
        {
            OnClicked(this, EventArgs.Empty);
        }
    }
}
