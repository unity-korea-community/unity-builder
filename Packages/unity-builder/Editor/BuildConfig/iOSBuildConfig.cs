using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

namespace UNKO.Unity_Builder
{
    /// <summary>
    /// 유니티 -> XCode Export -> XCode ArchiveBuildConfig -> .ipa 에 필요한 모든 설정
    /// </summary>
    [CreateAssetMenu(fileName = "iOSBuildConfig", menuName = GlobalConst.CreateAssetMenu_Prefix + "/iOSBuildConfig")]
    [StructLayout(LayoutKind.Auto)] // ignore codacy
    public class iOSBuildConfig : BuildConfigBase
    {
        private const string entitlements = @"
         <?xml version=""1.0"" encoding=""UTF-8\""?>
         <!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
         <plist version=""1.0"">
             <dict>
                 <key>aps-environment</key>
                 <string>production</string>
             </dict>
         </plist>";

        const string UNITY_IPHONE = "Unity-iPhone";

        [PostProcessBuild(999)]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
        {
            bool platformIsiOS = buildTarget == BuildTarget.iOS;
            if (platformIsiOS)
            {
#if UNITY_IOS
                string projectPath = path + $"/{UNITY_IPHONE}.xcodeproj/project.pbxproj";

                PBXProject pbxProject = new PBXProject();
                pbxProject.ReadFromFile(projectPath);

                string target = pbxProject.GetUnityMainTargetGuid();
                pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "NO");

                pbxProject.WriteToFile(projectPath);

                string infoPlistPath = path + "/Info.plist";

                PlistDocument plistDoc = new PlistDocument();
                plistDoc.ReadFromFile(infoPlistPath);
                if (plistDoc.root != null)
                {
                    plistDoc.root.SetBoolean("ITSAppUsesNonExemptEncryption", false);

                    plistDoc.WriteToFile(infoPlistPath);
                    Debug.Log($"plist edit complete, path:{infoPlistPath}");
                }
                else
                {
                    Debug.LogError($"plist Can't open, path:{infoPlistPath}");
                }

                // https://lhamed.github.io/66th-post-copy-2/
                var fileName = "unity.entitlements"; // 이거없으면 푸시 안됩니다 
                var targetGUID = pbxProject.TargetGuidByName(UNITY_IPHONE);
                var entitlementsFilePath = $"{path}/{UNITY_IPHONE}/{fileName}";
                try
                {
                    File.WriteAllText(entitlementsFilePath, entitlements);
                    pbxProject.AddFile($"{UNITY_IPHONE}/{fileName}", fileName);
                    pbxProject.AddBuildProperty(targetGUID, "CODE_SIGN_ENTITLEMENTS", UNITY_IPHONE + "/" + fileName);
                    pbxProject.WriteToFile(projectPath);
                }
                catch (IOException e)
                {
                    Debug.Log("Could not copy entitlements. Probably already exists. " + e);
                }

                ProjectCapabilityManager pcm = new ProjectCapabilityManager(projectPath, entitlementsFilePath, UNITY_IPHONE);
                pcm.AddPushNotifications(false);
#endif
            }

            Debug.Log($"OnPostProcessBuild - buildTarget:{buildTarget}, path:{path}");
        }

        ///<inheritdoc cref="IBuildConfig.buildTarget"/>
        public override BuildTarget buildTarget => BuildTarget.iOS;

        /// <summary>
        /// 애플 개발자 사이트에서 조회 가능, 숫자랑 영어로 된거
        /// </summary>
        public string appleTeamID;

        public iOSSdkVersion sdkVersion;

        /// <summary>
        /// Apple에서 세팅된 net.Company.Product 형식의 string
        /// </summary>ß
        public string bundle_Identifier;
        public string entitlementsFileName_Without_ExtensionName;

        /// <summary>
        /// 유니티 Asset 경로에서 XCode Project로 카피할 파일 목록, 확장자까지 작성해야 합니다
        /// <para>UnityProject/Assets/ 기준</para>
        /// </summary>
        public string[] copy_AssetFilePath_To_XCodeProjectPath;

        /// <summary>
        /// XCode 프로젝트에 추가할 Framework, 확장자까지 작성해야 합니다
        /// </summary>
        public string[] xcode_Framework_Add;

        /// <summary>
        /// XCode 프로젝트에 제거할 Framework, 확장자까지 작성해야 합니다
        /// </summary>
        public string[] xcode_Framework_Remove;

        public string[] xcode_OTHER_LDFLAGS_Add;
        public string[] xcode_OTHER_LDFLAGS_Remove;

        /// <summary>
        /// HTTP 주소 IOS는 기본적으로 HTTP를 허용 안하기 때문에 예외에 추가해야 합니다. http://는 제외할것
        /// <para>예시) http://www.naver.com = www.naver.com</para>
        /// </summary>
        public string[] HTTPAddress;

        /// <summary>
        /// 빌드 번호
        /// <para>이미 앱스토어에 올렸으면 그 다음 항상 1씩 올려야 합니다. 안그럼 앱스토어에서 안받음</para>
        /// </summary>
        public string buildNumber;

        [Serializable]
        public class PLIST_ADD
        {
            public string strKey;
            public string strValue;

            public PLIST_ADD(string strKey, string strValue)
            {
                this.strKey = strKey;
                this.strValue = strValue;
            }
        }

        public PLIST_ADD[] addPlist = new PLIST_ADD[] { new PLIST_ADD("ExampleKey", "ExampleValue") };
        public string[] removePlistKey = new string[] { "ExampleKey", "ExampleValue" };

        public override void OnPreBuild(IDictionary<string, string> commandLine, ref BuildPlayerOptions buildPlayerOptions)
        {
            base.OnPreBuild(commandLine, ref buildPlayerOptions);

            BuildTargetGroup targetGroup = buildPlayerOptions.targetGroup;

            PlayerSettings.SetApplicationIdentifier(targetGroup, bundle_Identifier);
            PlayerSettings.iOS.appleDeveloperTeamID = appleTeamID;
            PlayerSettings.iOS.buildNumber = buildNumber;
            PlayerSettings.iOS.sdkVersion = sdkVersion;

            Debug.LogFormat($"OnPreBuild [{targetGroup}]\n" +
                            $"bundle_Identifier:{PlayerSettings.GetApplicationIdentifier(targetGroup)}\n" +
                            $"appleDeveloperTeamID:{PlayerSettings.iOS.appleDeveloperTeamID}\n" +
                            $"buildNumber:{PlayerSettings.iOS.buildNumber}\n" +
                            $"sdkVersion:{PlayerSettings.iOS.sdkVersion}");
        }
    }

    /// <summary>
    /// <see cref="iOSBuildConfig"/> 인스펙터
    /// </summary>
    [CustomEditor(typeof(iOSBuildConfig))]
    public class iOSBuildConfig_Inspector : Editor
    {
        string _commandLine;

        ///<inheritdoc cref="Editor.OnInspectorGUI"/>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            iOSBuildConfig config = target as iOSBuildConfig;
            if (GUILayout.Button("Reset to Current EditorSetting"))
            {
                config.ResetSetting(config);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.SetDirty(config);
            }

            if (GUILayout.Button("Build!"))
            {
                UnityBuilder.Build(config);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("or with command line");
            _commandLine = EditorGUILayout.TextArea(_commandLine);
            if (GUILayout.Button($"Build with \'{_commandLine}\'"))
            {
                UnityBuilder.AddCommandLine(_commandLine);
                UnityBuilder.Build();
            }
        }
    }
}