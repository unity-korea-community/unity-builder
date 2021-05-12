using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UNKO.Localize.Sample
{
    public class FontInitilaizer : MonoBehaviour
    {
        [System.Serializable]
        public class FontData : ILocalizeFontData
        {
            [SerializeField]
            private SystemLanguage language;
            [SerializeField]
            private Font font;

            public SystemLanguage GetLanguage() => language;
            public Font GetFont() => font;
        }

        [SerializeField]
        private List<FontData> _fontData = new List<FontData>();

        void Awake()
        {
            GetComponent<LocalizeManagerComponent>().AddFontData(_fontData);
        }
    }
}
