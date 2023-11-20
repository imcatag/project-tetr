using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public Vector3Int position { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public int rotationIndex { get; private set; }
    public bool tspin { get; private set; }
    public bool tspinmini { get; private set; }

    public bool canHold { get; private set; }
    // public Boolean islastactionrotate { get; private set; }

    public void Initialize(Board board, Vector3Int position, TetrominoData data)
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
        Debug.Log("Position: " + (position) + " // RotationIndex: " + (rotationIndex));
    }

    public void Update()
    {
        board.Clear(this);

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Move(Vector2Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Move(Vector2Int.right);
        }
        
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Move(Vector2Int.down);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            Rotate(-1);
        }

        if(Input.GetKeyDown(KeyCode.D))
        {
            Rotate(1);
        }
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            Hold();
        }
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

    private bool Rotate (int direction){

        var newrotationIndex = Wrap(this.rotationIndex + direction, 0, 4);
        
        foreach(var cell in cells){
            Debug.Log(cell);
        }

        Debug.Log("Position: " + (position) + " // Direction: " + (direction) + " // OldRotationIndex: " + (rotationIndex) + " // NewRotationIndex: " + (newrotationIndex));

        Vector2Int[] newCells = Data.Cells[data.tetromino][newrotationIndex];

        //create copy of newcells that is vector3Int[]

        Vector3Int[] newCells3 = new Vector3Int[newCells.Length];

        for (int i = 0; i < newCells.Length; i++){
            Vector2Int cell = newCells[i];
            Vector3Int newCell = new Vector3Int(cell.x, cell.y, 0);
            newCells3[i] = newCell;
        }

        Vector2Int[] offsetList;

        if(direction == 1){
            if(data.tetromino == Tetromino.I)
                offsetList = Data.WallKicksI[rotationIndex];
            else
                offsetList = Data.WallKicksJLOSTZ[rotationIndex];
        }
        else{
            if(data.tetromino == Tetromino.I)
                offsetList = Data.CounterWallKicksI[rotationIndex];
            else
                offsetList = Data.CounterWallKicksJLOSTZ[rotationIndex];
        }
        

        // go through each offset, create a copy of the rotated cells, and add the offset to each cell, then check if the new cells are valid
        for (int i = 0; i < offsetList.Length; i++){
            Vector2Int offset = offsetList[i];
            Vector3Int[] rotatedCellsCopy = new Vector3Int[newCells.Length];
            for (int j = 0; j < newCells.Length; j++){
                rotatedCellsCopy[j] = (Vector3Int) newCells[j] + (Vector3Int)offset;
            }
            bool valid = board.IsRotationValid(rotatedCellsCopy, this.position);
            if (valid){
                Debug.Log("Valid" + (offset));

                tspin = false;
                tspinmini = false;

                if(data.tetromino == Tetromino.T){
                    // check for fin and overhang t spin -- if the last kick was used for a T in rotationIndex 2 or 0
                    if((rotationIndex == 2 || rotationIndex == 0) && (i == 4)){
                        tspin = true;
                    }
                    // else if there is 3 corners filled
                    else if(board.TSpinCorners(this.position + (Vector3Int)offset) >= 3){
                        // depending on rotation index, check if the corners the t is facing are filled
                        if (board.TSpinFacing(this.position + (Vector3Int)offset, newrotationIndex) >= 2){
                            tspin = true;
                        }
                        else{
                            tspinmini = true;
                        }
                    }
                }
                
                if(tspin) Debug.Log("TSPIN");
                if(tspinmini) Debug.Log("TSPINMINI");
                this.rotationIndex = newrotationIndex;
                this.cells = newCells3;
                this.position += (Vector3Int)offset;
                // islastactionrotate = true;
                return true;
            }
        }

        return false;

    }
    
    private void HardDrop()
    {
        while (Move(Vector2Int.down))
        {
            continue;
        }

        Lock();
    }

    private void Lock()
    {
        board.Set(this);

        canHold = true;

        // Check for line clears

        board.ClearLines(tspin, tspinmini);
        

        board.SpawnPiece();
    }
    private bool Move(Vector2Int translation)
    {
        Debug.Log("OldPosition" + (position));

        Vector3Int newPosition = position + (Vector3Int)translation;

        Debug.Log("NewPosition" + (newPosition));

        bool valid = board.IsPositionValid(this, newPosition);
        if (valid)
        {
            position = newPosition;
            tspin = false;
            tspinmini = false;
            // islastactionrotate = false;
        }
        return valid;
        
    }
}
