using UnityEngine;
using UnityEngine.UI;

public class Activity1Feedback : MonoBehaviour
{
    [SerializeField] private CanvasGroup mode1Feedback;
    [SerializeField] private CanvasGroup mode2Feedback;
    [SerializeField] private SoundEffect successBell;
    [SerializeField] private ParticleSystem confetti;

    private Image background;
    private AudioSource audioSource;
    private bool volumeIsOn = true;

    private void OnEnable()
    {
        CoordinateSpace2D.OnFoundNormalMode += HandleNormalModeFound;
        CoordinateSpace2D.OnPointerClick += HandlePointerClick;
    }

    private void OnDisable()
    {
        CoordinateSpace2D.OnFoundNormalMode -= HandleNormalModeFound;
        CoordinateSpace2D.OnPointerClick -= HandlePointerClick;
    }

    private void Start()
    {
        TryGetComponent(out background);
        TryGetComponent(out audioSource);
        HideAllFeedback();
    }

    private void HideAllFeedback()
    {
        if (mode1Feedback) mode1Feedback.alpha = 0;
        if (mode2Feedback) mode2Feedback.alpha = 0;
        SetBackgroundVisibility(false);
        SetConfettiVisibility(false);
    }

    public void HandleNormalModeFound(int modeNumber)
    {
        SetBackgroundVisibility(true);
        SetConfettiVisibility(true);

        if (successBell && audioSource && volumeIsOn) successBell.Play(audioSource);

        if (modeNumber == 1)
        {
            if (mode1Feedback) mode1Feedback.alpha = 1;
        }
        else if (modeNumber == 2)
        {
            if (mode2Feedback) mode2Feedback.alpha = 1;
        }
    }

    public void HandlePointerClick()
    {
        HideAllFeedback();
    }

    private void SetBackgroundVisibility(bool isVisible)
    {
        if (background) background.enabled = isVisible;
    }

    private void SetConfettiVisibility(bool isVisible)
    {
        if (confetti) confetti.gameObject.SetActive(isVisible);
    }

    public void ToggleVolume(bool isOn)
    {
        volumeIsOn = isOn;
    }
}
