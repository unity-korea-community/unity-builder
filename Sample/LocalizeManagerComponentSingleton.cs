using System.Collections.Generic;
using UnityEngine;

namespace UNKO.Localize
{
    public class LocalizeManagerComponentSingleton : LocalizeManagerComponent
    {
        public static LocalizeManagerComponentSingleton instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<LocalizeManagerComponentSingleton>();
                    LocalizeComponentBase.Init(_instance);
                }

                return _instance;
            }
        }

        static LocalizeManagerComponentSingleton _instance;
    }
}
