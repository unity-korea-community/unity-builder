using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UNKO.Localize;

[RequireComponent(typeof(Button))]
public class ChangeLanguageButton : MonoBehaviour
{
    [SerializeField]
    UNKO.Localize.Sample.CSVLocalizeInitializer.SupportLanguage language;

    private void Awake()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(ChangeLanguage);
    }

    public void ChangeLanguage()
    {
        switch (language)
        {
            case UNKO.Localize.Sample.CSVLocalizeInitializer.SupportLanguage.kor:
                FindObjectOfType<LocalizeManagerComponent>().ChangeLanguage(SystemLanguage.Korean);
                break;

            case UNKO.Localize.Sample.CSVLocalizeInitializer.SupportLanguage.eng:
                FindObjectOfType<LocalizeManagerComponent>().ChangeLanguage(SystemLanguage.English);
                break;
        }
    }
}
