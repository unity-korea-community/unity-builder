# Unity-Builder

## 소개

프로젝트에 목적에 따른 빌드마다\(**ex.** 개발버전, 치트버전, 스토어버전 등\)

다르게 해야 할 세팅들을 Unity-ScriptableObject로 저장하고,

빌드를 할 때 이 세팅을 Editor-BuildSetting-PlayerSetting에 덮어쓴 뒤

Inspector, CLI 등으로 빌드할 수 있게 해줍니다.

## 주요 기능

![](.gitbook/assets/image%20%283%29%20%281%29.png)

* Editor Setting을 SO값으로 세팅 및 빌드
* 빌드 결과물 경로에 주요 변수 **문자열 보간**
  * **ex.** Build/{productName}\_{MM}{dd}\_{hh}{mm}
  * **result.** Build/unity-builder\_월월일일\_시시분분
* **CLI**로 빌드시 SO값을 덮어쓰기 가능
  * **ex.** 
  * `-ovewrite {"product": "원하는값", "bundleVersionCode": "1"}`

## CLI 명령어

유니티 CLI Arg에 있는 `-executeMethod`를 이용합니다.

### 반드시 필요한 명령어

* **-configpath** UnityProject/하위 기준 로컬경로
  * Editor 내 SO를 선택 후 오른쪽 마우스 -&gt; CopyPath 값

| 인 | 설명 | 예시 |
| :--- | :--- | :--- |
| `-configpath` |  UnityProject/하위 기준 로컬경로 | Assets/unity-builder/Sample/AndroidBuildConfig.asset |

### 옵션 명령어

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

### Full Command Line Example

`-quit -batchmode -executeMethod UnityBuilder.Build -configpath Assets/unity-builder/Sample/AndroidBuildConfig.asset`

### CommandLineArguments - 유니티 메뉴얼 [https://docs.unity3d.com/Manual/CommandLineArguments.html](https://docs.unity3d.com/Manual/CommandLineArguments.html)

## 참고한 링크

* **과거의 내가 작성한 Jenkins 빌**
  * [https://github.com/KorStrix/Unity\_JenkinsBuilder](https://github.com/KorStrix/Unity_JenkinsBuilder) 
* **mob-sakai의 ProjectBuilder**
  * [https://github.com/mob-sakai/ProjectBuilder](https://github.com/mob-sakai/ProjectBuilder) 
* **mob-sakai의 SimpleBuildInterface**

  * [https://github.com/mob-sakai/SimpleBuildInterface](https://github.com/mob-sakai/SimpleBuildInterface)

* **superunitybuild의 buildtool**
  * [https://github.com/superunitybuild/buildtool](https://github.com/superunitybuild/buildtool)  



