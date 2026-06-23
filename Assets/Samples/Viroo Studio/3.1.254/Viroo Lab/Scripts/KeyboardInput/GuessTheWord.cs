#nullable enable
using System;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard;
using Viroo.Interactions;

namespace VirooLab
{
    public class GuessTheWord : BroadcastObjectAction
    {
        private const string TargetWord = "VIROO";
        private const string Red = "#D93E34";
        private const string Green = "#68C36C";
        private const string Yellow = "#C8C821";

        [SerializeField]
        private XRKeyboard keyboard = default!;

        [SerializeField]
        private TMP_Text[] letterTexts = default!;

        [SerializeField]
        private AudioSource correctAudioSource = default!;

        [SerializeField]
        private AudioSource wrongAudioSource = default!;

        private Color redColor;
        private Color greenColor;
        private Color yellowColor;

        protected new void Awake()
        {
            base.Awake();

            if (!ColorUtility.TryParseHtmlString(Red, out redColor))
            {
                redColor = Color.red;
            }

            if (!ColorUtility.TryParseHtmlString(Green, out greenColor))
            {
                greenColor = Color.green;
            }

            if (!ColorUtility.TryParseHtmlString(Yellow, out yellowColor))
            {
                yellowColor = Color.yellow;
            }

            ClearLetters();
        }

        protected void OnEnable()
        {
            keyboard.onTextSubmitted.AddListener(OnTextSubmitted);
        }

        protected new void OnDestroy()
        {
            base.OnDestroy();

            keyboard.onTextSubmitted.RemoveListener(OnTextSubmitted);
        }

        private void OnTextSubmitted(KeyboardTextEventArgs arg0)
        {
            Execute(arg0.keyboardText.ToUpperInvariant());

            keyboard.Clear();
        }

        private void ClearLetters()
        {
            foreach (TMP_Text letterText in letterTexts)
            {
                letterText.text = string.Empty;
            }
        }

        private void CheckWord(string word)
        {
            if (word.Length != TargetWord.Length)
            {
                word = word.PadRight(TargetWord.Length);
            }

            for (int i = 0; i < 5; i++)
            {
                letterTexts[i].text = word[i].ToString();

                if (word[i] == TargetWord[i])
                {
                    letterTexts[i].color = greenColor;
                }
                else if (TargetWord.Contains(word[i].ToString(), StringComparison.InvariantCulture))
                {
                    letterTexts[i].color = yellowColor;
                }
                else
                {
                    letterTexts[i].color = redColor;
                }
            }

            if (string.Equals(word, TargetWord, StringComparison.OrdinalIgnoreCase))
            {
                correctAudioSource.Play();
            }
            else
            {
                wrongAudioSource.Play();
            }
        }

        protected override void LocalExecuteImplementation(string data) => CheckWord(data);
    }
}
