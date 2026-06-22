#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using LitMotion;
using UnityEngine;
using UnityEngine.UI;
using Viroo.SceneLoader.SceneContext;

namespace VirooLab
{
    public class ChangeLanguageWindow : MonoBehaviour
    {
        [SerializeField]
        private ChangeLanguageButton buttonPrefab = default!;

        [SerializeField]
        private GridLayoutGroup gridLayout = default!;

        [SerializeField]
        private RectTransform buttonsHierarchy = default!;

        [SerializeField]
        private CanvasGroup canvasGroup = default!;

        [SerializeField]
        private float hideDuration = 0.5f;

        private readonly List<ChangeLanguageButton> changeLanguageButtons = new();

        private ISceneLocalizationService? sceneLocalizationService;

        protected void Inject(ISceneLocalizationService sceneLocalizationService)
        {
            this.sceneLocalizationService = sceneLocalizationService;

            sceneLocalizationService.OnInitialized += SceneLocalizationServiceOnInitialized;
        }

        protected void Awake()
        {
            this.QueueForInject();
        }

        private void SceneLocalizationServiceOnInitialized(object sender, EventArgs e)
        {
            (sender as ISceneLocalizationService)!.OnInitialized -= SceneLocalizationServiceOnInitialized;

            if (gridLayout.constraint == GridLayoutGroup.Constraint.FixedColumnCount &&
                sceneLocalizationService!.AvailableCultures!.Count < gridLayout.constraintCount)
            {
                gridLayout.constraintCount = sceneLocalizationService.AvailableCultures.Count;
            }

            foreach (CultureInfo culture in sceneLocalizationService!.AvailableCultures!)
            {
                ChangeLanguageButton button = Instantiate(buttonPrefab, buttonsHierarchy);
                button.Initialize(culture);
                button.OnClicked += OnLanguageClicked;

                changeLanguageButtons.Add(button);
            }
        }

        public void Hide()
        {
            foreach (ChangeLanguageButton button in changeLanguageButtons)
            {
                button.OnClicked -= OnLanguageClicked;
            }

            _ = LMotion.Create(canvasGroup.alpha, 0, hideDuration)
                .WithEase(Ease.Linear)
                .WithOnComplete(() =>
                {
                    canvasGroup.blocksRaycasts = false;
                    canvasGroup.interactable = false;
                })
                .Bind(value => canvasGroup.alpha = value);
        }

        private void OnLanguageClicked(object sender, EventArgs e)
        {
            Hide();
        }
    }
}
