using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GhostPiece : MonoBehaviour
{
    public Board board { get; private set; }
    public Ghost ghost { get; private set; }
    public Vector3Int position { get; set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; set; }
    public int rotationIndex { get; set; }

    public void Initialize(Board board1, Ghost ghost1, Piece piece1)
    {
        this.board = board1;
        this.ghost = ghost1;
        this.position = piece1.position;
        this.data = piece1.data;
        this.rotationIndex = piece1.rotationIndex;


        if (this.cells == null)
        {
            this.cells = new Vector3Int[data.cells.Length];
        }

        Vector2Int[] newCells = Data.Cells[data.tetromino][rotationIndex];
        
        // get cells from Data class
        for (int i = 0; i < data.cells.Length; i++)
        {
            this.cells[i] = (Vector3Int)newCells[i];
        }

        while(!board.Collides(cells, position + Vector3Int.down, piece1)){
            position += Vector3Int.down;
        }

        // Debug.Log("Position: " + (position) + " // RotationIndex: " + (rotationIndex));
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
