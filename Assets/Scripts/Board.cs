using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

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
        Application.targetFrameRate = 144;

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
}
