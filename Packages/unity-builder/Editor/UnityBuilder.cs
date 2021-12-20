using System;
using System.Collections.Generic;
using System.Linq;
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


        static string _commandLine = string.Empty;

        public static void AddCommandLine(string commandLine)
        {
            _commandLine = string.Empty;
            _commandLine += commandLine;
        }

        public static string GetCommandLine()
        {
            return Environment.CommandLine + _commandLine;
        }

        public static string[] GetCommandLineArgs()
        {
            List<string> commandargs = new List<string>(Environment.GetCommandLineArgs());

            if (string.IsNullOrEmpty(_commandLine) == false)
            {
                commandargs.AddRange(_commandLine.Replace("\r\n", "\n").Split(' ', '\n'));
            }

            return commandargs.ToArray();
        }

        private static string GetCommandLineValue(string commandLineArg)
        {
            string value = Environment.GetEnvironmentVariable(commandLineArg);
            if (string.IsNullOrEmpty(value) == false)
            {
                return value;
            }

            if (commandLineArg.StartsWith("-") == false)
            {
                commandLineArg = $"-{commandLineArg}";
            }

            string[] commandLines = GetCommandLineArgs();
            for (int i = 0; i < commandLines.Length; i++)
            {
                string command = commandLines[i];
                if (command.Equals(commandLineArg))
                {
                    value = commandLines[i + 1];
                    break;
                }
            }

            if (string.IsNullOrEmpty(value))
            {
                Debug.Log($"environment variable {commandLineArg} is null or empty");
            }

            return value;
        }

        /// <summary>
        /// 커맨드라인의 configpath에서 <see cref="BuildConfigBase"/>를 얻은 뒤 해당 config로 빌드합니다.
        /// </summary>
        public static void Build()
        {
            Debug.Log($"Build with commandLine: {GetCommandLine()}");

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
            string overwriteConfigJson = GetCommandLineValue("overwrite");
            if (string.IsNullOrEmpty(overwriteConfigJson) == false)
            {
                Debug.Log($"overwrite config: {overwriteConfigJson}");
                JsonUtility.FromJsonOverwrite(overwriteConfigJson, buildConfig);
            }
            else
            {
                Debug.Log($"not overwrite config");
            }

            if (buildConfig.overrideEditorSettingBundleVersion)
            {
                buildConfig.bundleVersion = PlayerSettings.bundleVersion;
            }

            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildConfig.buildTarget);
            BuildPlayerOptions buildPlayerOptions = Generate_BuildPlayerOption(buildConfig);
            PlayerSetting_Backup editorSetting_Backup = SettingBuildConfig_To_EditorSetting(buildConfig, buildTargetGroup);
            Debug.Log($"OnPreBuild DefineSymbol {PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup)}");

            Dictionary<string, string> commandLine = new Dictionary<string, string>();
            try
            {
                buildConfig.OnPreBuild(commandLine, ref buildPlayerOptions);
                BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
                buildConfig.OnPostBuild(commandLine);

                string path = buildConfig.GetBuildPath();
                path = path.Replace(".", "_");
                PrintBuildResult(path, report);
            }
            finally
            {
                editorSetting_Backup.Restore();
            }
            Debug.Log($"OnAfterBuild DefineSymbol {PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup)}");

            // 2018.4 에서 프로젝트 전체 리임포팅 하는 이슈 대응
            // https://issuetracker.unity3d.com/issues/osx-batchmode-build-hangs-at-refresh-detecting-if-any-assets-need-to-be-imported-or-removed
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }

        /// <summary>
        /// CommandLine에서 <see cref="ScriptableObject"/>를 얻습니다.
        /// </summary>
        /// <param name="commandLineArg">찾을 값</param>
        /// <param name="outFile">성공시 얻는 so</param>
        /// <typeparam name="T">scriptable object type</typeparam>
        /// <returns>성공 유무</returns>
        public static bool TryGetSO_FromCommandLine<T>(string commandLineArg, out T outFile)
            where T : ScriptableObject
        {
            outFile = null;

            string path = GetCommandLineValue(commandLineArg);
            outFile = AssetDatabase.LoadAssetAtPath<T>(path);
            return outFile != null;
        }

        // ==============================================================================================

        #region private

        private static BuildPlayerOptions Generate_BuildPlayerOption(BuildConfigBase config)
        {
            const string sceneExtension = ".unity";
            string dataPath = $"{Application.dataPath}/";
            string[] buildSettingScenes;
            if (config.useScenesInEditorSetting)
            {
                buildSettingScenes = EditorBuildSettings.scenes
                    .Where(scene => scene.enabled)
                    .Select(scene => scene.path)
                    .ToArray();
            }
            else
            {
                List<string> sceneNames = new List<string>(config.GetBuildSceneNames());
                for (int i = 0; i < sceneNames.Count; i++)
                {
                    string sceneName = sceneNames[i];
                    if (sceneName.StartsWith("Assets/"))
                    {
                        sceneName = sceneName.Replace("Assets/", "");
                    }

                    if (sceneName.StartsWith(dataPath) == false)
                    {
                        sceneName = dataPath + sceneName;
                    }

                    if (sceneName.EndsWith(sceneExtension) == false)
                    {
                        sceneName += sceneExtension;
                    }

                    sceneNames[i] = sceneName;
                }

                buildSettingScenes = sceneNames.ToArray();
            }

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = buildSettingScenes,
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

            if (summary.result == BuildResult.Failed || summary.result == BuildResult.Unknown)
            {
                int errorIndex = 1;
                foreach (var step in report.steps)
                {
                    foreach (var msg in step.messages)
                    {
                        if (msg.type == LogType.Error || msg.type == LogType.Exception)
                        {
                            Debug.LogErrorFormat("Build Fail Log[{0}] : type : {1}\n" +
                                            "content : {2}", ++errorIndex, msg.type, msg.content);
                        }
                    }
                }

                throw new System.Exception();
            }
        }


        #endregion private
    }
}
