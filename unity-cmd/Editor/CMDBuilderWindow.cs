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

namespace CMD
{
    /// <summary>
    /// <see cref="CMDBuilder"/>>를 유니티 에디터에서 제어하는 스크립트입니다.
    /// </summary>
    public class CMDBuilderWindow : EditorWindow
    {
        private const string gitHubURL = "https://github.com/KorStrix/Unity_JenkinsBuilder";
        BuildConfig _buildConfig;

        string _configPath;
        string _buildPath;

        [MenuItem(CMDBuilder.const_prefix_EditorContextMenu + "Show Builder Window", priority = -10000)]
        public static void DoShow_CMDBuilder()
        {
            CMDBuilderWindow window = (CMDBuilderWindow)GetWindow(typeof(CMDBuilderWindow), false);

            window.minSize = new Vector2(600, 200);
            window.Show();
        }

        private void OnEnable()
        {
            _configPath = EditorPrefs.GetString($"{nameof(CMDBuilderWindow)}_{nameof(_configPath)}");
            _buildPath = EditorPrefs.GetString($"{nameof(CMDBuilderWindow)}_{nameof(_buildPath)}");

            CheckConfigPath();
        }

        private void OnGUI()
        {
            GUILayout.Space(10f);

            EditorGUILayout.HelpBox("이 툴은 BuildConfig를 통해 빌드하는 툴입니다.\n\n" +
                                    "사용방법\n" +
                                    "1. Edit Config Json File을 클릭하여 세팅합니다.\n" +
                                    "2. Edit Build Output Path를 눌러 빌드파일 출력 경로를 세팅합니다.\n" +
                                    "3. 플랫폼(Android or iOS) 빌드를 누릅니다.\n" +
                                    "4. 빌드가 되는지 확인 후, 빌드가 완료되면 Open BuildFolder를 눌러 빌드 파일을 확인합니다."
                , MessageType.Info);


            if (GUILayout.Button("Github"))
                System.Diagnostics.Process.Start(gitHubURL);
            GUILayout.Space(30f);


            EditorGUI.BeginChangeCheck();
            DrawPath_File("Config Json File", ref _configPath,
                (strPath) => EditorPrefs.SetString($"{nameof(CMDBuilderWindow)}_{nameof(_configPath)}", strPath));
            if (EditorGUI.EndChangeCheck())
            {
                CheckConfigPath();
            }

            DrawPath_Folder("Build", ref _buildPath,
                (strPath) => EditorPrefs.SetString($"{nameof(CMDBuilderWindow)}_{nameof(_buildPath)}", strPath));
            GUILayout.Space(10f);

            bool bConfigIsNotNull = _buildConfig != null;

            GUI.enabled = bConfigIsNotNull;
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Android Build!", GUILayout.Height(40f)))
                {
                    CMDBuilder.Build(_buildConfig, _buildPath, _buildConfig.filename, BuildTarget.Android);
                }

                if (GUILayout.Button("iOS Build!", GUILayout.Height(40f)))
                {
                    CMDBuilder.Build(_buildConfig, _buildPath, _buildConfig.filename, BuildTarget.iOS);
                }
            }
            GUILayout.EndHorizontal();
            GUI.enabled = true;

            if (GUILayout.Button("Open BuildFolder Path !"))
            {
                System.Diagnostics.Process.Start(_buildPath);
            }
        }

        private string DrawPath_Folder(string explainName, ref string folderPath, Action<string> OnChangePath)
        {
            return DrawPath(explainName, ref folderPath, OnChangePath, true);
        }

        private string DrawPath_File(string explainName, ref string filePath, Action<string> OnChangePath)
        {
            return DrawPath(explainName, ref filePath, OnChangePath, false);
        }

        private string DrawPath(string explainName, ref string editPath, Action<string> OnChangePath, bool isFolder)
        {
            GUILayout.BeginHorizontal();

            if (isFolder)
                GUILayout.Label($"{explainName} Path : ", GUILayout.Width(150f));
            else
                GUILayout.Label($"{explainName} Path : ", GUILayout.Width(150f));

            GUI.enabled = false;
            GUILayout.TextArea(editPath, GUILayout.ExpandWidth(true), GUILayout.Height(40f));
            GUI.enabled = true;

            if (GUILayout.Button($"Edit {explainName}", GUILayout.Width(150f)))
            {
                string path = "";
                if (isFolder)
                    path = EditorUtility.OpenFolderPanel("Root Folder", "", "");
                else
                    path = EditorUtility.OpenFilePanel("File Path", "", "");

                editPath = path;
                OnChangePath?.Invoke(path);
            }

            GUILayout.EndHorizontal();

            return editPath;
        }

        private void CheckConfigPath()
        {
            if (string.IsNullOrEmpty(_configPath))
                return;

            Exception exception = CMDBuilder.DoTryParsing_JsonFileSO(_configPath, out _buildConfig);
            if (exception != null)
            {
                _configPath = "!! Error !!" + _configPath;
                Debug.LogError($"Json Parsing Fail Path : {_configPath}\n {exception}", this);
            }
        }

    }
}

#endif