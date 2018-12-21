using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TorusTerrain))]
public class TorusTerrainCustomInspector : Editor
{
    private TorusTerrain targetTorusTerrain;

    public override void OnInspectorGUI()
    {
        targetTorusTerrain = (TorusTerrain) target;

        DrawDefaultInspector();
        GenerateButton();
    }

    private void GenerateButton()
    {
        if (GUILayout.Button("Generate Mesh"))
            MeshBuilder.GenerateTorusTerrain(targetTorusTerrain);
    }
}
