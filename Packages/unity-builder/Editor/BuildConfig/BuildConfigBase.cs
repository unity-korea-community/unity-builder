using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UNKO.Unity_Builder
{
    public interface IBuildConfig
    {
        BuildTarget buildTarget { get; }

        void ResetSetting(BuildConfig config);
        string GetBuildPath();

        void OnPreBuild(IDictionary<string, string> commandLine, ref BuildPlayerOptions buildPlayerOptions);
        void OnPostBuild(IDictionary<string, string> commandLine);
    }

    /// <summary>
    /// Unity Inspector에 등록하기 위해 ScriptableObject를 상속
    /// </summary>
    public abstract class BuildConfigBase : ScriptableObject, IBuildConfig
    {
        /// <summary>
        /// 실행될 빌드 타겟
        /// </summary>
        public abstract BuildTarget buildTarget { get; }

        public abstract void ResetSetting(BuildConfig config);
        public abstract string GetBuildPath();

        public abstract void OnPostBuild(IDictionary<string, string> commandLine);
        public abstract void OnPreBuild(IDictionary<string, string> commandLine, ref BuildPlayerOptions buildPlayerOptions);
    }
}
