using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.VFX;

public class Board : MonoBehaviour
{
    public TetrisManager tetrisManager;
    
    public TetronimoData[] tetronimos;

    // Active piece initially contains a reference to the "Piece" prefab.
    public Piece piecePrefab;
    public Piece activePiece;

    public Vector2Int startPosition;
    public Tilemap tilemap;
    public Vector2Int boardSize;

    

    int left
    {
        get { return -boardSize.x / 2; }
    }
    int right
    {
        get { return boardSize.x / 2; }
    }
    int top
    {
        get { return boardSize.y / 2; }
    }
    int bottom
    {
        get { return -boardSize.y / 2; }
    }

    private void Start()
    {
        SpawnPiece();
    }

    public void SpawnPiece() 
    {
        activePiece = Instantiate(piecePrefab);
        
        Tetronimo t = (Tetronimo)Random.Range(0, tetronimos.Length);
        activePiece.Initialize(this, Tetronimo.T);
        Set(activePiece);
    }

    // Sets the tile color for this tetronimo piece.
    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int cellPosition = (Vector3Int)(piece.cells[i] + piece.position);
            tilemap.SetTile(cellPosition, piece.data.tile);
        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int cellPosition = (Vector3Int)(piece.cells[i] + piece.position);
            tilemap.SetTile(cellPosition, null);
        }
    }

    public bool IsPositionValid(Piece piece, Vector2Int position)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int cellPosition = (Vector3Int)(piece.cells[i] + position);

            // Check to see if the position is already occupied in the tilemap. 
            if (tilemap.HasTile(cellPosition)) return false;

            // Check to see if within bounds
            if (cellPosition.x < left || cellPosition.x >= right ||
                cellPosition.y < bottom || cellPosition.y >= top) return false;
        }
        
        return true;
    }

    public void CheckBoard()
    {
        List<int> destroyedLines = new List<int>();
        
        for (int y = bottom; y < top; y++)
        {
            if (IsLineFull(y))
            {
                DestroyLine(y);
                destroyedLines.Add(y);
            }
        }

        // Debug.Log($"Lines Destroyed:");

        int rowsShiftedDown = 0; 

        // We shift down here.
        foreach (int clearedRow in destroyedLines)
        {
            ShiftRowsDown(clearedRow);
            rowsShiftedDown++;
        }

    }

    void ShiftRowsDown(int clearedRow)
    {
        // top - 1 is correct - we will see this once we get to end conditions.
        for (int y = clearedRow; y < top; y++)
        {
            for (int x = left; x < right; x ++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y);

                // Store the tile temporarily.
                TileBase currentTile = tilemap.GetTile(cellPosition);

                // Clear the position it's in.
                tilemap.SetTile(cellPosition, null);

                // Move the tile down.
                cellPosition.y -= 1;
                tilemap.SetTile(cellPosition, currentTile);
            }
        }
    }

    bool IsLineFull(int y)
    {
        for (int x = left; x < right; x++)
        {
            Vector3Int cellPosition = new Vector3Int (x, y);

            if (!tilemap.HasTile(cellPosition)) return false;
        }
        return true;
    }

    void DestroyLine(int y)
    {
        for (int x = left; x < right; x++)
        {
            Vector3Int cellPosition = new Vector3Int(x, y);

            // MISSED SOMETHING HERE
        }
    }


    // Finally ready to set the score! not working atm!
    // int score = TetrisManager.CalculateScore(destroyedLines.Count);
}
