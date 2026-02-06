using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.VFX;

public class Board : MonoBehaviour
{
    public TetronimoData[] tetronimos; 

    public TetrisManager tetrisManager;
   
    // Active piece initially contains a reference to the "Piece" prefab.
    public Piece piecePrefab;
    Piece activePiece;

    public Tilemap tilemap;
    public Vector2Int boardSize;
    public Vector2Int startPosition;
    
    // Maps tilemap position to a Piece gameObject
    Dictionary<Vector3Int, Piece> pieces = new Dictionary<Vector3Int, Piece> ();

    private void Start()
    {
        SpawnPiece();
    }

    public void SpawnPiece() 
    {
        activePiece = Instantiate(piecePrefab);
        
        // Spawns a random tetronimo at the start of every turn
        Tetronimo t = (Tetronimo)Random.Range(0, tetronimos.Length);

        activePiece.Initialize(this, t);
        Set(activePiece);
    }

    void SetTile(Vector3Int cellPosition, Piece piece)
    {
        if (piece == null)
        {
            tilemap.SetTile(cellPosition, null);

            pieces.Remove(cellPosition);
        }
        else
        {
            tilemap.SetTile(cellPosition, piece.data.tile);

            // This line is creating an association between the cell position and the piece gameObject.
            pieces[cellPosition] = piece;
        }
    }

    // Sets the tile color for this tetronimo piece.
    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int cellPosition = (Vector3Int)(piece.cells[i] + piece.position);
            SetTile(cellPosition, piece);
        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int cellPosition = (Vector3Int)(piece.cells[i] + piece.position);
            SetTile(cellPosition, null);
        }
    }

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

        // Scan from bottom to top 
        for (int y = bottom; y < top; y++)
        {
            if (IsLineFull(y))
            {
                DestroyLine(y);
                destroyedLines.Add(y);
            }
        }

        int rowsShiftedDown = 0; 

        // We shift down here. 
        foreach (int y in destroyedLines)
        {
            ShiftRowsDown(y - rowsShiftedDown);

            // At the end of every loop, we have shifted rows down by one more.
            rowsShiftedDown++;
        }

        // Finally ready to set the score! 
        int score = tetrisManager.CalculateScore(destroyedLines.Count);
        tetrisManager.ChangeScore(score);
    }

    void ShiftRowsDown(int clearedRow)
    {
        for (int y = clearedRow + 1; y < top - 1; y++)
        {
            for (int x = left; x < right; x ++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y);

                if (pieces.ContainsKey(cellPosition))
                {
                    // Store the piece temporarily.
                    Piece currentPiece = pieces[cellPosition];

                    // Clear the position it's in.
                    SetTile(cellPosition, null);

                    // Move the tile down.
                    cellPosition.y -= 1;
                    SetTile(cellPosition, currentPiece);
                }
            }
        }
    }

    bool IsLineFull(int y)
    {
        for (int x = left; x < right; x++)
        {
            Vector3Int cellPosition = new Vector3Int (x, y, 0);

            if (!tilemap.HasTile(cellPosition)) return false;
        }
        return true;
    }

    void DestroyLine(int y)
    {
        for (int x = left; x < right; x++)
        {
            Vector3Int cellPosition = new Vector3Int(x, y, 0);

            // Do something here to clean up gameObjects


            SetTile(cellPosition, null);
        }
    }
}
