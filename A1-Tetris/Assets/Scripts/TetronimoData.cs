using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Tetronimo { I, O, T, J, L, S, Z }

[Serializable]
public struct TetronimoData
{
    public Tetronimo tetronimo;
    public Tile tile;
    public Vector2Int[] cells;
}

