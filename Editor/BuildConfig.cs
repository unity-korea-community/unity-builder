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

using System;
using UnityEngine;
using UnityEditor;

namespace CMD
{
    [Serializable]
    public partial class BuildConfig : ScriptableObject
    {
        /// <summary>
        /// 출력할 파일 명, cmd에서 -filename (filename:string) 로 설정가능
        /// </summary>
        public string filename = "Build";

        public string definesymbol;

        /// <summary>
        /// 설치한 디바이스에 표기될 이름
        /// </summary>
        public string productname;

        /// <summary>
        /// 빌드에 포함할 씬들, 확장자는 안쓰셔도 됩니다.
        /// <para>예시) ["Assets/SomethingScene_1", "Assets/SomethingScene_1"]</para>
        /// </summary>
        public string[] buildscenenames;


        // 출력할 폴더 및 파일은 Jenkins에서 처리할 예정이였으나,
        // IL2CPP의 경우 같은 장소에 빌드해놓으면 더 빠르다는 메뉴얼로 인해 일단 보류
        // https://docs.unity3d.com/kr/2020.2/Manual/IL2CPP-OptimizingBuildTimes.html
        public string buildoutputFolder_absolutepath;

        /// <summary>
        /// 빌드파일 끝에 DateTime을 붙일지
        /// </summary>
        public bool usedatetimesuffix;

        /// <summary>
        /// <see>
        ///     <cref>ScriptableObject.CreateInstance</cref>
        /// </see>
        ///     Wrapper
        /// <para>ScriptableObject 생성시 생성자에 PlayerSettings에서 Get할경우 Unity Exception이 남</para>
        /// </summary>
        public static BuildConfig CreateConfig()
        {
            BuildConfig config = ScriptableObject.CreateInstance<BuildConfig>();

            config.productname = PlayerSettings.productName;
            config.buildscenenames = CMDBuilder.GetEnabled_EditorScenes();

            config.buildoutputFolder_absolutepath = Application.dataPath.Replace("/Assets", "") + "/Build";
            config.usedatetimesuffix = true;

            config.androidSetting = AndroidSetting.CreateSetting();
            config.iOSSetting_ = iOSSetting.CreateSetting();

            return config;
        }
    }
}
