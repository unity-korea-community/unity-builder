using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace UNKO.Unity_Builder
{
    /// <summary>
    /// 유니티 프로젝트를 <see cref="BuildConfigBase"/>으로 빌드하는 static class
    /// </summary>
    public static class UnityBuilder
    {
        /// <summary>
        /// 빌드 전 세팅 (빌드 후 되돌리기용)
        /// </summary>
        public class PlayerSetting_Backup
        {
            /// <summary>
            /// 백업할 script define symbol
            /// </summary>
            public string DefineSymbol { get; private set; }

            /// <summary>
            /// 백업할 product name
            /// </summary>
            /// <value></value>
            public string ProductName { get; private set; }

            /// <summary>
            /// <see cref="PlayerSettings.SetScriptingDefineSymbolsForGroup"/>을 호출하기 위해 필요
            /// </summary>
            /// <value></value>
            public BuildTargetGroup BuildTargetGroup { get; private set; }

            /// <summary>
            /// 생성자
            /// </summary>
            /// <param name="buildTargetGroup"><see cref="PlayerSettings.SetScriptingDefineSymbolsForGroup"/>을 호출하기 위해 필요</param>
            /// <param name="defineSymbol">백업할 script define symbol</param>
            /// <param name="productName">백업할 product name</param>
            public PlayerSetting_Backup(BuildTargetGroup buildTargetGroup, string defineSymbol, string productName)
            {
                this.BuildTargetGroup = buildTargetGroup;
                this.DefineSymbol = defineSymbol;
                this.ProductName = productName;
            }

            /// <summary>
            /// Player Setting에 복구
            /// </summary>
            public void Restore()
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup, DefineSymbol);
                PlayerSettings.productName = ProductName;
            }
        }

        /// <summary>
        /// 커맨드라인의 configpath에서 <see cref="BuildConfigBase"/>를 얻은 뒤 해당 config로 빌드합니다.
        /// </summary>
        public static void Build()
        {
            if (TryGetSO_FromCommandLine("configpath", out BuildConfigBase config))
            {
                Build(config);
            }
            else
            {
                Debug.LogError("require -configpath");
            }
        }

        /// <summary>
        /// <see cref="BuildConfigBase"/>로 빌드합니다.
        /// </summary>
        /// <param name="buildConfig">빌드에 사용할 config</param>
        public static void Build(BuildConfigBase buildConfig)
        {
            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildConfig.buildTarget);
            BuildPlayerOptions buildPlayerOptions = Generate_BuildPlayerOption(buildConfig);
            PlayerSetting_Backup editorSetting_Backup = SettingBuildConfig_To_EditorSetting(buildConfig, buildTargetGroup);

            string overwriteConfigJson = Environment.GetEnvironmentVariable("overwrite");
            if (string.IsNullOrEmpty(overwriteConfigJson) == false)
            {
                Debug.Log($"overwrite config : {overwriteConfigJson}");
                JsonUtility.FromJsonOverwrite(overwriteConfigJson, buildConfig);
            }

            Dictionary<string, string> commandLine = new Dictionary<string, string>();
            try
            {
                buildConfig.OnPreBuild(commandLine, ref buildPlayerOptions);
                BuildReport report = UnityEditor.BuildPipeline.BuildPlayer(buildPlayerOptions);
                buildConfig.OnPostBuild(commandLine);

                PrintBuildResult(buildConfig.GetBuildPath(), report);
            }
            catch (Exception e)
            {
                Debug.Log("Error - " + e);
                throw;
            }
            finally
            {
                editorSetting_Backup.Restore();
            }
            Debug.Log($"After Build DefineSymbol Current {PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup)}");

            // 2018.4 에서 프로젝트 전체 리임포팅 하는 이슈 대응
            // https://issuetracker.unity3d.com/issues/osx-batchmode-build-hangs-at-refresh-detecting-if-any-assets-need-to-be-imported-or-removed
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }

        /// <summary>
        /// EnvironmentVariable에서 <see cref="ScriptableObject"/>를 얻습니다.
        /// </summary>
        /// <param name="commandLine">찾을 값</param>
        /// <param name="outFile">성공시 얻는 so</param>
        /// <typeparam name="T">scriptable object type</typeparam>
        /// <returns>성공 유무</returns>
        public static bool TryGetSO_FromCommandLine<T>(string commandLine, out T outFile)
            where T : ScriptableObject
        {
            outFile = null;
            string path = Environment.GetEnvironmentVariable(commandLine);
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError($"environment variable {commandLine} is null or empty");
                return false;
            }

            outFile = AssetDatabase.LoadAssetAtPath<T>(path);
            return outFile != null;
        }

        // ==============================================================================================

        #region private

        private static BuildPlayerOptions Generate_BuildPlayerOption(BuildConfigBase config)
        {
            List<string> sceneNames = new List<string>(config.GetBuildSceneNames());
            for (int i = 0; i < sceneNames.Count; i++)
            {
                const string sceneExtension = ".unity";
                string sceneName = sceneNames[i];
                if (sceneName.EndsWith(sceneExtension))
                {
                    sceneNames[i] = sceneName + sceneExtension;
                }
            }

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = sceneNames.ToArray(),
                locationPathName = config.GetBuildPath(),
                target = config.buildTarget,
                options = BuildOptions.None
            };

            return buildPlayerOptions;
        }

        private static PlayerSetting_Backup SettingBuildConfig_To_EditorSetting(BuildConfigBase config, BuildTargetGroup buildTargetGroup)
        {
            string defineSymbol_Backup = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, config.GetDefineSymbol());

            string productName_Backup = PlayerSettings.productName;
            PlayerSettings.productName = config.GetProductName();

            return new PlayerSetting_Backup(buildTargetGroup, defineSymbol_Backup, productName_Backup);
        }

        private static void PrintBuildResult(string path, BuildReport report)
        {
            BuildSummary summary = report.summary;
            Debug.Log($"Build Result:{summary.result}, Path:{path}");

            if (summary.result == BuildResult.Failed)
            {
                int errorIndex = 1;
                foreach (var step in report.steps)
                {
                    foreach (var msg in step.messages)
                    {
                        if (msg.type == LogType.Error || msg.type == LogType.Exception)
                        {
                            Debug.LogFormat("Build Fail Log[{0}] : type : {1}\n" +
                                            "content : {2}", ++errorIndex, msg.type, msg.content);
                        }
                    }
                }
            }
        }

        #endregion private
    }
}
