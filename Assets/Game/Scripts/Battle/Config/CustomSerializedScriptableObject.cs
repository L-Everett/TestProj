using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
sealed class ConfigTypeAttribute : Attribute
{
    public ConfigTypeAttribute(string typeName)
    {
        TypeName = typeName;
    }
    public string TypeName { private set; get; }
}

public class CustomSerializedScriptableObject : SerializedScriptableObject
{
    [Label("ID"), DelayedProperty]
    public string Id = "Unknow";
    [Label("名称"), DelayedProperty]
    public string Name = "未设置";

#if UNITY_EDITOR
    [OnInspectorGUI]
    public void OnUpdateObject()
    {
        RenameFile();
    }

    private void RenameFile()
    {
        string[] guids = UnityEditor.Selection.assetGUIDs;
        int i = guids.Length;
        if (i == 1)
        {
            string guid = guids[0];
            string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            var type = GetType();
            var so = UnityEditor.AssetDatabase.LoadAssetAtPath(assetPath, type);
            if (so != this) return;
            var fileName = Path.GetFileName(assetPath);
            var configType = GetType().GetCustomAttribute<ConfigTypeAttribute>();
            if (configType == null) return;
            var newName = $"{configType.TypeName}_{Id}";
            if (!fileName.StartsWith(newName))
            {
                UnityEditor.AssetDatabase.RenameAsset(assetPath, newName);
            }
        }
    }
#endif
}
