using UnityEngine;

namespace UNKO.Unity_Builder
{
    public abstract class SingletonSOBase<T> : ScriptableObject
        where T : SingletonSOBase<T>
    {
        public static T instance
        {
            get
            {
                if (_instance == null)
                {
                    string typeName = typeof(T).Name;
                    _instance = Resources.Load<T>($"{typeName}");
                    if (_instance == null)
                    {
                        Debug.LogError($"{typeName} instance is null");
                    }
                }

                _instance.OnGetInstance();
                return _instance;
            }
        }

        private static T _instance { get; set; }

        protected virtual void OnGetInstance()
        {
        }
    }
}