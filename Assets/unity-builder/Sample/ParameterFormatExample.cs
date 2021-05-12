using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UNKO.Localize;

public class ParameterFormatExample : MonoBehaviour
{
    [SerializeField]
    InputField input1;

    [SerializeField]
    InputField input2;

    [SerializeField]
    LocalizeText resultText;

    private void Awake()
    {
        // 바로 시작하면 manager가 set이 안되서 에러
        Invoke(nameof(ExampleSetup), 0.1f);
    }

    private void ExampleSetup()
    {
        LocalizeManagerComponentSingleton.instance.OnChangeLanguage += (language) => OnUpdate();

        input1.text = "one";
        input1.onValueChanged.AddListener((value) => OnUpdate());

        input2.text = "two";
        input2.onValueChanged.AddListener((value) => OnUpdate());

        OnUpdate();
    }

    void OnUpdate()
    {
        if (LocalizeManagerComponentSingleton.instance.TryGetLocalizeText(input1.text, out string variable1) == false)
            variable1 = input1.text;

        if (LocalizeManagerComponentSingleton.instance.TryGetLocalizeText(input2.text, out string variable2) == false)
            variable2 = input2.text;

        resultText.SetLanguageKeyWithParam("0is1", variable1, variable2);
    }
}
