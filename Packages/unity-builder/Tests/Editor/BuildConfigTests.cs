using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Unity_Builder;

namespace Tests
{
    public class BuildConfigTests
    {
        [Test]
        public void OverwriteTest()
        {
            AndroidBuildConfig buildConfig = AndroidBuildConfig.CreateInstance<AndroidBuildConfig>();
            buildConfig.ResetSetting(buildConfig);

            string productName = buildConfig.productName;
            string testProductName = "product_" + Random.Range(1, 100).ToString();

            string overwriteJson = $"{{\"productName\": \"{testProductName}\"}}";
            JsonUtility.FromJsonOverwrite(overwriteJson, buildConfig);

            Assert.AreNotEqual(buildConfig.productName, productName);
            Assert.AreEqual(buildConfig.productName, testProductName);
        }
    }
}
