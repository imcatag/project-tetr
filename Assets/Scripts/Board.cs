using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public class Board : MonoBehaviour
{
    public TetrominoData[] tetrominoes;
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }
    public Ghost ghost { get; private set; }
    public Tetromino heldTetromino { get; private set; }
    public Boolean hasHeld { get; private set; }
    private Vector3Int spawnPosition = new Vector3Int(4, 19, 0);
    private Vector2Int boardSize = new Vector2Int(10, 40);
    public TetrominoData[] bag = new TetrominoData[7];
    public int bagIndex = 0;
    public Queue<TetrominoData> queue = new Queue<TetrominoData>();
    public List<Tetromino> CreateBag()
    {
        int bagRandom = UnityEngine.Random.Range(0, 5040);
        // get permutation from bagRandom

        return Data.allBags[bagRandom];
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

        ghost = FindObjectOfType<Ghost>();

        ghost.Initialize(this);

        this.tilemap = GetComponentInChildren<Tilemap>();
        this.activePiece = GetComponentInChildren<Piece>();

        for (int i = 0; i < tetrominoes.Length; i++) {
            tetrominoes[i].Initialize();
        }

        // get two bags and put them in the queue
        List<Tetromino> bag1 = CreateBag();
        List<Tetromino> bag2 = CreateBag();
        for (int i = 0; i < bag1.Count; i++)
        {
            queue.Enqueue(tetrominoes[(int)bag1[i]]);
        }
        for (int i = 0; i < bag2.Count; i++)
        {
            queue.Enqueue(tetrominoes[(int)bag2[i]]);
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
            List<Tetromino> newBag = CreateBag();
            for (int i = 0; i < newBag.Count; i++)
            {
                queue.Enqueue(tetrominoes[(int)newBag[i]]);
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

    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }

        ghost.UpdateGhost(piece);
    }

    public void Update()
    {
        // pressing R reloads the scene
        if (Input.GetKeyDown(KeyCode.R))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsPositionValid(Piece piece, Vector3Int position)
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
            if (tspin)
            {
                if (tspinmini)
                {
                    Debug.Log("T-Spin Mini " + linesCleared + " lines");
                }
                else
                {
                    Debug.Log("T-Spin " + linesCleared + " lines");
                }
            }
            else
            {
                Debug.Log("Clear " + linesCleared + " lines");
            }

            if(allClear) Debug.Log("ALL CLEAR");
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
    
}

