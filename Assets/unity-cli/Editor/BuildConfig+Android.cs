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

#if UNITY_EDITOR
using UnityEditor;

namespace Unity_CLI
{
    public partial class BuildConfig
    {
        [Serializable]
        public class AndroidSetting
        {
            /// <summary>
            /// 예시) com.CompanyName.ProductName  
            /// </summary>
            public string fullpackagename;

            public string keyalias_name;
            public string keyalias_password;

            /// <summary>
            /// Keystore 파일의 경로입니다. `파일경로/파일명.keystore` 까지 쓰셔야 합니다.
            /// <para>UnityProject/Asset/ 기준의 상대 경로입니다. </para>
            /// <para>예를들어 UnityProject/Asset 폴더 밑에 example.keystore를 넣으셨으면 "/example.keystore" 입니다.</para>
            /// </summary>
            public string keystore_relativePath;
            public string keystore_password;

            /// <summary>
            /// CPP 빌드를 할지 체크, CPP빌드는 오래 걸리므로 Test빌드가 아닌 Alpha 빌드부터 하는걸 권장
            /// 아직 미지원
            /// </summary>
            public bool usecppbuild;

            public int bundleversioncode;
            public string version;


            /// <summary>
            /// <para>ScriptableObject 생성시 생성자에 PlayerSettings에서 Get할경우 Unity Exception이 남</para>
            /// </summary>
            /// <returns></returns>
            public static AndroidSetting CreateSetting()
            {
                AndroidSetting newSetting = new AndroidSetting();

                newSetting.fullpackagename = PlayerSettings.applicationIdentifier;
                newSetting.bundleversioncode = PlayerSettings.Android.bundleVersionCode;
                newSetting.version = PlayerSettings.bundleVersion;

                return newSetting;
            }
        }

        public AndroidSetting androidSetting;
    }
}

#endif
