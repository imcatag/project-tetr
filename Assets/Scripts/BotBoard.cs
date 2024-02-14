using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class BotBoard : MonoBehaviour, Attackable
{
    public TetrominoData[] tetrominoes;
    public Tilemap tilemap { get; private set; }
    public BotPiece activePiece { get; private set; }
    public Tetromino heldTetromino { get; private set; }
    public Boolean hasHeld { get; private set; }
    private Vector3Int spawnPosition = new Vector3Int(4, 20, 0);
    private Vector2Int boardSize = new Vector2Int(10, 40);
    public TetrominoData[] bag = new TetrominoData[7];
    private int bagIndex;
    public Queue<TetrominoData> queue = new Queue<TetrominoData>();
    private int BackToBack = 0;
    private int Combo = 0;
    private int totalLines = 0;
    public float DASTime = 0.075f;
    public TextMeshProUGUI totalLinesText;
    public TextMeshProUGUI extraText;
    public TextMeshProUGUI B2BText;
    public TextMeshProUGUI comboText;
    private BagGenerator bagGenerator = new BagGenerator();
    public List<int> damageToDo { get; set; }
    public List<Tetromino> CreateBag()
    {
        int bag = Convert.ToInt32(bagGenerator.mt.Next() % 5040);
        int bagRandom = UnityEngine.Random.Range(0, 5040);

        // get permutation from bagRandom

        return Data.allBags[bag];
    }
    public RectInt Bounds {
        get{
            // make the rectint such that the leftmost and bottommost tile is at (0, 0)
            // Vector2Int position = new Vector2Int(-this.boardSize.x / 2, -this.boardSize.y / 2);
            Vector2Int position = new Vector2Int(0, 0);
            return new RectInt(position, this.boardSize);
        }
    }
    private void Awake()
    {
        Application.targetFrameRate = 0;

        this.tilemap = GetComponentInChildren<Tilemap>();
        this.activePiece = GetComponentInChildren<BotPiece>();
        
        damageToDo = new List<int>();

        for (int i = 0; i < 7; i++) {
            tetrominoes[i].Initialize();
        }

        // get two bags and put them in the queue
        List<Tetromino> bag1 = CreateBag();
        for (int i = 0; i < bag1.Count; i++)
        {
            var td = tetrominoes[(int)bag1[i]];
            queue.Enqueue(td);
        }
    }

    private void Start()
    {
        SpawnPiece();
    }

    public void SpawnPiece()
    {
        // if queue has < 7 pieces, get a new bag and put it in the queue
        if (queue.Count < 7)
        {
            List<Tetromino> bag = CreateBag();
            for (int i = 0; i < bag.Count; i++)
            {
                var td = tetrominoes[(int)bag[i]];
                queue.Enqueue(td);
            }
        }
        var data = queue.Dequeue();

        // display first 5 elements in queue in UI, starting at (12, 18) and going down by 3 each time
        // unset the tiles from 10, 0 to 14, 20
        
        for (int i = 10; i < 15; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                tilemap.SetTile(new Vector3Int(i, j, 0), null);
            }
        }

        for(int i = 0; i < 5; i++)
        {
            // get the tetrominoes
            TetrominoData tetromino = queue.ToArray()[i];
            // get the cells
            Vector2Int[] cells = tetromino.cells;
            // set the tiles
            for (int j = 0; j < cells.Length; j++)
            {
                tilemap.SetTile(new Vector3Int(cells[j].x + 12, cells[j].y + 18 - (i * 3), 0), tetromino.tile);
            }
        }

        // check if there are tiles obstructing the spawn of a new piece

        for(int i = 0; i < data.cells.Length; i++)
        {
            Vector3Int tilePosition = (Vector3Int)data.cells[i] + spawnPosition;
            if (this.tilemap.HasTile(tilePosition))
            {
                Debug.Log("Game Over");
                
                return;
            }
        }

        activePiece.Initialize(this, spawnPosition, data);

        Set(activePiece);
    }

    public void Hold()
    {
        if(!hasHeld)
        {
            hasHeld = true;
            // if there is no held piece, hold the current piece
            heldTetromino = activePiece.data.tetromino;
            // destroy the current piece
            Clear(activePiece);
            // spawn a new piece
            SpawnPiece();
        }
        else
        {
            // if there is a held piece, swap the current piece with the held piece
            TetrominoData temp = activePiece.data;
            activePiece.Initialize(this, spawnPosition, tetrominoes[(int)heldTetromino]);
            heldTetromino = temp.tetromino;
        }

        // display held piece in UI at (-4, 18)
        // unset the tiles from -6, 14 to 1, 20
        for (int i = -6; i < 0; i++)
        {
            for (int j = 14; j < 21; j++)
            {
                tilemap.SetTile(new Vector3Int(i, j, 0), null);
            }
        }

        // get the cells
        Vector2Int[] cells = tetrominoes[(int)heldTetromino].cells;
        // set the tiles
        for (int j = 0; j < cells.Length; j++)
        {
            tilemap.SetTile(new Vector3Int(cells[j].x - 4, cells[j].y + 18, 0), tetrominoes[(int)heldTetromino].tile);
        }

    }

    public void Set(BotPiece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    public void Update()
    {
        // apply damage
        ApplyDamage();
    }

    public void Clear(BotPiece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsPositionValid(BotPiece piece, Vector3Int position)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;
            if (this.tilemap.HasTile(tilePosition))
            {
                return false;
            }

            if (!Bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }

        }

        return true;
    }

    public bool IsRotationValid(Vector3Int[] cells, Vector3Int position)
    {
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3Int tilePosition = cells[i] + position;
            if (this.tilemap.HasTile(tilePosition))
            {
                return false;
            }

            if (!Bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }

        }

        return true;
    }

    public int ClearLines(bool tspin, bool tspinmini)
    {
        // go through each row and check if it is full

        // if it is full, clear the row and move all rows above it down

        // return number of lines cleared

        int linesCleared = 0;
        bool allClear = true;

        for (int y = Bounds.yMin; y < Bounds.yMax; y++)
        {
            bool rowFull = true;
            bool atLeastOne = false;

            for (int x = Bounds.xMin; x < Bounds.xMax; x++)
            {
                if (!this.tilemap.HasTile(new Vector3Int(x, y, 0)))
                {
                    rowFull = false;
                }
                else
                    atLeastOne = true;
            }

            if(atLeastOne && !rowFull) allClear = false;

            if (rowFull)
            {
                linesCleared++;

                for (int x = Bounds.xMin; x < Bounds.xMax; x++)
                {
                    this.tilemap.SetTile(new Vector3Int(x, y, 0), null);
                }

                for (int aboveY = y + 1; aboveY < Bounds.yMax; aboveY++)
                {
                    for (int x = Bounds.xMin; x < Bounds.xMax; x++)
                    {
                        TileBase tile = this.tilemap.GetTile(new Vector3Int(x, aboveY, 0));
                        if (tile != null)
                        {
                            this.tilemap.SetTile(new Vector3Int(x, aboveY - 1, 0), tile);
                            this.tilemap.SetTile(new Vector3Int(x, aboveY, 0), null);
                        }
                    }
                }

                y--;
            }
        }

        if (linesCleared > 0)
        {
            Combo++;
            if(Combo > 1){
                comboText.text = "Combo: " + Combo.ToString();
                comboText.CrossFadeAlpha(1, 0, false);
                comboText.CrossFadeAlpha(0, 2, false);
            }

            if (tspin)
            {
                switch (linesCleared){
                    case 1:
                        extraText.text = "T-Spin Single";
                        break;
                    case 2:
                        extraText.text = "T-Spin Double";
                        break;
                    case 3:
                        extraText.text = "T-Spin Triple";
                        break;
                    default:
                        extraText.text = "T-Spin " + linesCleared.ToString() + " Lines";
                        break;
                }
                BackToBack++;
            }
            else if (tspinmini)
                {
                    switch (linesCleared){
                        case 1:
                            extraText.text = "T-Spin Mini Single";
                            break;
                        case 2:
                            extraText.text = "T-Spin Mini Double";
                            break;
                        case 3:
                            extraText.text = "T-Spin Mini Triple";
                            break;
                        default:
                            extraText.text = "T-Spin Mini " + linesCleared.ToString() + " Lines";
                            break;
                    }
                    BackToBack++;
                }
            else
            {
                switch (linesCleared){
                    case 1:
                        extraText.text = "Single";
                        BackToBack = 0;
                        break;
                    case 2:
                        extraText.text = "Double";
                        BackToBack = 0;
                        break;
                    case 3:
                        extraText.text = "Triple";
                        BackToBack = 0;
                        break;
                    case 4:
                        extraText.text = "Quad";
                        BackToBack++;
                        break;
                    default:
                        extraText.text = linesCleared.ToString() + " Lines";
                        break;
                }
            }
            // over the next 2 seconds, change the alpha of extraText from 1 to 0
            extraText.CrossFadeAlpha(1, 0, false);
            extraText.CrossFadeAlpha(0, 2, false);

            if(allClear) Debug.Log("ALL CLEAR");

            if(BackToBack > 0){
                B2BText.text = "B2B: " + BackToBack.ToString();
            }
            else{
                B2BText.text = "";
            }

            totalLines += linesCleared;
            totalLinesText.text = "Lines: " + totalLines.ToString();
        }
        else
        {
            if(tspinmini){
                extraText.text = "T-Spin Mini";
                extraText.CrossFadeAlpha(1, 0, false);
                extraText.CrossFadeAlpha(0, 2, false);
            }
            else if(tspin){
                extraText.text = "T-Spin";
                extraText.CrossFadeAlpha(1, 0, false);
                extraText.CrossFadeAlpha(0, 2, false);
            }
            
            Combo = 0;
        }
        

        

        return linesCleared;
    }

    public int TSpinCorners(Vector3Int position){
        var offsets = new Vector2Int [] {new Vector2Int(-1, 1), new Vector2Int(1, 1), new Vector2Int(1, -1), new Vector2Int(-1, -1)};
        int cornercount = 0;
        for (int i = 0; i < offsets.Length; i++){
            // if there is a tile in the corner
            if (this.tilemap.HasTile(position + (Vector3Int)offsets[i])){
                cornercount++;
            }
            // or if corner is out of border
            else if(!Bounds.Contains((Vector2Int)(position + (Vector3Int)offsets[i]))){
                cornercount++;
            }
        }
        return cornercount;
    }

    public int TSpinFacing(Vector3Int position, int rotationIndex){
        Dictionary<int, Vector2Int[]> dictCorners = new Dictionary<int, Vector2Int[]>{
            {0, new Vector2Int[] {new Vector2Int(-1, 1), new Vector2Int(1, 1)}},
            {1, new Vector2Int[] {new Vector2Int(1, 1), new Vector2Int(1, -1)}},
            {2, new Vector2Int[] {new Vector2Int(1, -1), new Vector2Int(-1, -1)}},
            {3, new Vector2Int[] {new Vector2Int(-1, -1), new Vector2Int(-1, 1)}}
        };

        int facingcount = 0;
        Vector2Int[] corners = dictCorners[rotationIndex];
        foreach(var Corner in corners){
            // if there is a tile in the corner
            if (this.tilemap.HasTile(position + (Vector3Int)Corner)){
                facingcount++;
            }
            // or if corner is out of border
            else if(!Bounds.Contains((Vector2Int)(position + (Vector3Int)Corner))){
                facingcount++;
            }
        }

        return facingcount;
    }

    public bool Collides(Vector3Int[] cells, Vector3Int position, Piece exceptionPiece){
        for (int i = 0; i < cells.Length; i++){
            Vector3Int tilePosition = cells[i] + position;

            bool excepts = false;
            for(int j = 0; j < exceptionPiece.cells.Length; j++){
                if(tilePosition == exceptionPiece.cells[j] + exceptionPiece.position){
                    excepts = true;
                }
            }

            if(excepts) continue;
            
            if (this.tilemap.HasTile(tilePosition))
            {
                // Debug.Log("Collides at" + (tilePosition));
                return true;
            }

            if (!Bounds.Contains((Vector2Int)tilePosition))
            {
                return true;
            }
        }
        return false;
    }

    
    void AddGarbage(int lines)
    {
        int hole = Random.Range(0, 9);
        // move everything up by lines, top to bottom
        for (int y = Bounds.yMax - lines; y >= Bounds.yMin; y--)
        {
            for (int x = Bounds.xMin; x < Bounds.xMax; x++)
            {
                TileBase tile = this.tilemap.GetTile(new Vector3Int(x, y - lines, 0));
                this.tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
        
        // add the garbage
        for (int y = Bounds.yMin; y < Bounds.yMin + lines; y++)
        {
            for (int x = Bounds.xMin; x < Bounds.xMax; x++)
            {
                Debug.Log("Adding garbage at " + x + ", " + y);
                if (x != hole)
                {
                    this.tilemap.SetTile(new Vector3Int(x, y, 0), tetrominoes[(int)Tetromino.NullTetromino].tile);
                }
            }
        }
    }
    public void TakeDamage(int damage)
    {
        damageToDo.Add(damage);
    }

    public void ApplyDamage()
    {
        // apply up to 8 lines at once
        int doableDamage = 8;
        while(damageToDo.Count > 0 && doableDamage > 0)
        {
            // apply min(damageQueue.first, doableDamage) lines
            if (damageToDo[0] <= doableDamage)
            {
                doableDamage -= damageToDo[0];
                AddGarbage(damageToDo[0]);
                damageToDo.RemoveAt(0);
            }
            else
            {
                damageToDo[0] -= doableDamage;
                AddGarbage(doableDamage);
                doableDamage = 0;
                
            }
        }
    }
}

