using System.Collections;
using UnityEngine;

public class SlideManager : MonoBehaviour
{
    [Header("Header")]
    [SerializeField] private Transform header;
    [SerializeField] private bool showHeader = true;

    [Header("Slides")]
    [SerializeField] private Transform slideContainer;
    [SerializeField] private int currentSlideIndex = 0;

    [Header("Navigation")]
    [SerializeField] private Transform navigation;
    [SerializeField] private bool clickableBubbles = true;
    [SerializeField] private bool useProgressBar;

    [Header("Slide Transitions")]
    [SerializeField, Min(0)] private float fadeInTime = 0.3f;
    [SerializeField, Min(0)] private float fadeInDelay = 0f;
    [SerializeField, Min(0)] private float fadeOutTime = 0.3f;
    [SerializeField, Min(0)] private float fadeOutDelay = 0f;

    private static Camera mainCamera;

    private void Awake()
    {
        // Set header visibility
        if (header != null)
        {
            header.gameObject.SetActive(showHeader);
        }

        // Get reference to the main camera (to pass on to camera controllers)
        mainCamera = Camera.main;

        // Get reference to the Slides container if it exists
        if (slideContainer == null)
        {
            Debug.LogWarning("A SlideContainer has not been assigned.");
            return;
        }

        // Hide all UI elements of each slide using its CanvasGroup
        foreach (var canvasGroup in slideContainer.GetComponentsInChildren<CanvasGroup>())
        {
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
        }

        // Turn off all simulations initially that have an associated SlideController
        foreach (var slideController in slideContainer.GetComponentsInChildren<SimulationSlideController>())
        {
            slideController.DeactivateSimulation();
        }
    }

    private void Start()
    {
        InitializeSlides();  // Activate the current slide and deactivate all others
        GenerateNavigationUI();
    }

    private void InitializeSlides()
    {
        // Do nothing if no slide container is assigned or if it has no slides
        if (slideContainer == null) { return; }

        if (slideContainer.childCount == 0)
        {
            Debug.LogWarning("Slides GameObject does not contain any actual slides.");
            return;
        }

        // Activate the UI and associated simulations of the starting slide
        Transform slide = slideContainer.GetChild(currentSlideIndex);
        if (slide.TryGetComponent(out CanvasGroup canvasGroup))
        {
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
        }
        if (slide.TryGetComponent(out CameraController cameraController))
        {
            cameraController.AssignCameraReference(mainCamera);
            cameraController.InitializeCamera();
        }
        foreach (var simSlideController in slide.GetComponents<SimulationSlideController>())
        {
            simSlideController.ActivateSimulation();
            simSlideController.enabled = true;
        }
    }

    private void GenerateNavigationUI()
    {
        // Do not create navigation UI if there are no slides
        if (slideContainer == null) { return; }

        if (navigation == null)
        {
            Debug.LogWarning("SlideManager did not find a child GameObject called Navigation.");
            return;
        }

        // Create navigation bubbles (or progress bar) and set the correct one active
        if (navigation.TryGetComponent(out Navigation nav))
        {
            if (useProgressBar)
            {
                nav.useProgressBar = true;
                nav.SetNumSlides(slideContainer.childCount);
            }
            else
            {
                nav.SetBubbleClickability(clickableBubbles);
                nav.GenerateBubbles(slideContainer.childCount);
            }
            nav.SetCurrentSlideIndex(currentSlideIndex);
            nav.ChangeSlide(currentSlideIndex, false);
        }
    }

    public void LoadSlide(int slideIndex)
    {
        if (slideContainer == null || currentSlideIndex == slideIndex)
        {
            return;
        }

        //Debug.Log("Slide Manager > Loading Slide index " + slideIndex);

        // Turn off the current slide
        Transform prevSlide = slideContainer.GetChild(currentSlideIndex);
        if (prevSlide.TryGetComponent(out CanvasGroup prevCG))
        {
            prevCG.blocksRaycasts = false;
            StartCoroutine(FadeSlide(prevCG, 0, fadeOutTime, fadeOutDelay));
        }
        // Release the current slide's camera reference
        if (prevSlide.TryGetComponent(out CameraController prevCC))
        {
            prevCC.ReleaseCameraReference();
        }
        // Deactivate all simulations associated to this slide
        foreach (var prevSSC in prevSlide.GetComponents<SimulationSlideController>())
        {
            prevSSC.DeactivateSimulation();
            prevSSC.enabled = false;
        }

        // Turn on the new slide
        Transform nextSlide = slideContainer.GetChild(slideIndex);
        if (nextSlide.TryGetComponent(out CanvasGroup nextCG))
        {
            nextCG.blocksRaycasts = true;
            StartCoroutine(FadeSlide(nextCG, 1, fadeInTime, fadeInDelay));
        }
        // Pass the camera reference to the new slide and move it to the right place
        if (nextSlide.TryGetComponent(out CameraController nextCC))
        {
            nextCC.AssignCameraReference(mainCamera);
            nextCC.InitializeCamera();
        }
        // Activate all simulations associated to this slide
        foreach (var nextSSC in nextSlide.GetComponents<SimulationSlideController>())
        {
            nextSSC.ActivateSimulation();
            nextSSC.enabled = true;
        }

        currentSlideIndex = slideIndex;
    }

    private IEnumerator FadeSlide(CanvasGroup canvasGroup, float targetAlpha, float fadeTime, float startDelay = 0)
    {
        yield return new WaitForSeconds(startDelay);

        float time = 0;
        float startAlpha = canvasGroup.alpha;

        while (time < fadeTime)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeTime);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }

    public void HandleThemeChange(Color backgroundColor)
    {
        if (backgroundColor == Color.white)
        {
            BroadcastMessage("SetLightTheme", SendMessageOptions.DontRequireReceiver);
        }

        if (backgroundColor == Color.black)
        {
            BroadcastMessage("SetDarkTheme", SendMessageOptions.DontRequireReceiver);
        }
    }
}