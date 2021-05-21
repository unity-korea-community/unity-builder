using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Unity_Builder
{
    [CreateAssetMenu(fileName = "BuildConfigGroup", menuName = GlobalConst.CreateAssetMenu_Prefix + "/BuildConfigGroup")]
    public class BuildConfigGroup : ScriptableObject
    {
        public List<BuildConfig> configList = new List<BuildConfig>();
    }

    [CustomEditor(typeof(BuildConfigGroup))]
    public class BuildConfigGroup_Inspector : Editor
    {
        string _commandLine;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            BuildConfigGroup configGroup = target as BuildConfigGroup;
            if (GUILayout.Button("All Build!"))
            {
                configGroup.configList.ForEach(buildConfig => UnityBuilder.Build(buildConfig));
            }
        }
    }
}
