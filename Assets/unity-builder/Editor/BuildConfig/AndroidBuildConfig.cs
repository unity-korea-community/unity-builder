using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Unity_Builder
{
    [CreateAssetMenu(fileName = "AndroidBuildConfig", menuName = GlobalConst.CreateAssetMenu_Prefix + "/AndroidBuildConfig")]
    public class AndroidBuildConfig : BuildConfigBase
    {
        public override BuildTarget buildTarget => BuildTarget.Android;

        public string keyaliasName;
        public string keyaliasPassword;

        /// <summary>
        /// Keystore 파일의 경로입니다. `파일경로/파일명.keystore` 까지 쓰셔야 합니다.
        /// <para>UnityProject/Asset/ 기준의 상대 경로입니다. </para>
        /// <para>예를들어 UnityProject/Asset 폴더 밑에 example.keystore가 있으면 "/example.keystore" 입니다.</para>
        /// </summary>
        public string keystorePath;
        public string keystorePassword;

        /// <summary>
        /// CPP 빌드를 할지 체크, CPP빌드는 오래 걸리므로 Test빌드가 아닌 Alpha 빌드부터 하는걸 권장
        /// 아직 미지원
        /// </summary>
        public ScriptingImplementation scriptingBackEnd;

        public int bundleVersionCode;

        public override void ResetSetting()
        {
            base.ResetSetting();

            BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
            scriptingBackEnd = PlayerSettings.GetScriptingBackend(targetGroup);
            bundleVersionCode = PlayerSettings.Android.bundleVersionCode;
            buildPath +=
                "\n_{bundleVersion}.{bundleVersionCode}" +
                "\n_{scriptingBackEnd}";
        }

        public override void OnPreBuild(IDictionary<string, string> commandLine)
        {
            PlayerSettings.Android.keyaliasName = keyaliasName;
            PlayerSettings.Android.keyaliasPass = keyaliasPassword;

            PlayerSettings.Android.keystoreName = Application.dataPath + keystorePath;
            PlayerSettings.Android.keystorePass = keystorePassword;
            PlayerSettings.Android.bundleVersionCode = bundleVersionCode;

            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, scriptingBackEnd);

            Debug.LogFormat($"OnPreBuild [Android]\n" +
                            $"PackageName : {PlayerSettings.applicationIdentifier}\n" +
                            $"keyaliasName : {PlayerSettings.Android.keyaliasName}, keyaliasPass : {PlayerSettings.Android.keyaliasPass}\n" +
                            $"keystoreName : {PlayerSettings.Android.keystoreName}, keystorePass : {PlayerSettings.Android.keystorePass}\n");
        }

        public override void OnPostBuild(IDictionary<string, string> commandLine)
        {
        }

        public override string GetBuildPath()
        {
            return base.GetBuildPath()
                .Replace("{scriptingBackEnd}", scriptingBackEnd.ToString())
                .Replace("{bundleVersionCode}", bundleVersionCode.ToString())

                + ".apk";
        }
    }

    [CustomEditor(typeof(AndroidBuildConfig))]
    public class AndroidBuildConfig_Inspector : Editor
    {
        string _commandLine;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            AndroidBuildConfig config = target as AndroidBuildConfig;
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
                            Environment.SetEnvironmentVariable(command, commands[i + 1]);
                        else
                            Environment.SetEnvironmentVariable(command, "");
                    }
                }

                UnityBuilder.Build();
            }
        }
    }
}
