using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class LanguageToggle : MonoBehaviour, IPointerClickHandler
{
    public enum Language { EN, FR }
    public Language activeLanguage = Language.EN;

    public enum Theme { Light, Dark }
    [SerializeField] private Theme theme = Theme.Light;

    public LanguageState state = default;

    public delegate void OnSetLanguage(string language);
    public static event OnSetLanguage onSetLanguage;

    [Header("TMPs")]
    [SerializeField] private TextMeshProUGUI labelEN = default;
    [SerializeField] private TextMeshProUGUI labelFR = default;

    [Header("Light Theme")]
    [SerializeField] private Color lightActiveColor = Color.black;
    [SerializeField] private Color lightInactiveColor = Color.gray;

    [Header("Dark Theme")]
    [SerializeField] private Color darkActiveColor = Color.gray;
    [SerializeField] private Color darkInactiveColor = Color.black;

    private Color active;
    private Color inactive;

    private void Start()
    {
        // Signal to other listening scripts to display the active language
        SetTheme(theme);
        // SetLanguage(activeLanguage);
        if (state)
        {
            SetLanguage(state.language);
        }
        else
        {
            SetLanguage(activeLanguage);
        }
    }

    // private void OnValidate()
    // {
    //     Debug.Log("LanguageToggle OnValidate");
    //     SetTheme(theme);
    //     SetLanguage(activeLanguage);
    // }

    public void SetLanguage(Language language)
    {
        activeLanguage = language;
        UpdateLanguageDisplay(activeLanguage);
        onSetLanguage?.Invoke(activeLanguage.ToString());
        if (state) state.language = language;
    }

    public void SetTheme(Theme theme)
    {
        switch (theme)
        {
            case Theme.Light:
                active = lightActiveColor;
                inactive = lightInactiveColor;
                break;
            case Theme.Dark:
                active = darkActiveColor;
                inactive = darkInactiveColor;
                break;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ToggleDisplay();
        onSetLanguage?.Invoke(activeLanguage.ToString());
    }

    private void ToggleDisplay()
    {
        activeLanguage = (activeLanguage == Language.EN) ? Language.FR : Language.EN;
        if (state) state.language = activeLanguage;
        UpdateLanguageDisplay(activeLanguage);
    }

    private void UpdateLanguageDisplay(Language language)
    {
        if (!labelEN || !labelFR) { return; }

        switch (language)
        {
            case Language.EN:
                labelEN.color = active;
                labelFR.color = inactive;
                break;
            case Language.FR:
                labelEN.color = inactive;
                labelFR.color = active;
                break;
        }
    }
}
