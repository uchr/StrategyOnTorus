using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorusTerrainData : ScriptableObject {
    [SerializeField] public TorusCell[,] cells;
}
