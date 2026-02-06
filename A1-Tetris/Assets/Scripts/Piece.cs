using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public TetronimoData data;
    public Vector2Int[] cells;
    public Vector2Int position;
    public Board board;

    bool freeze = false;

    bool isSpecial;

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
        for (int i = 0; i < cells.Length; i++) cells[i] = data.cells[i];

        // Set the start position.
        position = board.startPosition;
    }

    private void Update()
    {
        if (freeze) return;

        board.Clear(this);
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }
        else
        {
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
            // else if (Input.GetKeyDown(KeyCode.W))
            //{
            //    Move(Vector2Int.up);
            //}

            // Rotation
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Rotate(-1);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Rotate(1);
            }
        }



        board.Set(this);

        // We only check the board and spawn the new piece AFTER the final piece has been frozen.
        if (freeze)
        {
            board.CheckBoard();
            board.SpawnPiece();
        }
    }

    void Rotate(int direction)
    {
        Quaternion rotation = Quaternion.Euler(0, 0, 90.0f * direction);
       
        for (int i = 0; i < cells.Length; i++)
        {
            // This is JUST the local cell position.
            Vector2Int cellPosition = cells[i];

            // Cast this to a Vector3 because that's how Quaternions work.
            Vector3 cellPositionV3 = new Vector3(cellPosition.x, cellPosition.y);

            // Get the result
            Vector3 result = rotation * cellPositionV3;

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

        //  MISSED SOMETHING HERE
    }

    // Move will return whether or not the translation is valid. 
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
