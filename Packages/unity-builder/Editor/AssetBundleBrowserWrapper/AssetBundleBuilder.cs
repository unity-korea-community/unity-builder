#if WIP
using UnityEditor;

namespace UNKO.Unity_Builder
{
    /// <summary>
    /// 
    /// </summary>
    public class AssetBundleBuilder
    {
        /* const & readonly declaration             */


        /* enum & struct declaration                */

        /* public - Field declaration               */


        /* protected & private - Field declaration  */


        // ========================================================================== //

        /* public - [Do~Something] Function 	        */


        [MenuItem("Tools/Build/BundleBuild Test - Android", priority = 10000)]
        public static void Build_Android()
        {
            AssetBundleBrowserWrapper pWrapper = new AssetBundleBrowserWrapper();
            pWrapper.BuildBundle(BuildTarget.Android);
        }


        [MenuItem("Tools/Build/BundleBuild Test - IOS", priority = 10000)]
        public static void Build_IOS()
        {
            AssetBundleBrowserWrapper pWrapper = new AssetBundleBrowserWrapper();
            pWrapper.BuildBundle(BuildTarget.iOS);
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
