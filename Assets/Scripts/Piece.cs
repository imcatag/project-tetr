using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public Vector3Int position { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; set; }
    public int rotationIndex { get; set; }
    public bool tspin { get; private set; }
    public bool tspinmini { get; private set; }
    public bool canHold { get; private set; }
    public float autoFallTime { get; private set; }
    public int autoFalls { get; private set; }
    public float noFallTime { get; private set; }
    public float totalNoFallTime { get; private set; }

    [SerializeField]
    private float autoFallInterval;
    private Dictionary<KeyCode, float> startedPressing = new Dictionary<KeyCode, float>();
    private Dictionary<KeyCode, bool> contPressed = new Dictionary<KeyCode, bool>();
    private Dictionary<KeyCode, Vector2Int> keyToVector = new Dictionary<KeyCode, Vector2Int>();
    private Dictionary<KeyCode, float> DASStart = new Dictionary<KeyCode, float>();
    public void Awake()
    {
        canHold = true;
        fillDicts();
        autoFallTime = Time.time;
        autoFalls = 0;
    }

    private void fillDicts()
    {
        startedPressing.Add(KeyCode.LeftArrow, 0f);
        startedPressing.Add(KeyCode.RightArrow, 0f);
        startedPressing.Add(KeyCode.DownArrow, 0f);
        startedPressing.Add(KeyCode.Space, 0f);
        startedPressing.Add(KeyCode.A, 0f);
        startedPressing.Add(KeyCode.D, 0f);
        startedPressing.Add(KeyCode.S, 0f);
        startedPressing.Add(KeyCode.LeftShift, 0f);

        contPressed.Add(KeyCode.LeftArrow, false);
        contPressed.Add(KeyCode.RightArrow, false);
        contPressed.Add(KeyCode.DownArrow, false);
        contPressed.Add(KeyCode.Space, false);
        contPressed.Add(KeyCode.A, false);
        contPressed.Add(KeyCode.D, false);
        contPressed.Add(KeyCode.S, false);
        contPressed.Add(KeyCode.LeftShift, false);

        keyToVector.Add(KeyCode.LeftArrow, Vector2Int.left);
        keyToVector.Add(KeyCode.RightArrow, Vector2Int.right);

        DASStart.Add(KeyCode.LeftArrow, 0f);
        DASStart.Add(KeyCode.RightArrow, 0f);
    }
    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        autoFallTime = Time.time;
        autoFalls = 0;
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
    private void setKeyProperties(KeyCode kc)
    {
        if(Input.GetKeyDown(kc) && !contPressed[kc])
        {
            Move(keyToVector[kc]);
            startedPressing[kc] = Time.time;
            DASStart[kc] = Time.time + board.DASTime;
            contPressed[kc] = true;
        }
        if(Input.GetKeyUp(kc))
        {
            contPressed[kc] = false;
        }
    }
    private void checkMove()
    {
        
        setKeyProperties(KeyCode.LeftArrow);
        setKeyProperties(KeyCode.RightArrow);


        if(Time.time > DASStart[KeyCode.LeftArrow] && contPressed[KeyCode.LeftArrow] && Time.time > DASStart[KeyCode.RightArrow] && contPressed[KeyCode.RightArrow])
        {
            // Whichever DASStart is later gets priority
            if(DASStart[KeyCode.LeftArrow] > DASStart[KeyCode.RightArrow])
            {
                while(Move(Vector2Int.left))
                {
                    continue;
                }
            }
            else
            {
                while(Move(Vector2Int.right))
                {
                    continue;
                }
            }
        }
        // else check individual DASStarts
        else if(Time.time > DASStart[KeyCode.LeftArrow] && contPressed[KeyCode.LeftArrow])
        {
            while(Move(Vector2Int.left))
            {
                continue;
            }
            // If the other key is pressed, move once in that direction
            if(contPressed[KeyCode.RightArrow])
            {
                Move(Vector2Int.right);
            }

        }
        else if(Time.time > DASStart[KeyCode.RightArrow] && contPressed[KeyCode.RightArrow])
        {
            while(Move(Vector2Int.right))
            {
                continue;
            }
            // If the other key is pressed, move once in that direction
            if(contPressed[KeyCode.LeftArrow])
            {
                Move(Vector2Int.left);
            }
        }
    }
    public void Update()
    {
        if (board == null) return;

        board.Clear(this);
        checkMove();

        if (board.frozen)
        {
            board.Set(this);
            return;
        };
        
        if(!board.IsPositionValid(this, position + Vector3Int.down)) // if not falling
        {
            autoFallTime = Time.time;
            totalNoFallTime += Time.deltaTime;
            if(noFallTime + autoFallInterval < Time.time || totalNoFallTime > autoFallInterval * 2)
            {
                Lock();
            }
            autoFalls = 0;
        }
        else // if falling
        {
            noFallTime = Time.time;
            if(autoFallTime + autoFallInterval * (autoFalls + 1) < Time.time)
            {
                Move(Vector2Int.down);
                autoFalls++;
            }
        }


        if (Input.GetKey(KeyCode.DownArrow))
        {
            while(Move(Vector2Int.down)) // personal settings - using infinite soft drop for now
            {
                // reset fall variables
                autoFallTime = Time.time;
                autoFalls = 0;
                noFallTime = Time.time;
            }
            
            // Move(Vector2Int.down);
        }
        
        if (Input.GetKeyDown(KeyCode.A))
        {
            Rotate(-1);
        }

        if(Input.GetKeyDown(KeyCode.D))
        {
            Rotate(1);
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            Flip();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }
        else if(Input.GetKeyDown(KeyCode.LeftShift))
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
    private bool Flip(){
        var newrotationIndex = Wrap(this.rotationIndex + 2, 0, 4);

        Vector2Int[] newCells = Data.Cells[data.tetromino][newrotationIndex];

        //create copy of newcells that is vector3Int[]

        Vector3Int[] newCells3 = new Vector3Int[newCells.Length];

        for (int i = 0; i < newCells.Length; i++){
            Vector2Int cell = newCells[i];
            Vector3Int newCell = new Vector3Int(cell.x, cell.y, 0);
            newCells3[i] = newCell;
        }

        Vector2Int[] offsetList = Data.Flips[rotationIndex];

        // go through each offset, create a copy of the rotated cells, and add the offset to each cell, then check if the new cells are valid

        for (int i = 0; i < offsetList.Length; i++){
            Vector2Int offset = offsetList[i];
            Vector3Int[] rotatedCellsCopy = new Vector3Int[newCells.Length];
            for (int j = 0; j < newCells.Length; j++){
                rotatedCellsCopy[j] = (Vector3Int) newCells[j] + (Vector3Int)offset;
            }
            bool valid = board.IsRotationValid(rotatedCellsCopy, this.position);
            if (valid){
                // Debug.Log("Valid" + (offset));
                this.rotationIndex = newrotationIndex;
                this.cells = newCells3;
                this.position += (Vector3Int)offset;
                // islastactionrotate = true;
                return true;
            }
        }

        return false;
    }
    private bool Rotate (int direction){

        var newrotationIndex = Wrap(this.rotationIndex + direction, 0, 4);

        // Debug.Log("Position: " + (position) + " // Direction: " + (direction) + " // OldRotationIndex: " + (rotationIndex) + " // NewRotationIndex: " + (newrotationIndex));

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
                // Debug.Log("Valid" + (offset));

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
                
                // if(tspin) Debug.Log("TSPIN");
                // if(tspinmini) Debug.Log("TSPINMINI");
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
        board.totalPieces++;
        board.Set(this);
        canHold = true;

        // Check for line clears

        var cleared = board.ClearLines(tspin, tspinmini);
        
        // If no lines were cleared, apply damage
        if(cleared == 0)
        {
            board.ApplyDamage();
        }

        autoFallTime = Time.time;
        autoFalls = 0;
        noFallTime = Time.time;
        totalNoFallTime = 0;
        
        board.SpawnPiece();
    }
    public bool Move(Vector2Int translation)
    {
        // Debug.Log("OldPosition" + (position));

        Vector3Int newPosition = position + (Vector3Int)translation;

        // Debug.Log("NewPosition" + (newPosition));

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
