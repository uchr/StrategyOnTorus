using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public static class MeshBuilder {
    public static void GenerateTorusTerrain(TorusTerrainSettings terrain) {
        string terrainName = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(terrain));

        string pathToPrefabFolder = "Assets\\Prefabs\\Terrains";
        string pathToPrefab = Path.Combine(pathToPrefabFolder, terrainName + ".prefab");
        string pathToAssetFolder = "Assets\\Meshes\\Terrains";
        string pathToAsset = Path.Combine(pathToAssetFolder, terrainName + ".asset");

        RemoveAsset(pathToAsset);

        Mesh mesh = GenerateTorusTerrainMesh(terrain);
        SaveMesh(mesh, pathToAssetFolder, pathToAsset);

        GameObject go = GenerateTorusTerrain(terrainName, mesh, terrain);
        SavePrefab(go, pathToPrefabFolder, pathToPrefab);
        Object.DestroyImmediate(go);
    }

    private static void RemoveAsset(string pathToAsset) {
        if(File.Exists(pathToAsset)) {
            File.Delete(pathToAsset + ".meta");
            File.Delete(pathToAsset);
        }
        AssetDatabase.Refresh();
    }

    private static void SavePrefab(GameObject go, string pathToFolder, string pathToPrefab) {
        if (!Directory.Exists(pathToFolder))
            Directory.CreateDirectory(pathToFolder);
        PrefabUtility.SaveAsPrefabAsset(go, pathToPrefab);
    }

    private static void SaveMesh(Mesh mesh, string pathToFolder, string pathToAsset) {
        if (!Directory.Exists(pathToFolder))
            Directory.CreateDirectory(pathToFolder);
        AssetDatabase.CreateAsset(mesh, pathToAsset);
    }

    private static GameObject GenerateTorusTerrain(string name, Mesh mesh, TorusTerrainSettings terrain) {
        GameObject go = new GameObject(name);
        go.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = go.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = mesh;
        go.GetComponent<Renderer>().material = terrain.material;
        return go;
    }

    private static Mesh GenerateTorusTerrainMesh(TorusTerrainSettings terrain) {
        Texture2D heightMap = (Texture2D) terrain.heightMap;

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> faces = new List<int>();

        int x, z, ti, tj;
        float h, midleHeight, fi, fj, rHeight;
        int index = 0;
        for(int i = 0; i < terrain.xCells; ++i) {
            for(int j = 0; j < terrain.yCells; ++j) {
                // Left down triangle
                midleHeight = 0.0f;

                ti = i;
                tj = j;
                x = Mathf.FloorToInt((float) ti / terrain.xCells * heightMap.width);
                z = Mathf.FloorToInt((float) tj / terrain.yCells * heightMap.height);
                h = heightMap.GetPixel(x, z).grayscale;
                midleHeight += h;
                rHeight = terrain.smallRadious + h * terrain.height;
                fi = ti * (2.0f * Mathf.PI / terrain.xCells);
                fj = tj * (2.0f * Mathf.PI / terrain.yCells);
                vertices.Add(new Vector3((terrain.bigRadious + rHeight * Mathf.Cos(fi)) * Mathf.Cos(fj), rHeight * Mathf.Sin(fi), (terrain.bigRadious + rHeight * Mathf.Cos(fi)) * Mathf.Sin(fj)));

                ti = i;
                tj = j + 1;
                x = Mathf.FloorToInt((float) ti / terrain.xCells * heightMap.width);
                z = Mathf.FloorToInt((float) tj / terrain.yCells * heightMap.height);
                h = heightMap.GetPixel(x, z).grayscale;
                midleHeight += h;
                rHeight = terrain.smallRadious + h * terrain.height;
                fi = ti * (2.0f * Mathf.PI / terrain.xCells);
                fj = tj * (2.0f * Mathf.PI / terrain.yCells);
                vertices.Add(new Vector3((terrain.bigRadious + rHeight * Mathf.Cos(fi)) * Mathf.Cos(fj), rHeight * Mathf.Sin(fi), (terrain.bigRadious + rHeight * Mathf.Cos(fi)) * Mathf.Sin(fj)));

                ti = i + 1;
                tj = j;
                x = Mathf.FloorToInt((float) ti / terrain.xCells * heightMap.width);
                z = Mathf.FloorToInt((float) tj / terrain.yCells * heightMap.height);
                h = heightMap.GetPixel(x, z).grayscale;
                midleHeight += h;
                rHeight = terrain.smallRadious + h * terrain.height;
                fi = ti * (2.0f * Mathf.PI / terrain.xCells);
                fj = tj * (2.0f * Mathf.PI / terrain.yCells);
                vertices.Add(new Vector3((terrain.bigRadious + rHeight * Mathf.Cos(fi)) * Mathf.Cos(fj), rHeight * Mathf.Sin(fi), (terrain.bigRadious + rHeight * Mathf.Cos(fi)) * Mathf.Sin(fj)));

                midleHeight /= 3;
                if(midleHeight < 0.25f) {
                    midleHeight = 0.15f;
                }
                else if(midleHeight < 0.5f) {
                    midleHeight = 0.4f;
                }
                else if(midleHeight < 0.75f) {
                    midleHeight = 0.65f;
                }
                else {
                    midleHeight = 0.9f;
                }
                uvs.Add(new Vector2(0.4f, midleHeight));
                uvs.Add(new Vector2(0.5f, midleHeight));
                uvs.Add(new Vector2(0.4f, midleHeight - 0.05f));

                faces.Add(index + 1);
                faces.Add(index);
                faces.Add(index + 2);
                index += 3;

                // Right top triangle
                midleHeight = 0.0f;
                ti = i + 1;
                tj = j + 1;
                x = Mathf.FloorToInt((float) ti / terrain.xCells * heightMap.width);
                z = Mathf.FloorToInt((float) tj / terrain.yCells * heightMap.height);
                h = heightMap.GetPixel(x, z).grayscale;
                midleHeight += h;
                rHeight = terrain.smallRadious + h * terrain.height;
                fi = ti * (2.0f * Mathf.PI / terrain.xCells);
                fj = tj * (2.0f * Mathf.PI / terrain.yCells);
                vertices.Add(new Vector3((terrain.bigRadious + rHeight * Mathf.Cos(fi)) * Mathf.Cos(fj), rHeight * Mathf.Sin(fi), (terrain.bigRadious + rHeight * Mathf.Cos(fi)) * Mathf.Sin(fj)));

                ti = i + 1;
                tj = j;
                x = Mathf.FloorToInt((float) ti / terrain.xCells * heightMap.width);
                z = Mathf.FloorToInt((float) tj / terrain.yCells * heightMap.height);
                h = heightMap.GetPixel(x, z).grayscale;
                midleHeight += h;
                rHeight = terrain.smallRadious + h * terrain.height;
                fi = ti * (2.0f * Mathf.PI / terrain.xCells);
                fj = tj * (2.0f * Mathf.PI / terrain.yCells);
                vertices.Add(new Vector3((terrain.bigRadious + rHeight * Mathf.Cos(fi)) * Mathf.Cos(fj), rHeight * Mathf.Sin(fi), (terrain.bigRadious + rHeight * Mathf.Cos(fi)) * Mathf.Sin(fj)));

                ti = i;
                tj = j + 1;
                x = Mathf.FloorToInt((float) ti / terrain.xCells * heightMap.width);
                z = Mathf.FloorToInt((float) tj / terrain.yCells * heightMap.height);
                h = heightMap.GetPixel(x, z).grayscale;
                midleHeight += h;
                rHeight = terrain.smallRadious + h * terrain.height;
                fi = ti * (2.0f * Mathf.PI / terrain.xCells);
                fj = tj * (2.0f * Mathf.PI / terrain.yCells);
                vertices.Add(new Vector3((terrain.bigRadious + rHeight * Mathf.Cos(fi)) * Mathf.Cos(fj), rHeight * Mathf.Sin(fi), (terrain.bigRadious + rHeight * Mathf.Cos(fi)) * Mathf.Sin(fj)));

                midleHeight /= 3;
                if(midleHeight < 0.25f) {
                    midleHeight = 0.15f;
                }
                else if(midleHeight < 0.5f) {
                    midleHeight = 0.4f;
                }
                else if(midleHeight < 0.75f) {
                    midleHeight = 0.65f;
                }
                else {
                    midleHeight = 0.9f;
                }
                uvs.Add(new Vector2(0.4f, midleHeight));
                uvs.Add(new Vector2(0.5f, midleHeight));
                uvs.Add(new Vector2(0.4f, midleHeight - 0.1f));

                faces.Add(index + 1);
                faces.Add(index);
                faces.Add(index + 2);
                index += 3;
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = faces.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        return mesh;
    }
}
