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
    /// <summary>
    /// 플랫폼 별 빌드 설정 부모 클래스 
    /// </summary>
    [StructLayout(LayoutKind.Auto)] // ignore codacy
    public abstract class BuildConfigBase : BuildConfigSO
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
        /// <summary>
        /// productName를 얻습니다.
        /// </summary>
        /// <returns>productName</returns>
        public string GetProductName() => productName;

        /// <summary>
        /// script define symbol입니다.
        /// </summary>
        [SerializeField]
        protected string defineSymbol;
        /// <summary>
        /// script define을 얻습니다.
        /// </summary>
        /// <returns></returns>
        public string GetDefineSymbol() => defineSymbol;

        /// <summary>
        /// 에디터 세팅에 있는 씬을 사용할 지
        /// </summary>
        public bool useScenesInEditorSetting;

        /// <summary>
        /// 빌드에 포함할 씬들, 확장자는 포함해도 되고 포함하지 않아도 됩니다..
        /// <para>예시) ["Assets/SomethingScene_1", "Assets/SomethingScene_1"]</para>
        /// </summary>
        [SerializeField]
        protected string[] buildSceneNames;

        /// <summary>
        /// 빌드에 포함할 씬 이름들을 얻습니다.
        /// </summary>
        /// <returns>buildSceneNames</returns>
        public string[] GetBuildSceneNames() => buildSceneNames;

        /// <summary>
        /// 번들 버전을 에디터 세팅의 값으로 덮어씁니다.
        /// </summary>
        public bool overrideEditorSettingBundleVersion = true;

        /// <summary>
        /// 번들 버젼을 얻습니다. (=앱 버전)
        /// </summary>
        public string bundleVersion;


        /// <summary>
        /// 출력할 빌드 경로, 절대경로가 아니면 UnityProject/Assets/의 상대 경로입니다.
        /// NOTE IL2CPP의 경우 같은 장소에 빌드해놓으면 더 빠르다는 메뉴얼
        /// https://docs.unity3d.com/kr/2020.2/Manual/IL2CPP-OptimizingBuildTimes.html
        /// </summary>
        [Tooltip("absolute or relative Path, when relative:UnityProject/Assets/")]
        [Multiline]
        [SerializeField]
        protected string buildPath;

        /// <summary>
        /// buildsetting - developmentBuild 옵션
        /// </summary>
        [SerializeField]
        protected bool developmentBuild;

        /// <summary>
        /// buildsetting - autoRunPlayer 옵션
        /// TODO 아직 테스트가 제대로 안되있습니다.
        /// </summary>
        [SerializeField]
        protected bool autoRunPlayer;

        /// <summary>
        /// buildsetting - autoRunPlayer 옵션, 아직 테스트가 제대로 안되있습니다.
        /// </summary>
        [SerializeField]
        protected bool openBuildFolder;

        /// <summary>
        /// AppVersion에 어떻게 버전을 할당할 지
        /// </summary>
        public string versionString;

        void Reset()
        {
            ResetSetting(this);
        }

        /// <summary>
        /// 빌드 경로에 라인을 추가합니다.
        /// </summary>
        /// <param name="addLine">빌드 경로에 추가할 라인</param>
        public void AddBuildPath(string addLine)
        {
            buildPath += addLine;
        }

        ///<inheritdoc cref="IBuildConfig.GetBuildPath"/>
        public override string GetBuildPath()
        {
            string newBuildPath = ReplaceStrings(buildPath);
            bool isAbsolutePath = Path.IsPathRooted(newBuildPath);
            if (isAbsolutePath == false)
            {
                newBuildPath = Application.dataPath.Replace("/Assets", "/") + newBuildPath;
            }

            UnityEngine.Debug.Log($"isAbsolute:{isAbsolutePath}, buildPath:{newBuildPath}");

            return newBuildPath;
        }

        ///<inheritdoc cref="IBuildConfig.ResetSetting"/>
        public override void ResetSetting(BuildConfigBase config)
        {
            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);

            applicationIdentifier = PlayerSettings.applicationIdentifier;
            productName = PlayerSettings.productName;
            defineSymbol = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            buildSceneNames = GetEnabled_EditorScenes();
            bundleVersion = PlayerSettings.bundleVersion;
            buildPath = "Builds/{productName}\n_{MM}{dd}_{hh}{mm}";
            versionString = $"1.{{bundleVersion}}";
        }

        ///<inheritdoc cref="IBuildConfig.OnPreBuild"/>
        public override void OnPreBuild(IDictionary<string, string> commandLine, ref BuildPlayerOptions buildPlayerOptions)
        {
            if (string.IsNullOrEmpty(applicationIdentifier) == false)
            {
                PlayerSettings.applicationIdentifier = applicationIdentifier;
            }

            if (overrideEditorSettingBundleVersion == false)
            {
                PlayerSettings.bundleVersion = bundleVersion;
            }

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

            string version = ReplaceStrings(versionString);
            AppVersion.instance.SetVersion(version);

            if (AppVersion.instance.TryGetVersion(out Version parsedVersion))
            {
                UnityEngine.Debug.Log($"Set appversion:{version}, parsed Major:{parsedVersion.Major}, Minor:{parsedVersion.Minor}, Build:{parsedVersion.Build}");
            }
            else
            {
                UnityEngine.Debug.Log($"Set appversion:{version}");
            }
        }

        ///<inheritdoc cref="IBuildConfig.OnPostBuild"/>
        public override void OnPostBuild(IDictionary<string, string> commandLine)
        {
            if (Application.isBatchMode == false)
            {
                if (openBuildFolder)
                {
                    string newBuildPath = GetBuildPath();
                    string buildFolderPath = Path.GetDirectoryName(newBuildPath);
                    Process.Start(buildFolderPath);
                }
            }
        }

        public virtual string ReplaceStrings(string replaceTarget)
        {
            if (string.IsNullOrEmpty(replaceTarget))
            {
                UnityEngine.Debug.LogError("ReplaceString string is null or empty");
                return string.Empty;
            }

            DateTime now = DateTime.Now;
            return replaceTarget.Replace("\n", "")
                .Replace("{versionString}", versionString)
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
        }

        /// <summary>
        /// 현재 Editor BuildSetting에서 빌드에 포함되는 씬 중에 활성화된 씬을 얻습니다.
        /// </summary>
        /// <returns>빌드에 포함되고 활성화 되있는 씬들</returns>
        public static string[] GetEnabled_EditorScenes()
        {
            return EditorBuildSettings.scenes
                .Where(p => p.enabled)
                .Select(p => p.path.Replace(".unity", ""))
                .ToArray();
        }
    }
}
