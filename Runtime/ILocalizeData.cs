using UnityEngine;

namespace UNKO.Localize
{
    /// <summary>
    /// 로컬라이즈 텍스트 데이터
    /// </summary>
    public interface ILocalizeData
    {
        string GetLocalizeID();
        string GetLocalizeText(SystemLanguage systemLanguage);
    }

    /// <summary>
    /// 로컬라이즈 폰트 데이터
    /// </summary>
    public interface ILocalizeFontData
    {
        SystemLanguage GetLanguage();

        Font GetFont();
    }
}
