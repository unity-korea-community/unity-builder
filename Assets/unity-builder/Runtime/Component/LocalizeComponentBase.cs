using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UNKO.Localize
{
    public abstract class LocalizeComponentBase : MonoBehaviour
    {
        static protected bool s_isAppQuitting = false;
        static protected ILocalizeManager s_manager;

        [SerializeField]
        protected string _languageKey;

        [SerializeField]
        protected string[] _languageParam = new string[0];

        public static void Init(ILocalizeManager manager)
        {
            s_manager = manager;
        }

        public void SetLanguageKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogWarning($"{name} SetLanguageKey - key is null or empty", this);
                return;
            }

            _languageKey = key;
            _languageParam = Array.Empty<string>();
            OnChangeLanguage(s_manager.currentLanguage);
        }

        public void SetLanguageKeyWithParam(string key, params string[] param)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogWarning($"{name} SetLanguageKey - key is null or empty", this);
                return;
            }

            _languageKey = key;
            _languageParam = param;
            OnChangeLanguage(s_manager.currentLanguage);
        }

        public void UpdateLocalize()
        {
            OnChangeLanguage(s_manager.currentLanguage);
        }

        void OnEnable()
        {
            if (s_manager == null)
            {
                StartCoroutine(OnEnableCoroutine());
                return;
            }

            OnSetup();
        }

        IEnumerator OnEnableCoroutine()
        {
            int loopCount = 10;
            while (s_manager == null && --loopCount > 0)
            {
                yield return null;
            }

            if (loopCount == 0)
            {
                Debug.LogError($"{name}.OnEnableCoroutine - loopCount == 0 %% _manager == null", this);
                yield break;
            }

            OnSetup();
        }

        void OnDisable()
        {
            if (s_isAppQuitting)
                return;

            s_manager.OnChangeLanguage -= OnChangeLanguage;
        }

        protected virtual void OnSetup()
        {
            if (s_manager == null)
            {
                Debug.LogError($"{name}.Setup - _manager == null", this);
                return;
            }

            s_manager.OnChangeLanguage -= OnChangeLanguage;
            s_manager.OnChangeLanguage += OnChangeLanguage;
            UpdateLocalize();
        }

        private void OnApplicationQuit()
        {
            s_isAppQuitting = true;
        }

        protected abstract void OnChangeLanguage(SystemLanguage language);
    }

#if UNITY_EDITOR
    public abstract class LocalizeComponentBase_Inspector<T> : Editor
        where T : LocalizeComponentBase
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // if (GUILayout.Button("update"))
            // {
            //     Debug.Log("update");
            // }
        }
    }
#endif
}
