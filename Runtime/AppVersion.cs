using System;
using UnityEngine;

namespace UNKO.Unity_Builder
{
    [CreateAssetMenu(fileName = "AppVersion", menuName = "UNKO/AppVersion", order = 0)]
    public class AppVersion : SingletonSOBase<AppVersion>
    {
        public string VersionString => _versionString;

        [SerializeField]
        private string _versionString = string.Empty;

        public void SetVersion(string versionString)
        {
            if (versionString.Contains(".") == false)
            {
                versionString = $"0.{versionString}";
                Debug.Log($"{nameof(AppVersion)}.{nameof(SetVersion)} edit, input:{versionString}");
            }
            _versionString = versionString;

            bool parseResult = Version.TryParse(versionString, out Version version);
            Debug.Log($"{nameof(AppVersion)}.{nameof(SetVersion)}, input:{version}, parseResult:{parseResult}");
        }

        public bool TryGetVersion(out Version version)
        {
            return Version.TryParse(_versionString, out version);
        }
    }

    public static class VersionEx
    {
        public static bool IsGreater(this Version target, Version other)
        {
            bool isError = false;
            if (target == null)
            {
                Debug.LogError("Version.IsGreater fail, target is null");
                isError = true;
            }
            if (other == null)
            {
                Debug.LogError("Version.IsGreater fail, other is null");
                isError = true;
            }

            if (isError)
            {
                return false;
            }

            int majorResult = target.Major.CompareTo(other.Major);
            if (majorResult > 0)
            {
                return true;
            }

            int minorResult = target.Minor.CompareTo(other.Minor);
            if (minorResult > 0)
            {
                return true;
            }

            int buildResult = target.Build.CompareTo(other.Build);
            if (buildResult > 0)
            {
                return true;
            }

            return false;
        }
    }
}