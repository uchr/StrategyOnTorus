using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TorusTerrainSettings",
                 menuName = "Torus/Torus Terrain Settings", order = 1)]
public class TorusTerrainSettings : ScriptableObject {
    public float height;

    public float smallRadious;
    public float bigRadious;

    public int xCells;
    public int yCells;

    public Material material;
    public Texture2D heightMap;
}
