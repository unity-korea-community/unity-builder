using System.Diagnostics;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using System;
using System.IO;

namespace UNKO.Unity_Builder
{
    public interface IBuildConfig
    {
        BuildTarget buildTarget { get; }

        void ResetSetting(BuildConfig buildConfig);
        string GetBuildPath();

        void OnPreBuild(IDictionary<string, string> commandLine, ref BuildPlayerOptions buildPlayerOptions);
        void OnPostBuild(IDictionary<string, string> commandLine);
    }

    /// <summary>
    /// Unity Inspector에 등록하기 위함..
    /// </summary>
    public abstract class BuildConfigBase : ScriptableObject, IBuildConfig
    {
        public abstract BuildTarget buildTarget { get; }

        public abstract void ResetSetting(BuildConfig buildConfig);
        public abstract string GetBuildPath();

        public abstract void OnPostBuild(IDictionary<string, string> commandLine);
        public abstract void OnPreBuild(IDictionary<string, string> commandLine, ref BuildPlayerOptions buildPlayerOptions);
    }
}
