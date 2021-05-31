using System.Runtime.InteropServices;
using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace UNKO.Unity_Builder
{
    [CreateAssetMenu(fileName = "WindowBuildConfig", menuName = GlobalConst.CreateAssetMenu_Prefix + "/WindowBuildConfig")]
    public class WindowBuildConfig : BuildConfig
    {
        public override BuildTarget buildTarget => BuildTarget.StandaloneWindows64;

        /// <summary>
        /// CPP 빌드를 할지 체크, CPP빌드는 오래 걸리므로 Test빌드가 아닌 Alpha 빌드부터 하는걸 권장
        /// </summary>
        public ScriptingImplementation scriptingBackEnd;

        public override void ResetSetting(BuildConfig config)
        {
            base.ResetSetting(config);

            BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
            scriptingBackEnd = PlayerSettings.GetScriptingBackend(targetGroup);
            config.buildPath +=
                     "\n_{bundleVersion}.{bundleVersionCode}" +
                     "\n_{scriptingBackEnd}";
        }

        public override void OnPreBuild(IDictionary<string, string> commandLine, ref BuildPlayerOptions buildPlayerOptions)
        {
            base.OnPreBuild(commandLine, ref buildPlayerOptions);

            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, scriptingBackEnd);
        }

        public override string GetBuildPath()
        {
            return base.GetBuildPath()
                .Replace("{scriptingBackEnd}", scriptingBackEnd.ToString())
                + ".apk";
        }
    }

    [CustomEditor(typeof(WindowBuildConfig))]
    public class WindowBuildConfig_Inspector : Editor
    {
        string _commandLine;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            AndroidBuildConfig config = target as AndroidBuildConfig;
            if (GUILayout.Button("Reset to Current EditorSetting"))
            {
                config.ResetSetting(config);
            }

            if (GUILayout.Button("Build!"))
            {
                UnityBuilder.Build(config);
            }

            _commandLine = EditorGUILayout.TextField("commandLine", _commandLine);
            if (GUILayout.Button($"Build with \'{_commandLine}\'"))
            {
                string[] commands = _commandLine.Split(' ');
                for (int i = 0; i < commands.Length; i++)
                {
                    string command = commands[i];
                    bool hasNextCommand = i + 1 < commands.Length;
                    if (command.StartsWith("-"))
                    {
                        if (hasNextCommand)
                        {
                            Environment.SetEnvironmentVariable(command, commands[i + 1]);
                        }
                        else
                        {
                            Environment.SetEnvironmentVariable(command, "");
                        }
                    }
                }

                UnityBuilder.Build();
            }
        }
    }
}
