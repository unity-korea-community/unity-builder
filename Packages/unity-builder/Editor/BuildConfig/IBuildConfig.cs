using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UNKO.Unity_Builder
{
    /// <summary>
    /// Unity를 빌드하기 위해 세팅할 interface
    /// </summary>
    public interface IBuildConfig
    {
        /// <summary>
        /// 실행될 빌드 타겟
        /// </summary>
        /// <inheritdoc/>
        BuildTarget buildTarget { get; }

        /// <summary>
        /// 빌드 세팅을 리셋할 때
        /// </summary>
        /// <param name="config">리셋할 대상 config</param>
        void ResetSetting(BuildConfigBase config);

        /// <summary>
        /// 빌드 출력 경로를 얻고자 할 때
        /// </summary>
        /// <returns>빌드 출력 경로</returns>
        string GetBuildPath();

        /// <summary>
        /// 빌드를 하기 전 이벤트 함수
        /// </summary>
        /// <param name="commandLine">빌드에 실행한 commandline</param>
        /// <param name="buildPlayerOptions">빌드에 사용한 option</param>
        void OnPreBuild(IDictionary<string, string> commandLine, ref BuildPlayerOptions buildPlayerOptions);

        /// <summary>
        /// 빌드를 한 뒤 이벤트 함수
        /// </summary>
        /// <param name="commandLine">빌드에 실행한 commandline</param>
        void OnPostBuild(IDictionary<string, string> commandLine);
    }

    /// <summary>
    /// Unity Inspector에 등록하기 위해 ScriptableObject를 상속
    /// </summary>
    public abstract class BuildConfigSO : ScriptableObject, IBuildConfig
    {
        ///<inheritdoc cref="IBuildConfig.buildTarget"/>
        public abstract BuildTarget buildTarget { get; }

        ///<inheritdoc cref="IBuildConfig.ResetSetting"/>
        public abstract void ResetSetting(BuildConfigBase config);
        ///<inheritdoc cref="IBuildConfig.GetBuildPath"/>
        public abstract string GetBuildPath();

        ///<inheritdoc cref="IBuildConfig.OnPostBuild"/>
        public abstract void OnPostBuild(IDictionary<string, string> commandLine);
        ///<inheritdoc cref="IBuildConfig.OnPreBuild"/>
        public abstract void OnPreBuild(IDictionary<string, string> commandLine, ref BuildPlayerOptions buildPlayerOptions);
    }
}
