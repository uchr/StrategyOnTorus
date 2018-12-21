using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TorusTerrain", menuName = "Torus/Terrain", order = 1)]
public class TorusTerrain : ScriptableObject
{
    public float height;
    public float minRadious;
    public float maxRadious;

    public int xCells;
    public int yCells;

    public Material material;
    public Texture2D heightMap;
}
