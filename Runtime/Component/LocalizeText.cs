using System.Linq;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UNKO.Localize
{
    public class LocalizeText : LocalizeComponentBase
    {
        [SerializeField]
        private Text _text; public Text textComponent => _text;
        public string text { get => _text.text; set => _text.text = value; }

        void Awake()
        {
            if (_text == null)
                _text = GetComponent<Text>();
        }

        protected override void OnSetup()
        {
            base.OnSetup();

            s_manager.OnChangeFont -= OnChangeFont;
            s_manager.OnChangeFont += OnChangeFont;
        }

        protected override void OnChangeLanguage(SystemLanguage language)
        {
            if (string.IsNullOrEmpty(_languageKey))
                return;

            if (_languageParam.Length > 0)
            {
                textComponent.text = s_manager.GetLocalizeText(_languageKey, _languageParam);
            }
            else
            {
                textComponent.text = s_manager.GetLocalizeText(_languageKey);
            }
        }

        void OnChangeFont(Font font)
        {
            textComponent.font = font;
        }
    }

#if UNITY_EDITOR
    // [CustomEditor(typeof(LocalizeText))]
    // public abstract class LocalizeText_Inspector : LocalizeComponentBase_Inspector<LocalizeText>
    // {
    // }
#endif
}
