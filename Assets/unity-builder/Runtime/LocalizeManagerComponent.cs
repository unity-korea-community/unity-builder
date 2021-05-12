using System.Collections.Generic;
using UnityEngine;

namespace UNKO.Localize
{
    public class LocalizeManagerComponent : MonoBehaviour, ILocalizeManager
    {
        public event System.Action<SystemLanguage> OnChangeLanguage;
        public event System.Action<Font> OnChangeFont;
        public SystemLanguage currentLanguage { get; private set; }

        private Dictionary<string, ILocalizeData> _languageDictionary = new Dictionary<string, ILocalizeData>();
        private Dictionary<SystemLanguage, ILocalizeFontData> _fontDictionary = new Dictionary<SystemLanguage, ILocalizeFontData>();

        public ILocalizeManager AddData(IEnumerable<ILocalizeData> datas)
        {
            foreach (ILocalizeData data in datas)
                _languageDictionary.Add(data.GetLocalizeID(), data);

            return this;
        }

        public ILocalizeManager AddFontData(IEnumerable<ILocalizeFontData> datas)
        {
            foreach (ILocalizeFontData data in datas)
                _fontDictionary.Add(data.GetLanguage(), data);

            return this;
        }

        public ILocalizeManager ChangeLanguage(SystemLanguage language)
        {
            currentLanguage = language;
            OnChangeLanguage?.Invoke(language);
            if (_fontDictionary.TryGetValue(language, out ILocalizeFontData fontData))
            {
                Font font = fontData.GetFont();
                if (font == null)
                {
                    Debug.LogError($"{name}.{nameof(ChangeLanguage)}(language({language}) font == null");
                    return this;
                }

                OnChangeFont?.Invoke(font);
            }

            return this;
        }

        public string GetLocalizeText(string languageID, params string[] param)
        {
            TryGetLocalizeText(true, languageID, out string result, param);
            return result;
        }

        public bool TryGetLocalizeText(string languageID, out string result, params string[] param)
        {
            return TryGetLocalizeText(false, languageID, out result, param);
        }

        public bool IsValidID(string languageID)
        {
            return _languageDictionary.ContainsKey(languageID);
        }


        bool TryGetLocalizeText(bool printError, string languageID, out string result, params string[] param)
        {
            result = $"Error:'{languageID}'";
            if (string.IsNullOrEmpty(languageID))
                return false;

            if (_languageDictionary.TryGetValue(languageID, out ILocalizeData data) == false)
            {
                if (printError)
                    Debug.LogError($"LanguageManager GetLocalizeText('{languageID}') == not found data");

                return false;
            }

            result = data.GetLocalizeText(currentLanguage);
            if (param.Length > 0)
                result = string.Format(result, param);

            return true;
        }
    }
}
