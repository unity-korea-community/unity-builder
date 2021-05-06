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
using System.IO;
using System.Linq;

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace Unity_CMD
{
    /// <summary>
    /// 에디터 플레이어 빌드 전 세팅 (빌드 후 되돌리기용)
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
    /// CMD 빌드를 위한 스크립트입니다.
    /// </summary>
    public partial class CMDBuilder
    {
        public const string const_prefix_EditorContextMenu = "Tools/Strix/CMD Build/";
        const string const_prefix_ForLog = "!@#$";

        public enum ECommandLine
        {
            /// <summary>
            /// 결과물이 나오는 파일 명
            /// </summary>
            filename,

            /// <summary>
            /// 컨피그 파일이 있는 절대 경로
            /// </summary>
            config_path,

            /// <summary>
            /// 결과물이 나오는 절대 경로
            /// </summary>
            output_path,

            /// <summary>
            /// PlayerSetting.android.BundleVersionCode
            /// </summary>
            android_bundle_versioncode,

            /// <summary>
            /// PlayerSetting.android.Version
            /// </summary>
            android_version,

            /// <summary>
            /// 스토어에 올라가는 빌드 번호
            /// </summary>
            ios_buildnumber,

            /// <summary>
            /// 스토어에 올라가는 버전
            /// </summary>
            ios_version,
        }

        static readonly IReadOnlyDictionary<ECommandLine, string> commandLine =
            new Dictionary<ECommandLine, string>()
            {
                {ECommandLine.filename, $"{nameof(ECommandLine.filename)}"},
                {ECommandLine.config_path, $"{nameof(ECommandLine.config_path)}"},
                {ECommandLine.output_path, $"{nameof(ECommandLine.output_path)}"},
                {ECommandLine.android_bundle_versioncode, $"{nameof(ECommandLine.android_bundle_versioncode)}"},
                {ECommandLine.android_version, $"{nameof(ECommandLine.android_version)}"},
                {ECommandLine.ios_buildnumber, $"{nameof(ECommandLine.ios_buildnumber)}"},
                {ECommandLine.ios_version, $"{nameof(ECommandLine.ios_version)}"}
            };

        public static string GetCommandLineString(ECommandLine commandLine) => CMDBuilder.commandLine[commandLine];

        private static BuildConfig s_lastConfig;


        [MenuItem(const_prefix_EditorContextMenu + "Create BuildConfig Example Json File")]
        public static void CreateBuildConfig_DummyJson()
        {
            string content = JsonUtility.ToJson(BuildConfig.CreateConfig(), true);
            string filePath = nameof(BuildConfig) + "_Example.json";
            File.WriteAllText(Application.dataPath + "/" + filePath, content);

            AssetDatabase.Refresh();
            UnityEngine.Object jsonFile = AssetDatabase.LoadMainAssetAtPath($"Assets/{filePath}");

            Selection.activeObject = jsonFile;
            Debug.Log("Create BuildConfig Done", jsonFile);
        }

        // SO는 아직 고려중
        //[MenuItem(const_strPrefix_EditorContextMenu + "Create BuildConfig Example SO File")]
        //public static void Create_BuildConfig_Dummy_SO()
        //{
        //    BuildConfig pConfig = BuildConfig.CreateConfig();
        //    AssetDatabase.CreateAsset(pConfig, $"Assets/{nameof(BuildConfig)} _Example.asset");
        //    AssetDatabase.SaveAssets();
        //    AssetDatabase.Refresh();

        //    Selection.activeObject = pConfig;
        //    Debug.Log("Create BuildConfig Done", pConfig);
        //}

        #region public

        public static string GetCommandLineArg(string name)
        {
            string[] arguments = Environment.GetCommandLineArgs();
            for (int i = 0; i < arguments.Length; ++i)
            {
                if (arguments[i] == name && arguments.Length > i + 1)
                    return arguments[i + 1];
            }

            return null;
        }

        public static bool GetFileFromCommandLine<T>(string commandLine, out T outFile)
            where T : new()
        {
            string path = GetCommandLineArg(commandLine);
            Exception exception = DoTryParsing_JsonFile(path, out outFile);
            if (exception != null)
            {
                Debug.LogErrorFormat(const_prefix_ForLog + " Error - FilePath : {0}, FilePath : {1}\n" +
                                     " Error : {2}", commandLine, path, exception);
                return false;
            }

            return true;
        }

        public static bool GetFileFromCommandLineSO<T>(string commandLine, out T outFile)
            where T : ScriptableObject
        {
            string path = GetCommandLineArg(commandLine);
            Exception exception = DoTryParsing_JsonFileSO(path, out outFile);
            if (exception != null)
            {
                Debug.LogErrorFormat(const_prefix_ForLog + " Error - FilePath : {0}, FilePath : {1}\n" +
                                " Error : {2}", commandLine, path, exception);
                return false;
            }

            return true;
        }

        public static void GetAppFilePath_FromConfig(BuildConfig config, out string buildOutputFolderPath,
            out string fileName)
        {
            string buildOutputFolderPath_CommandLine = GetCommandLineArg(commandLine[ECommandLine.output_path]);
            string fileName_CommandLine = GetCommandLineArg(commandLine[ECommandLine.filename]);

            buildOutputFolderPath = string.IsNullOrEmpty(buildOutputFolderPath_CommandLine)
                ? config.buildoutputFolder_absolutepath
                : buildOutputFolderPath_CommandLine;

            if (string.IsNullOrEmpty(fileName_CommandLine))
            {
                fileName = config.filename;
            }
            else
            {
                fileName = fileName_CommandLine;
                config.usedatetimesuffix = false;
            }
        }


        public static Exception DoTryParsing_JsonFile<T>(string jsonFilePath, out T outFile)
            where T : new()
        {
            outFile = default(T);

            try
            {
                string configJson = File.ReadAllText(jsonFilePath);
                outFile = JsonUtility.FromJson<T>(configJson);
            }
            catch (Exception exception)
            {
                return exception;
            }

            return null;
        }

        public static Exception DoTryParsing_JsonFileSO<T>(string jsonFilePath, out T outFile)
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


        public static void Build(BuildConfig buildConfig, string buildOutputFolderAbsolutePath,
            string fileName, BuildTarget buildTarget)
        {
            s_lastConfig = buildConfig;

            OnPreBuild(buildConfig, buildOutputFolderAbsolutePath, fileName, buildTarget, out var buildTargetGroup, out var buildPath);

            BuildPlayerOptions buildPlayerOptions = Generate_BuildPlayerOption(buildConfig, buildTarget, buildPath);
            PlayerSetting_Backup editorSetting_Backup = SettingBuildConfig_To_EditorSetting(buildConfig, buildTargetGroup);

            Debug.LogFormat(const_prefix_ForLog + " Before Build DefineSymbol TargetGroup : {0}\n" +
                            "Origin Symbol : {1}\n " +
                            "Config : {2} \n" +
                            "Current : {3} \n" +
                            "strBuildPath : {4}",
                buildTargetGroup,
                editorSetting_Backup.defineSymbol,
                buildConfig.definesymbol,
                PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup),
                buildPath);

            try
            {
                BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
                PrintBuildResult(buildPath, report, report.summary);
            }
            catch (Exception e)
            {
                Debug.Log(const_prefix_ForLog + " Error - " + e);
                throw;
            }

            editorSetting_Backup.Restore();
            OnPostBuild(buildTarget, buildOutputFolderAbsolutePath);

            Debug.LogFormat(const_prefix_ForLog + " After Build DefineSymbol Current {0}",
                PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup));

            // 2018.4 에서 프로젝트 전체 리임포팅 하는 이슈 대응
            // https://issuetracker.unity3d.com/issues/osx-batchmode-build-hangs-at-refresh-detecting-if-any-assets-need-to-be-imported-or-removed
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }

        /// <summary>
        /// Android를 빌드합니다. CommandLine - ConfigPath 필요
        /// <para>Command Line으로 실행할 때 partial class file에 있으면 못찾음..</para>
        /// </summary>
        public static void Build_Android()
        {
            if (GetFileFromCommandLineSO(commandLine[ECommandLine.config_path], out BuildConfig config))
            {
                GetAppFilePath_FromConfig(config, out string buildOutputFolderPath, out string fileName);
                Build(config, buildOutputFolderPath, fileName, BuildTarget.Android);
            }
        }

        /// <summary>
        /// IOS를 빌드합니다. CommandLine - ConfigPath 필요
        /// <para>Command Line으로 실행할 때 partial class file에 있으면 못찾음..</para>
        /// </summary>
        public static void Build_IOS()
        {
            if (GetFileFromCommandLineSO(commandLine[ECommandLine.config_path], out BuildConfig config))
            {
                GetAppFilePath_FromConfig(config, out string buildOutputFolderPath, out string fileName);
                Build(config, buildOutputFolderPath, fileName, BuildTarget.iOS);
            }
        }

        #endregion public

        // ==============================================================================================


        #region private
        private static BuildPlayerOptions Generate_BuildPlayerOption(BuildConfig buildConfig, BuildTarget buildTarget,
            string buildPath)
        {
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = buildConfig.buildscenenames.Select(p => p += ".unity").ToArray(),
                locationPathName = buildPath,
                target = buildTarget,
                options = BuildOptions.None
            };

            return buildPlayerOptions;
        }


        private static PlayerSetting_Backup SettingBuildConfig_To_EditorSetting(BuildConfig buildConfig,
            BuildTargetGroup buildTargetGroup)
        {
            string defineSymbol_Backup = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, buildConfig.definesymbol);

            string productName_Backup = PlayerSettings.productName;
            PlayerSettings.productName = buildConfig.productname;

            return new PlayerSetting_Backup(buildTargetGroup, defineSymbol_Backup, productName_Backup);
        }

        private static void OnPreBuild(BuildConfig buildConfig, string buildOutputFolderAbsolutePath,
            string fileName, BuildTarget buildTarget, out BuildTargetGroup buildTargetGroup,
            out string buildPath)
        {
            buildTargetGroup = GetBuildTargetGroup(buildTarget);
            buildPath = CreateBuildPath(buildConfig.usedatetimesuffix, buildOutputFolderAbsolutePath,
                fileName);

            switch (buildTarget)
            {
                case BuildTarget.Android:
                    BuildSetting_Android(buildConfig.androidSetting);
                    buildPath += ".apk";
                    break;

                case BuildTarget.iOS:
                    BuildSetting_IOS(buildConfig.iOSSetting_);
                    break;
            }
        }

        private static string CreateBuildPath(bool useDateTimeSuffix, string folderName, string fileName)
        {
            Debug.LogFormat(const_prefix_ForLog + " FolderName : {0}, FileName : {1}", folderName,
                fileName);

            try
            {
                if (Directory.Exists(folderName) == false)
                    Directory.CreateDirectory(folderName);
            }
            catch (Exception e)
            {
                Debug.Log(const_prefix_ForLog + " Error - Create Directory - " + e);
            }

            string buildPath = folderName + "/" + fileName;
            if (useDateTimeSuffix)
            {
                string strDateTime = DateTime.Now.ToString("MMdd_HHmm");
                buildPath = buildPath + "_" + strDateTime;
            }

            return buildPath;
        }


        private static void OnPostBuild(BuildTarget buildTarget, string buildOutputFolderAbsolutePath)
        {
            switch (buildTarget)
            {
                case BuildTarget.Android:

                    try
                    {
                        // Mac OS에서 구동 시 Directory.GetFiles함수는 Error가 나기 때문에
                        // DirectoryInfo.GetFiles를 통해 체크
                        DirectoryInfo directory = new DirectoryInfo(buildOutputFolderAbsolutePath);
                        foreach (var file in directory.GetFiles())
                        {
                            // IL2CPP 파일로 빌드 시 자동으로 생inf기는 파일, 삭제해도 무방
                            if (file.Extension == ".zip" && file.Name.Contains("symbols"))
                            {
                                Debug.Log(const_prefix_ForLog + " Delete : " + file.Name);
                                file.Delete();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log(const_prefix_ForLog + " Error - " + e);
                    }

                    break;
            }
        }


        private static void PrintBuildResult(string path, BuildReport report, BuildSummary summary)
        {
            Debug.LogFormat(const_prefix_ForLog + " Path : {0}, Build Result : {1}", path, summary.result);

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log(const_prefix_ForLog + " Build Succeeded!");
            }
            else if (summary.result == BuildResult.Failed)
            {
                int errorIndex = 1;
                foreach (var step in report.steps)
                {
                    foreach (var msg in step.messages)
                    {
                        if (msg.type == LogType.Error || msg.type == LogType.Exception)
                        {
                            Debug.LogFormat(const_prefix_ForLog + " Build Fail Log[{0}] : type : {1}\n" +
                                            " content : {2}", ++errorIndex, msg.type, msg.content);
                        }
                    }
                }
            }
        }

        private static BuildTargetGroup GetBuildTargetGroup(BuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                case BuildTarget.Android: return BuildTargetGroup.Android;
                case BuildTarget.iOS: return BuildTargetGroup.iOS;
            }

            return BuildTargetGroup.Standalone;
        }

        #endregion private
    }
}

#endif
