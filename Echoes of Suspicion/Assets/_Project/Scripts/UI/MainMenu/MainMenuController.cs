using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public sealed class MainMenuController : MonoBehaviour
{
    [Header("Vistas")]
    [SerializeField]
    private CanvasGroup splashView;

    [SerializeField]
    private CanvasGroup mainMenuView;

    [Header("Navegación")]
    [SerializeField]
    private GameObject firstSelectedButton;

    [Header("Transición")]
    [SerializeField, Min(0.05f)]
    private float transitionDuration = 0.6f;

    private IDisposable inputSubscription;
    private bool menuWasOpened;

    private void Awake()
    {
        ConfigureInitialState();
    }

    private void OnEnable()
    {
        StartListeningForInput();
    }

    private void OnDisable()
    {
        StopListeningForInput();
    }

    private void ConfigureInitialState()
    {
        if (splashView == null || mainMenuView == null)
        {
            Debug.LogError(
                "MainMenuController: faltan referencias de las vistas.",
                this
            );

            enabled = false;
            return;
        }

        splashView.gameObject.SetActive(true);
        splashView.alpha = 1f;
        splashView.interactable = false;
        splashView.blocksRaycasts = false;

        mainMenuView.gameObject.SetActive(true);
        mainMenuView.alpha = 0f;
        mainMenuView.interactable = false;
        mainMenuView.blocksRaycasts = false;

        menuWasOpened = false;
    }

    private void StartListeningForInput()
    {
        if (menuWasOpened)
        {
            return;
        }

        inputSubscription =
            InputSystem.onAnyButtonPress.CallOnce(_ => OpenMainMenu());
    }

    private void StopListeningForInput()
    {
        inputSubscription?.Dispose();
        inputSubscription = null;
    }

    private void OpenMainMenu()
    {
        if (menuWasOpened)
        {
            return;
        }

        menuWasOpened = true;
        StopListeningForInput();

        StartCoroutine(TransitionToMainMenu());
    }

    private IEnumerator TransitionToMainMenu()
    {
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            float progress =
                Mathf.Clamp01(elapsedTime / transitionDuration);

            // Suaviza el inicio y el final de la transición.
            float smoothProgress =
                progress * progress * (3f - 2f * progress);

            splashView.alpha = 1f - smoothProgress;
            mainMenuView.alpha = smoothProgress;

            yield return null;
        }

        splashView.alpha = 0f;
        splashView.gameObject.SetActive(false);

        mainMenuView.alpha = 1f;
        mainMenuView.interactable = true;
        mainMenuView.blocksRaycasts = true;

        SelectFirstButton();
    }

    private void SelectFirstButton()
    {
        if (EventSystem.current == null || firstSelectedButton == null)
        {
            return;
        }

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }
}