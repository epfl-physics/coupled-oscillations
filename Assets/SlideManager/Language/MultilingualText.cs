using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class MultilingualText : MonoBehaviour
{
    private TextMeshProUGUI tmp;

    [Header("Texts")]
    [TextArea(3, 10)] public string textEN;
    [TextArea(3, 10)] public string textFR;

    [Header("Language-Specific Transforms")]
    public RectTransform transformsEN;
    public RectTransform transformsFR;

    private string currentTextEN;
    private string currentTextFR;

    private void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        LanguageToggle.onSetLanguage += UpdateText;
    }

    private void OnDisable()
    {
        LanguageToggle.onSetLanguage -= UpdateText;
    }

    private void OnValidate()
    {
        tmp = GetComponent<TextMeshProUGUI>();

        string activeLanguage = "";
        if (currentTextEN != textEN)
        {
            activeLanguage = "EN";
            currentTextEN = textEN;
        }
        else if (currentTextFR != textFR)
        {
            activeLanguage = "FR";
            currentTextFR = textFR;
        }

        UpdateText(activeLanguage);
    }

    private void UpdateText(string activeLanguage)
    {
        if (tmp == null)
        {
            tmp = GetComponent<TextMeshProUGUI>();
        }

        switch (activeLanguage)
        {
            case "EN":
                tmp.text = textEN;
                if (transformsEN) transformsEN.gameObject.SetActive(true);
                if (transformsFR) transformsFR.gameObject.SetActive(false);
                break;
            case "FR":
                tmp.text = textFR;
                if (transformsEN) transformsEN.gameObject.SetActive(false);
                if (transformsFR) transformsFR.gameObject.SetActive(true);
                break;
        }
    }
}