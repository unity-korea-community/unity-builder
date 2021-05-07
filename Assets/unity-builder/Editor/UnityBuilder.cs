using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace Unity_Builder
{
    /// <summary>
    /// 빌드 전 세팅 (빌드 후 되돌리기용)
    /// </summary>
    public class PlayerSetting_Backup
    {
        public string defineSymbol { get; private set; }
        public string productName { get; private set; }

        public BuildTargetGroup buildTargetGroup { get; private set; }

        public PlayerSetting_Backup(BuildTargetGroup buildTargetGroup, string defineSymbol, string productName)
        {
            this.buildTargetGroup = buildTargetGroup;
            this.defineSymbol = defineSymbol;
            this.productName = productName;
        }

        public void Restore()
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, defineSymbol);
            PlayerSettings.productName = productName;
        }
    }

    public class UnityBuilder
    {
        public static void Build()
        {
            if (GetSO_FromCommandLine("configpath", out BuildConfigBase config))
            {
                Build(config);
            }
            else
            {
                Debug.LogError("require -configpath");
            }
        }

        public static void Build(BuildConfigBase buildConfig)
        {
            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildConfig.buildTarget);
            BuildPlayerOptions buildPlayerOptions = Generate_BuildPlayerOption(buildConfig);
            PlayerSetting_Backup editorSetting_Backup = SettingBuildConfig_To_EditorSetting(buildConfig, buildTargetGroup);

            Dictionary<string, string> commandLine = new Dictionary<string, string>();
            try
            {
                buildConfig.OnPreBuild(commandLine);
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
            Debug.LogFormat("After Build DefineSymbol Current {0}", PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup));

            // 2018.4 에서 프로젝트 전체 리임포팅 하는 이슈 대응
            // https://issuetracker.unity3d.com/issues/osx-batchmode-build-hangs-at-refresh-detecting-if-any-assets-need-to-be-imported-or-removed
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }

        public static bool GetSO_FromCommandLine<T>(string commandLine, out T outFile)
            where T : ScriptableObject
        {
            string path = Environment.GetEnvironmentVariable(commandLine);
            outFile = AssetDatabase.LoadAssetAtPath<T>(path);

            return outFile != null;
        }

        // ==============================================================================================

        #region private

        private static BuildPlayerOptions Generate_BuildPlayerOption(BuildConfigBase buildConfig)
        {
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = buildConfig.buildSceneNames.Select(p => p += ".unity").ToArray(),
                locationPathName = buildConfig.GetBuildPath(),
                target = buildConfig.buildTarget,
                options = BuildOptions.None
            };

            return buildPlayerOptions;
        }


        private static PlayerSetting_Backup SettingBuildConfig_To_EditorSetting(BuildConfigBase buildConfig, BuildTargetGroup buildTargetGroup)
        {
            string defineSymbol_Backup = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, buildConfig.defineSymbol);

            string productName_Backup = PlayerSettings.productName;
            PlayerSettings.productName = buildConfig.productName;

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

