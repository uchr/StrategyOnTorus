using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public static class MeshBuilder {
    public static void GenerateTorusTerrain(TorusTerrainSettings terrainSettings) {
        string terrainName = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(terrainSettings));

        string pathToTerrainFolder = Path.Combine("Assets\\Terrains\\", terrainName);
        string pathToData = Path.Combine(pathToTerrainFolder, terrainName + "Data.asset");
        string pathToMesh = Path.Combine(pathToTerrainFolder, terrainName + "Mesh.asset");
        string pathToPrefab = Path.Combine(pathToTerrainFolder, terrainName + ".prefab");

        RemoveFolder(pathToTerrainFolder);

        TorusTerrainData terrainData = GenerateTorusTerrainData(terrainSettings);
        SaveAsset(terrainData, pathToTerrainFolder, pathToData);

        Mesh mesh = GenerateTorusTerrainMesh(terrainData, terrainSettings);
        SaveAsset(mesh, pathToTerrainFolder, pathToMesh);

        GameObject go = GenerateTorusTerrainPrefab(terrainName, mesh, terrainData, terrainSettings);
        SavePrefab(go, pathToTerrainFolder, pathToPrefab);
        Object.DestroyImmediate(go);
    }

    private static void RemoveFolder(string pathToFolder) {
        if(Directory.Exists(pathToFolder)) {
            Directory.Delete(pathToFolder, true);
            File.Delete(pathToFolder + ".meta");
            AssetDatabase.Refresh();
        }
    }

    private static void RemoveAsset(string pathToAsset) {
        if(File.Exists(pathToAsset)) {
            File.Delete(pathToAsset);
            File.Delete(pathToAsset + ".meta");
            AssetDatabase.Refresh();
        }
    }

    private static void SavePrefab(GameObject go, string pathToFolder, string pathToPrefab) {
        if (!Directory.Exists(pathToFolder))
            Directory.CreateDirectory(pathToFolder);
        PrefabUtility.SaveAsPrefabAsset(go, pathToPrefab);
    }

    private static void SaveAsset(Object asset, string pathToFolder, string pathToAsset) {
        if (!Directory.Exists(pathToFolder))
            Directory.CreateDirectory(pathToFolder);
        AssetDatabase.CreateAsset(asset, pathToAsset);
        AssetDatabase.SaveAssets();
    }

    private static GameObject GenerateTorusTerrainPrefab(string name,
                                                         Mesh mesh,
                                                         TorusTerrainData data,
                                                         TorusTerrainSettings settings) {
        GameObject go = new GameObject(name);
        go.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = go.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = mesh;
        go.GetComponent<Renderer>().material = settings.material;
        TorusTerrain terrain = go.AddComponent<TorusTerrain>();
        terrain.settings = settings;
        terrain.data = data;
        return go;
    }

    private static TorusTerrainData GenerateTorusTerrainData(TorusTerrainSettings settings) {
        Texture2D heightMap = (Texture2D) settings.heightMap;

        System.Func<int, int, TorusVector3> calcVertices = delegate(int ti, int tj) {
            int x = Mathf.FloorToInt((float) ti / settings.xCells * heightMap.width);
            int z = Mathf.FloorToInt((float) tj / settings.yCells * heightMap.height);
            float h = heightMap.GetPixel(x, z).grayscale;
            float sR = settings.smallRadious + h * settings.height;
            float xA = ti * (2.0f * Mathf.PI / settings.xCells);
            float yA = tj * (2.0f * Mathf.PI / settings.yCells);
            return new TorusVector3(sR, settings.bigRadious, xA, yA);
        };

        TorusCell[,] cells = new TorusCell[settings.xCells, settings.yCells];
        for(int i = 0; i < settings.xCells; ++i) {
            for(int j = 0; j < settings.yCells; ++j) {
                cells[i, j].v0 = calcVertices(i, j);
                cells[i, j].v1 = calcVertices(i + 1, j);
                cells[i, j].v2 = calcVertices(i, j + 1);
                cells[i, j].v3 = calcVertices(i + 1, j + 1);
            }
        }

        TorusTerrainData terrainData = ScriptableObject.CreateInstance<TorusTerrainData>();
        terrainData.cells = cells;
        return terrainData;
    }

    private static Mesh GenerateTorusTerrainMesh(TorusTerrainData data, TorusTerrainSettings settings) {
        Texture2D heightMap = (Texture2D) settings.heightMap;

        System.Func<TorusVector3, TorusVector3, TorusVector3, float> getAverageHeight = 
            delegate(TorusVector3 tv0, TorusVector3 tv1, TorusVector3 tv2) {
                float averageHeight = tv0.sR + tv1.sR + tv2.sR - 3 * settings.smallRadious;
                averageHeight /= 3 * settings.height;
                if(averageHeight < 0.25f)
                    averageHeight = 0.15f;
                else if(averageHeight < 0.5f)
                    averageHeight = 0.4f;
                else if(averageHeight < 0.75f)
                    averageHeight = 0.65f;
                else
                    averageHeight = 0.9f;

                return averageHeight;
        };

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> faces = new List<int>();

        int index = 0;
        for(int i = 0; i < settings.xCells; ++i) {
            for(int j = 0; j < settings.yCells; ++j) {
                TorusCell cell = data.cells[i, j];
                // Left down triangle
                vertices.Add(cell.v0.ToCartesian());
                vertices.Add(cell.v1.ToCartesian());
                vertices.Add(cell.v2.ToCartesian());

                float averageHeight = getAverageHeight(cell.v0, cell.v1, cell.v2);
                uvs.Add(new Vector2(0.4f, averageHeight));
                uvs.Add(new Vector2(0.5f, averageHeight));
                uvs.Add(new Vector2(0.4f, averageHeight - 0.05f));

                faces.Add(index + 0);
                faces.Add(index + 1);
                faces.Add(index + 2);
                index += 3;

                // Right top triangle
                vertices.Add(cell.v1.ToCartesian());
                vertices.Add(cell.v2.ToCartesian());
                vertices.Add(cell.v3.ToCartesian());

                averageHeight = getAverageHeight(cell.v1, cell.v2, cell.v3);
                uvs.Add(new Vector2(0.4f, averageHeight));
                uvs.Add(new Vector2(0.5f, averageHeight));
                uvs.Add(new Vector2(0.4f, averageHeight - 0.05f));

                faces.Add(index + 1);
                faces.Add(index + 0);
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
