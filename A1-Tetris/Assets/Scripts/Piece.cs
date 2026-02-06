using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public TetronimoData data;
    public Board board;
    public Vector2Int[] cells;
    public Vector2Int position;
    
    bool freeze = false;

    //Remove later
    //bool isSpecial;

    public void Initialize(Board board, Tetronimo tetronimo)
    {
        // The piece object needs a reference to the game board.
        this.board = board;

        // Find the tetronimo data and attach it.
        for (int i = 0; i < board.tetronimos.Length; i++)
        {
            if (board.tetronimos[i].tetronimo == tetronimo)
            {
                this.data = board.tetronimos[i];
                break;
            }
        }

        // Create a copy of the Tetronimo local cell coordinates.
        cells = new Vector2Int[data.cells.Length];
        for (int i = 0; i < data.cells.Length; i++) cells[i] = data.cells[i];

        // Set the start position.
        position = board.startPosition;
    }

    private void Update()
    {
        // If the piece is frozen, do NOT process the update loop
        if (freeze) return;

        board.Clear(this);
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }
        else
        {
            // If we hard drop, do NOT process any other piece motion.
            if (Input.GetKeyDown(KeyCode.A))
            {
                Move(Vector2Int.left); 
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                Move(Vector2Int.right);
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                Move(Vector2Int.down);
            }

            // Just for debugging
            // else if (Input.GetKeyDown(KeyCode.W))
            //{
            //    Move(Vector2Int.up);
            //}

            // Rotation Inputs
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Rotate(1); //Clockwise
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Rotate(-1); //Counter-Clockwise
            }
        }

        board.Set(this);

        // DEBUG ONLY - "P" is the debug key.
        if (Input.GetKeyDown(KeyCode.P))
        {
            board.CheckBoard();
        }

        // We only check the board and spawn the new piece AFTER the final piece has been frozen.
        if (freeze)
        {
            board.CheckBoard();
            board.SpawnPiece();
        }
    }

    void Rotate(int direction)
    {
        // Store the cell locations so that we can revert it.
        Vector2Int[] temporaryCells = new Vector2Int[cells.Length];
        for (int i = 0; i < cells.Length; i++) temporaryCells[i] = cells[i];
        
        // Performs normal rotation
        ApplyRotation(direction);

        // Checks if rotation is valid
        if (!board.IsPositionValid(this, position))
        {
            if (!TryWallKicks())
            {
                RevertRotation(temporaryCells);
            }
            else
            {
                Debug.Log("Wall kick success");
            }
        }
        else
        {
            Debug.Log("Valid rotation");
        }
    }

    bool TryWallKicks()
    {
        Vector2Int[] wallKickOffsets = new Vector2Int[]
        {
            Vector2Int.left,
            Vector2Int.right,
            Vector2Int.down,
            new Vector2Int(-1, -1), // Diagonally down-left
            new Vector2Int(1, -1), // Diagonally down-right
        };

        foreach (Vector2Int offset in wallKickOffsets)
        {
            if (Move(offset)) return true;
        }

        return false;
    }

    void RevertRotation(Vector2Int[] temporaryCells)
    {
        for (int i = 0; i < cells.Length; i++) cells[i] = temporaryCells[i];
    }

    void ApplyRotation(int direction)
    {
        Quaternion rotation = Quaternion.Euler(0, 0, 90.0f * direction);

        bool isSpecial = data.tetronimo == Tetronimo.I || data.tetronimo == Tetronimo.O || data.tetronimo == Tetronimo.E;

        for (int i = 0; i < cells.Length; i++)
        {
            // Convert cell location to a Vector3 to work with Quaternions
            Vector3 cellPosition = new Vector3(cells[i].x, cells[i].y);

            if (isSpecial)
            {
                cellPosition.x -= 0.5f;
                cellPosition.y -= 0.5f;
            }

            // Get the result
            Vector3 result = rotation * cellPosition;

            // Put it back in the cells data
            if (isSpecial)
            {
                cells[i].x = Mathf.CeilToInt(result.x);
                cells[i].y = Mathf.CeilToInt(result.y);
            }
            else
            {
                cells[i].x = Mathf.RoundToInt(result.x);
                cells[i].y = Mathf.RoundToInt(result.y);
            }
        }
    }

    void HardDrop()
    {
        // Keep moving down until the translation is invalid.
        while (Move(Vector2Int.down))
        {
            // do nothing
        }

        freeze = true;
    }

    // Move will return whether or not the translation is valid.
    // Will ONLY actually move the tetronimo if the position is valid.
    bool Move(Vector2Int translation)
    {
        Vector2Int newPosition = position;
        newPosition += translation;

        // If the new position is valid, then we move to that position.
        bool isValid = board.IsPositionValid(this, newPosition);   
        if (isValid) position = newPosition;

        return isValid;
    }
}
