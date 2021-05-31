#if WIP
using UnityEngine;
using System.Reflection;
using UnityEditor;

#if ASSET_BUNDLE_BROWSER
using AssetBundleBrowser;
#endif

namespace UNKO.Unity_Builder
{
    public class AssetBundleBrowserWrapper
    {
        /* const & readonly declaration             */

        const string const_prefix_ForLog = "!@#$";

        /* enum & struct declaration                */

        /* public - Field declaration               */


        /* protected & private - Field declaration  */

#if ASSET_BUNDLE_BROWSER
	AssetBundleBrowserMain _bundleBrowser;
#endif

        System.Type browserType;

        FieldInfo _buildtabField;
        FieldInfo _buildtabField_UserData;
        FieldInfo _buildtabField_UserData_BuildTarget;

        private object _buildTabInstance;
        private object _userDataInstance;

        MethodInfo _buildMethod;

        // ========================================================================== //

        /* public - [Do~Something] Function 	        */

        public AssetBundleBrowserWrapper()
        {
#if ASSET_BUNDLE_BROWSER
		_bundleBrowser = AssetBundleBrowserMain.GetWindow<AssetBundleBrowserMain>();
		browserType = _bundleBrowser.GetType();

		_buildtabField = browserType.GetField("m_BuildTab", BindingFlags.NonPublic | BindingFlags.Instance);
		_pInstance_BuildTab = _buildtabField.GetValue(_pBrowser);
		
		_buildtabField_UserData =  _buildtabField.FieldType.GetField("m_UserData", BindingFlags.NonPublic | BindingFlags.Instance);
		_pInstance_UserData = _buildtabField_UserData.GetValue(_pInstance_BuildTab);

		_buildtabField_UserData_BuildTarget = _buildtabField_UserData.FieldType.GetField("m_BuildTarget", BindingFlags.NonPublic | BindingFlags.Instance);
		
		_pMethod_Build = _buildtabField.FieldType.GetMethod("ExecuteBuild", BindingFlags.NonPublic | BindingFlags.Instance);
#endif
        }

        public void BuildBundle(BuildTarget eBuildTarget)
        {
            _buildtabField_UserData_BuildTarget.SetValue(_userDataInstance, (int)eBuildTarget);

            UnityEngine.Debug.Log($"{const_prefix_ForLog} Start Build Bundle \n" +
                                  $"Current Platform : {Application.platform} Target : {_buildtabField_UserData_BuildTarget.GetValue(_userDataInstance)} \n" +
                                  $"Symbol : {PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup)} \n");

#if ASSET_BUNDLE_BROWSER
		_pMethod_Build?.Invoke(_buildtabField.GetValue(_pBrowser), null);
		_bundleBrowser.Close();
#else
            UnityEngine.Debug.Log($"{const_prefix_ForLog} Use Define Symbol ASSET_BUNDLE_BROWSER");
#endif
            UnityEngine.Debug.Log($"{const_prefix_ForLog} Finish Build");
        }

        // ========================================================================== //

        /* protected - [Override & Unity API]       */


        /* protected - [abstract & virtual]         */


        // ========================================================================== //

#region Private

#endregion Private
    }
}
#endif
