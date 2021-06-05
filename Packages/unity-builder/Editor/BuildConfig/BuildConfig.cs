using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

namespace UNKO.Unity_Builder
{
    [StructLayout(LayoutKind.Auto)] // ignore codacy
    public abstract class BuildConfig : BuildConfigBase
    {
        /// <summary>
        /// 예시) com.CompanyName.ProductName  
        /// </summary>
        [SerializeField]
        protected string applicationIdentifier;

        /// <summary>
        /// 설치한 디바이스에 표기될 이름
        /// </summary>
        [SerializeField]
        protected string productName;
        public string GetProductName() => productName;

        [SerializeField]
        protected string defineSymbol;
        public string GetDefineSymbol() => defineSymbol;

        /// <summary>
        /// 빌드에 포함할 씬들, 확장자는 안쓰셔도 됩니다.
        /// <para>예시) ["Assets/SomethingScene_1", "Assets/SomethingScene_1"]</para>
        /// </summary>
        [SerializeField]
        protected string[] buildSceneNames;
        public string[] GetBuildSceneNames() => buildSceneNames;

        [SerializeField]
        protected string bundleVersion;

        // 출력할 폴더 및 파일은 Jenkins에서 처리할 예정이였으나,
        // IL2CPP의 경우 같은 장소에 빌드해놓으면 더 빠르다는 메뉴얼로 인해 일단 보류
        // https://docs.unity3d.com/kr/2020.2/Manual/IL2CPP-OptimizingBuildTimes.html
        [Tooltip("relative Path - UnityProject/Assets/")]
        [Multiline]
        [SerializeField]
        protected string buildPath;

        [SerializeField]
        protected bool developmentBuild;
        [SerializeField]
        protected bool autoRunPlayer;
        [SerializeField]
        protected bool openBuildFolder;

        // List<BuildConfigBase>

        void Reset()
        {
            ResetSetting(this);
        }

        public void AddBuildPath(string addLine)
        {
            buildPath += addLine;
        }



        public override string GetBuildPath()
        {
            DateTime now = DateTime.Now;

            string newBuildPath = Application.dataPath.Replace("/Assets", "/") + buildPath
                .Replace("\n", "")
                .Replace("{applicationIdentifier}", applicationIdentifier)
                .Replace("{productName}", productName)
                .Replace("{bundleVersion}", bundleVersion)

                .Replace("{yyyy}", now.ToString("yyyy"))
                .Replace("{yy}", now.ToString("yy"))
                .Replace("{MM}", now.ToString("MM"))
                .Replace("{dd}", now.ToString("dd"))
                .Replace("{hh}", now.ToString("HH"))
                .Replace("{mm}", now.ToString("mm"))
            ;

            return newBuildPath;
        }

        public override void ResetSetting(BuildConfig config)
        {
            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);

            applicationIdentifier = PlayerSettings.applicationIdentifier;
            productName = PlayerSettings.productName;
            defineSymbol = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            buildSceneNames = GetEnabled_EditorScenes();
            bundleVersion = PlayerSettings.bundleVersion;
            buildPath = "Builds/{productName}\n_{MM}{dd}_{hh}{mm}";
        }

        public override void OnPreBuild(IDictionary<string, string> commandLine, ref BuildPlayerOptions buildPlayerOptions)
        {
            if (string.IsNullOrEmpty(applicationIdentifier) == false)
            {
                PlayerSettings.applicationIdentifier = applicationIdentifier;
            }
            PlayerSettings.bundleVersion = bundleVersion;

            var options = buildPlayerOptions.options;
            if (developmentBuild)
            {
                options |= BuildOptions.Development;
            }

            if (autoRunPlayer)
            {
                options |= BuildOptions.AutoRunPlayer;
            }

            buildPlayerOptions.options = options;
        }

        public override void OnPostBuild(IDictionary<string, string> commandLine)
        {
            if (openBuildFolder)
            {
                string newBuildPath = GetBuildPath();
                string buildFolderPath = Path.GetDirectoryName(newBuildPath);
                Process.Start(buildFolderPath);
            }
        }

        public static string[] GetEnabled_EditorScenes()
        {
            return EditorBuildSettings.scenes
                .Where(p => p.enabled)
                .Select(p => p.path.Replace(".unity", ""))
                .ToArray();
        }
    }
}
