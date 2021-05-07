using System;
using UnityEngine;
using UnityEditor;
using System.Collections.ObjectModel;

namespace Unity_CLI
{
    public interface IBuildConfig
    {
        BuildTarget buildTarget { get; }
        Texture icon { get; }

        void ResetSetting();
        void OnPreBuild(CLIBuilder builder, ReadOnlyDictionary<string, string> commandLine);
        void OnPostBuild(CLIBuilder builder, ReadOnlyDictionary<string, string> commandLine);
    }

    public abstract class BuildConfigBase : ScriptableObject, IBuildConfig
    {
        public abstract BuildTarget buildTarget { get; }
        public virtual Texture icon => EditorGUIUtility.FindTexture("BuildSettings.Editor.Small");

        /// <summary>
        /// 예시) com.CompanyName.ProductName  
        /// </summary>
        public string applicationIdentifier;

        /// <summary>
        /// 출력할 파일 명, cli에서 -filename (filename:string) 로 설정가능
        /// </summary>
        public string filename = "Build";

        public string defineSymbol;

        /// <summary>
        /// 설치한 디바이스에 표기될 이름
        /// </summary>
        public string productName;

        /// <summary>
        /// 빌드에 포함할 씬들, 확장자는 안쓰셔도 됩니다.
        /// <para>예시) ["Assets/SomethingScene_1", "Assets/SomethingScene_1"]</para>
        /// </summary>
        public string[] buildSceneNames;

        public string bundleVersion;

        // 출력할 폴더 및 파일은 Jenkins에서 처리할 예정이였으나,
        // IL2CPP의 경우 같은 장소에 빌드해놓으면 더 빠르다는 메뉴얼로 인해 일단 보류
        // https://docs.unity3d.com/kr/2020.2/Manual/IL2CPP-OptimizingBuildTimes.html
        public string buildPath;

        void Reset()
        {
            ResetSetting();
        }

        public virtual string GetBuildPath()
        {
            return buildPath;
        }

        public virtual void ResetSetting()
        {
            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);

            applicationIdentifier = PlayerSettings.applicationIdentifier;
            productName = PlayerSettings.productName;
            defineSymbol = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            buildSceneNames = CLIBuilder.GetEnabled_EditorScenes();
            buildPath = Application.dataPath.Replace("/Assets", "") + "/Builds";
            bundleVersion = PlayerSettings.bundleVersion;
        }

        public virtual void OnPreBuild(CLIBuilder builder, ReadOnlyDictionary<string, string> commandLine)
        {
            if (string.IsNullOrEmpty(applicationIdentifier) == false)
                PlayerSettings.applicationIdentifier = applicationIdentifier;

            PlayerSettings.bundleVersion = bundleVersion;

        }
        public abstract void OnPostBuild(CLIBuilder builder, ReadOnlyDictionary<string, string> commandLine);
    }
}
