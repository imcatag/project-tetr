using System.Collections.Generic;
using UnityEngine;

public class BotPiece : MonoBehaviour
{
    public BotBoard board { get; private set; }
    public Vector3Int position { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; set; }
    public int rotationIndex { get; set; }
    public bool canHold { get; private set; }
    public void Awake()
    {
        canHold = true;
    }
    public void Initialize(BotBoard board, Vector3Int position, TetrominoData data)
    {
        this.board = board;
        this.position = position;
        this.data = data;
        this.rotationIndex = 0;
        if (this.cells == null)
        {
            this.cells = new Vector3Int[data.cells.Length];
        }

        for (int i = 0; i < data.cells.Length; i++)
        {
            this.cells[i] = (Vector3Int)data.cells[i];
        }
        // Debug.Log("Position: " + (position) + " // RotationIndex: " + (rotationIndex));
    }

    public void Update()
    {
        board.Clear(this);
        // Instead of key based movement, get bot movement here

        board.Set(this);
        
    }
    private int Wrap (int value, int min, int max){
        if (value < min){
            return max - (min - value) % (max - min);
        }
        else{
            return min + (value - min) % (max - min);
        }
    }

    private void Hold(){
        if(canHold)
            board.Hold();
        canHold = false;
    }
    public void Lock(bool tspin = false, bool tspinmini = false)
    {
        board.Set(this);

        canHold = true;

        // Check for line clears

        
        board.ClearLines(tspin, tspinmini);
        board.SpawnPiece();
    }

    public void MoveTo(Vector2Int location, int orientation)
    {
        Vector3Int newPosition = (Vector3Int)location;
        position = newPosition;
        rotationIndex = orientation;
        // set cells
        Vector2Int[] newCells = Data.Cells[data.tetromino][rotationIndex];
        
        //create copy of newcells that is vector3Int[]
        Vector3Int[] newCells3 = new Vector3Int[newCells.Length];
        for (int i = 0; i < newCells.Length; i++){
            Vector2Int cell = newCells[i];
            Vector3Int newCell = new Vector3Int(cell.x, cell.y, 0);
            newCells3[i] = newCell;
        }
        cells = newCells3;
        
    }
}
