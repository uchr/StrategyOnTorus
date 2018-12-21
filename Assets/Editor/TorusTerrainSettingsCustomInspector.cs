using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TorusTerrainSettings))]
public class TorusTerrainSettingsCustomInspector : Editor
{
    private TorusTerrainSettings targetSetting;

    public override void OnInspectorGUI()
    {
        targetSetting = (TorusTerrainSettings) target;

        DrawDefaultInspector();
        GenerateButton();
    }

    private void GenerateButton()
    {
        if (GUILayout.Button("Generate Mesh"))
            MeshBuilder.GenerateTorusTerrain(targetSetting);
    }
}
