using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public class Board : MonoBehaviour
{
    public TetrominoData[] tetrominoes;
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }
    public Vector3Int spawnPosition = new Vector3Int(0, 0, 0);
    public Vector2Int boardSize = new Vector2Int(10, 20);
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
            Vector2Int position = new Vector2Int(-this.boardSize.x / 2, -this.boardSize.y / 2);
            return new RectInt(position, this.boardSize);
        }
    }
    private void Awake()
    {
        Application.targetFrameRate = 0;

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
        activePiece.Initialize(this, spawnPosition, data);

        Set(activePiece);
    }

    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
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
}
