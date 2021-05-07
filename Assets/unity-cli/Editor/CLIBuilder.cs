using UnityEngine;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace Unity_CLI
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

    /// <summary>
    /// CLI 빌드를 위한 스크립트입니다.
    /// </summary>
    public partial class CLIBuilder
    {
        private static BuildConfigBase s_lastConfig;

        #region public

        public static string GetCommandLineArg(string name)
        {
            string[] arguments = Environment.GetCommandLineArgs();
            for (int i = 0; i < arguments.Length; ++i)
            {
                if (arguments[i] == name && arguments.Length > i + 1)
                    return arguments[i + 1];
            }

            return "";
        }

        public static bool GetSOFromCommandLine<T>(string commandLine, out T outFile)
            where T : ScriptableObject
        {
            string path = GetCommandLineArg(commandLine);
            Exception exception = DoTryParsing_JsonSOFile(path, out outFile);
            if (exception != null)
            {
                Debug.LogErrorFormat("Error - FilePath : {0}, FilePath : {1}\n" +
                                "Error : {2}", commandLine, path, exception);
                return false;
            }

            return true;
        }

        public static Exception DoTryParsing_JsonSOFile<T>(string jsonFilePath, out T outFile)
            where T : ScriptableObject
        {
            outFile = ScriptableObject.CreateInstance<T>();

            try
            {
                string configJson = File.ReadAllText(jsonFilePath);
                JsonUtility.FromJsonOverwrite(configJson, outFile);
            }
            catch (Exception exception)
            {
                outFile = null;
                return exception;
            }

            return null;
        }

        public static string[] GetEnabled_EditorScenes()
        {
            return EditorBuildSettings.scenes.
                Where(p => p.enabled).
                Select(p => p.path.Replace(".unity", "")).
                ToArray();
        }


        public static void Build(BuildConfigBase buildConfig)
        {
            s_lastConfig = buildConfig;

            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildConfig.buildTarget);
            BuildPlayerOptions buildPlayerOptions = Generate_BuildPlayerOption(buildConfig);
            PlayerSetting_Backup editorSetting_Backup = SettingBuildConfig_To_EditorSetting(buildConfig, buildTargetGroup);

            try
            {
                BuildReport report = UnityEditor.BuildPipeline.BuildPlayer(buildPlayerOptions);
                PrintBuildResult(buildConfig.buildPath, report, report.summary);
            }
            catch (Exception e)
            {
                Debug.Log("Error - " + e);
                throw;
            }

            editorSetting_Backup.Restore();

            Debug.LogFormat("After Build DefineSymbol Current {0}", PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup));

            // 2018.4 에서 프로젝트 전체 리임포팅 하는 이슈 대응
            // https://issuetracker.unity3d.com/issues/osx-batchmode-build-hangs-at-refresh-detecting-if-any-assets-need-to-be-imported-or-removed
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }

        #endregion public

        // ==============================================================================================


        #region private
        private static BuildPlayerOptions Generate_BuildPlayerOption(BuildConfigBase buildConfig)
        {
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = buildConfig.buildSceneNames.Select(p => p += ".unity").ToArray(),
                locationPathName = buildConfig.buildPath,
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

        private static void PrintBuildResult(string path, BuildReport report, BuildSummary summary)
        {
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

