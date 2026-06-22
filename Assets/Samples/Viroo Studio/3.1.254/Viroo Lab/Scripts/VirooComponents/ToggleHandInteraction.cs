#nullable enable
using System;
using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;
using Viroo.Api;
using Viroo.Interactions.Hand;
using Viroo.Interactions.Mouse;
using Viroo.UI;

namespace VirooLab
{
    /// <summary>
    /// This script is an example of how to disable interaction with Viroo components.
    /// It will reset the interaction when the Viroo menu is shown and stop the countdown coroutine. It will return to the previous state when the menu is closed.
    /// </summary>
    public class ToggleHandInteraction : MonoBehaviour
    {
        private const float CountdownTime = 10;

        [SerializeField]
        private GameObject activateInteractionText = default!;

        [SerializeField]
        private GameObject deactivateInteractionText = default!;

        [SerializeField]
        private TextMeshPro countdownText = default!;

        private bool isInteractionDisabled;
        private bool restoreCountdown;
        private float remainingTime;

        private Coroutine? countdownCoroutine;

        private IHandInteractorProvider? handInteractorProvider;
        private IXRMouseInteractorProvider? mouseInteractorProvider;
        private IPlayerUI? playerUI;

        protected void Inject(IPlayerUI playerUI)
        {
            this.playerUI = playerUI;

            playerUI.OnShowCompleted += OnMenuHide;
            playerUI.OnHideCompleted += OnMenuShow;
        }

        protected void Awake()
        {
            this.QueueForInject();

            VirooApi.Instance.OnInitialized += OnVirooInitialized;

            UpdateText();
        }

        protected void OnDestroy()
        {
            if (playerUI == null)
            {
                return;
            }

            playerUI.OnShowCompleted -= OnMenuHide;
            playerUI.OnHideCompleted -= OnMenuShow;
        }

        private void OnVirooInitialized(object sender, EventArgs e)
        {
            VirooApi.Instance.OnInitialized -= OnVirooInitialized;

            handInteractorProvider = VirooApi.Instance.Interactions().Hands;
            mouseInteractorProvider = VirooApi.Instance.Interactions().Mouse;
        }

        public void DisableInteractors()
        {
            if (handInteractorProvider == null || mouseInteractorProvider == null)
            {
                return;
            }

            if (isInteractionDisabled)
            {
                return;
            }

            ToggleHandInteractor(enableInteraction: false);

            remainingTime = CountdownTime;

            countdownCoroutine = StartCoroutine(CountDown());
        }

        private void OnMenuHide(object sender, EventArgs ee)
        {
            if (isInteractionDisabled)
            {
                StopCoroutine(countdownCoroutine);
                ToggleHandInteractor(enableInteraction: true);

                restoreCountdown = true;
            }
        }

        private void OnMenuShow(object sender, EventArgs ee)
        {
            if (restoreCountdown)
            {
                countdownCoroutine = StartCoroutine(CountDown());
                ToggleHandInteractor(enableInteraction: false);

                restoreCountdown = false;
            }
        }

        private IEnumerator CountDown()
        {
            while (remainingTime > 0)
            {
                countdownText.text = remainingTime.ToString(CultureInfo.InvariantCulture);

                yield return new WaitForSeconds(1f);
                remainingTime--;
            }

            ToggleHandInteractor(enableInteraction: true);
        }

        private void ToggleHandInteractor(bool enableInteraction)
        {
            mouseInteractorProvider!.SetEnabled(this, enableInteraction);
            handInteractorProvider!.SetEnabled(this, enableInteraction);

            isInteractionDisabled = !isInteractionDisabled;

            UpdateText();
        }

        private void UpdateText()
        {
            activateInteractionText.SetActive(value: isInteractionDisabled);
            deactivateInteractionText.SetActive(value: !isInteractionDisabled);
        }
    }
}
