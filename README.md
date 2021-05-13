# unity-builder

## 소개

![](https://gblobscdn.gitbook.com/assets%2F-MZR4z-_JNrTJUnxyKxJ%2F-M_8lNE-u3FbNMIJAU6k%2F-M_8n78pOJUrs-S1R6jd%2Funity-builder.gif?alt=media&token=02169a87-68b3-4c32-be07-3799703e9d6c)‌

프로젝트에 목적에 따른 빌드마다\(**ex.** 개발버전, 치트버전, 스토어버전 등\)‌

다르게 해야 할 세팅들을 Unity-ScriptableObject로 저장하고,‌

빌드를 할 때 이 세팅을 Editor-BuildSetting-PlayerSetting에 덮어쓴 뒤‌

Inspector, CLI 등으로 빌드할 수 있게 해줍니다.‌

## 주요 기능 <a id="undefined-1"></a>

![](https://gblobscdn.gitbook.com/assets%2F-MZR4z-_JNrTJUnxyKxJ%2F-M_8diDv3CtBwdjU1_xx%2F-M_8gSv_sRzTgPjTA4d1%2Fimage.png?alt=media&token=7d0d556e-b0bb-48d7-ba5f-8b401cbe5997)‌

* Editor Setting을 SO값으로 세팅 및 빌드
* 빌드 결과물 경로에 주요 변수 **문자열 보간**
  * **ex.** Build/{productName}\_{MM}{dd}\_{hh}{mm}
  * **result.** Build/unity-builder\_월월일일\_시시분분
* **CLI**로 빌드시 SO값을 덮어쓰기 가능
  * **ex.**
  * `-ovewrite {"product": "원하는값", "bundleVersionCode": "1"}`

‌

## 설치 <a id="undefined-2"></a>

Unity Editor/상단 Window 탭/Package Manager/+ 버튼/‌

Add package from git URL 클릭 후‌

이 저장소의 URL 입력‌

​[`https://github.com/unity-korea-community/unity-builder.git`](https://github.com/unity-korea-community/unity-builder.git)

## CLI 명령어 <a id="cli"></a>

‌

유니티 CLI Arg에 있는 `-executeMethod`를 이용합니다.‌

### 반드시 필요한 명령어 <a id="undefined-4"></a>

| 인 | 설명 | 예시 |
| :--- | :--- | :--- |
| `-configpath` |  UnityProject/하위 기준 로컬경로 | Assets/unity-builder/Sample/AndroidBuildConfig.asset |

‌

### 옵션 명령어 <a id="undefined-5"></a>

<table>
  <thead>
    <tr>
      <th style="text-align:left">&#xC778;&#xC790;</th>
      <th style="text-align:left">&#xC124;&#xBA85;</th>
      <th style="text-align:left">&#xC608;&#xC2DC;</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td style="text-align:left"><code>-overwrite</code>
      </td>
      <td style="text-align:left">
        <p>json &#xD615;&#xC2DD;&#xC758;</p>
        <p>SO&#xC5D0; &#xB36E;&#xC5B4; &#xC4F8; &#xAC12;</p>
      </td>
      <td style="text-align:left">{&quot;product&quot;: &quot;&#xC6D0;&#xD558;&#xB294;&#xAC12;&quot;, &quot;bundleVersionCode&quot;:
        &quot;777&quot;}</td>
    </tr>
  </tbody>
</table>

‌

### Full Command Line Example <a id="full-command-line-example"></a>

‌

`-quit -batchmode -executeMethod UnityBuilder.Build -configpath Assets/unity-builder/Sample/AndroidBuildConfig.asset`‌

### CommandLineArguments - 유니티 메뉴얼 [https://docs.unity3d.com/Manual/CommandLineArguments.html](https://docs.unity3d.com/Manual/CommandLineArguments.html)​ <a id="commandlinearguments-https-docs-unity-3-d-com-manual-commandlinearguments-html"></a>

‌

## 참고한 링크 <a id="undefined-3"></a>

‌

* **KorStrix/Unity\_JenkinsBuilder**
  * ​[https://github.com/KorStrix/Unity\_JenkinsBuilder](https://github.com/KorStrix/Unity_JenkinsBuilder)
* **mob-sakai/ProjectBuilder**
  * ​[https://github.com/mob-sakai/ProjectBuilder](https://github.com/mob-sakai/ProjectBuilder)
* **mob-sakai/SimpleBuildInterface**

  * ​[https://github.com/mob-sakai/SimpleBuildInterface](https://github.com/mob-sakai/SimpleBuildInterface)​

  ​

* **superunitybuild/buildtool**
  * ​[https://github.com/superunitybuild/buildtool](https://github.com/superunitybuild/buildtool)

