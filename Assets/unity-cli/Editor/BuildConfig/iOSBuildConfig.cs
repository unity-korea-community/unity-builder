using System;

namespace Unity_CLI
{
    /// <summary>
    /// 유니티 -> XCode Export -> XCode ArchiveBuildConfig -> .ipa 에 필요한 모든 설정
    /// </summary>
    [Serializable]
    public class iOSBuildConfig
    {
        /// <summary>
        /// 애플 개발자 사이트에서 조회 가능, 숫자랑 영어로 된거
        /// </summary>
        public string strAppleTeamID;

        /// <summary>
        /// Apple에서 세팅된 net.Company.Product 형식의 string
        /// </summary>
        public string strBundle_Identifier;
        public string strEntitlementsFileName_Without_ExtensionName;

        /// <summary>
        /// 유니티 Asset 경로에서 XCode Project로 카피할 파일 목록, 확장자까지 작성해야 합니다
        /// <para>UnityProject/Assets/ 기준</para>
        /// </summary>
        public string[] arrCopy_AssetFilePath_To_XCodeProjectPath;

        /// <summary>
        /// XCode 프로젝트에 추가할 Framework, 확장자까지 작성해야 합니다
        /// </summary>
        public string[] arrXCode_Framework_Add;

        /// <summary>
        /// XCode 프로젝트에 제거할 Framework, 확장자까지 작성해야 합니다
        /// </summary>
        public string[] arrXCode_Framework_Remove;

        public string[] arrXCode_OTHER_LDFLAGS_Add;
        public string[] arrXCode_OTHER_LDFLAGS_Remove;

        /// <summary>
        /// HTTP 주소 IOS는 기본적으로 HTTP를 허용 안하기 때문에 예외에 추가해야 합니다. http://는 제외할것
        /// <para>예시) http://www.naver.com = www.naver.com</para>
        /// </summary>
        public string[] arrHTTPAddress;

        /// <summary>
        /// 출시할 빌드 버전
        /// <para>이미 앱스토어에 올렸으면 그 다음 항상 숫자를 올려야 합니다. 안그럼 앱스토어에서 안받음</para>
        /// </summary>
        public string strBuildVersion;

        /// <summary>
        /// 빌드 번호
        /// <para>이미 앱스토어에 올렸으면 그 다음 항상 1씩 올려야 합니다. 안그럼 앱스토어에서 안받음</para>
        /// </summary>
        public string strBuildNumber;

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

        public PLIST_ADD[] arrAddPlist = new PLIST_ADD[] { new PLIST_ADD("ExampleKey", "ExampleValue") };
        public string[] arrRemovePlistKey = new string[] { "ExampleKey", "ExampleValue" };

        /// <summary>
        /// <para>ScriptableObject 생성시 생성자에 PlayerSettings에서 Get할경우 Unity Exception이 남</para>
        /// </summary>
        /// <returns></returns>
        public static iOSBuildConfig CreateSetting()
        {
            iOSBuildConfig pNewSetting = new iOSBuildConfig();

            return pNewSetting;
        }
    }
}