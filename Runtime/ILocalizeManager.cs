using System.Collections.Generic;
using UnityEngine;

namespace UNKO.Localize
{
    public interface ILocalizeManager
    {
        event System.Action<SystemLanguage> OnChangeLanguage;
        event System.Action<Font> OnChangeFont;
        SystemLanguage currentLanguage { get; }

        ILocalizeManager AddData(IEnumerable<ILocalizeData> datas);
        ILocalizeManager AddFontData(IEnumerable<ILocalizeFontData> datas);
        ILocalizeManager ChangeLanguage(SystemLanguage language);

        string GetLocalizeText(string languageID, params string[] param);
        bool TryGetLocalizeText(string languageID, out string result, params string[] param);
    }
}
