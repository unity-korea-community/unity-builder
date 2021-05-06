#region Header

/*	============================================
 *	개요 : 
 *	에디터 폴더에 있어야 정상 동작합니다.
 *	
 *	참고한 원본 코드 링크
 *	https://slway000.tistory.com/74
 *	https://smilejsu.tistory.com/1528
   ============================================ */

#endregion Header

using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;

namespace Unity_CMD
{
    public partial class CMDBuilder
    {
        [MenuItem(const_prefix_EditorContextMenu + "Build Test - Android")]
        public static void Build_Test_Android()
        {
            BuildConfig config = BuildConfig.CreateConfig();
            BuildTargetGroup buildTargetGroup = GetBuildTargetGroup(BuildTarget.Android);
            config.definesymbol = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

            Build(config, config.buildoutputFolder_absolutepath, config.filename, BuildTarget.Android);
        }

        // ==============================================================================================


        /// <summary>
        /// 안드로이드 세팅
        /// </summary>
        private static void BuildSetting_Android(BuildConfig.AndroidSetting setting)
        {
            if (string.IsNullOrEmpty(setting.fullpackagename) == false)
                PlayerSettings.applicationIdentifier = setting.fullpackagename;

            PlayerSettings.Android.keyaliasName = setting.keyalias_name;
            PlayerSettings.Android.keyaliasPass = setting.keyalias_password;

            PlayerSettings.Android.keystoreName = Application.dataPath + setting.keystore_relativePath;
            PlayerSettings.Android.keystorePass = setting.keystore_password;

            string bundleVersionCode_FromCommandLine = GetCommandLineArg(commandLine[ECommandLineList.android_bundle_versioncode]);
            if (int.TryParse(bundleVersionCode_FromCommandLine, out int iBundleVersionCode))
                PlayerSettings.Android.bundleVersionCode = iBundleVersionCode;
            else
                PlayerSettings.Android.bundleVersionCode = setting.bundleversioncode;

            string versionCode_FromCommandLine = GetCommandLineArg(commandLine[ECommandLineList.android_version]);
            if (string.IsNullOrEmpty(versionCode_FromCommandLine) == false)
                PlayerSettings.bundleVersion = versionCode_FromCommandLine;
            else
                PlayerSettings.bundleVersion = setting.version;

            if (setting.usecppbuild)
                PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            else
                PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.Mono2x);

            Debug.LogFormat(const_prefix_ForLog + " Build Setting [Android]\n" +
                            "strPackageName : {0}\n" +
                            "keyaliasName : {1}, keyaliasPass : {2}\n" +
                            "keystoreName : {3}, keystorePass : {4}\n" +
                            "bUse_IL_TO_CPP_Build : {5}",
                PlayerSettings.applicationIdentifier,
                PlayerSettings.Android.keyaliasName, PlayerSettings.Android.keyaliasPass,
                PlayerSettings.Android.keystoreName, PlayerSettings.Android.keystorePass,
                setting.usecppbuild);
            ;
        }
    }
}

#endif
