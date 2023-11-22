using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Ghost : MonoBehaviour
{
    // Start is called before the first frame update

    public Board board { get; private set; }
    public Tilemap ghostmap { get; private set; }

    public GhostPiece ghostpiece { get; private set; }
    // public GhostPiece lastpiece { get; private set; }

    void Awake()
    {
        ghostmap = GetComponentInChildren<Tilemap>();

    }
    public void Initialize(Board board)
    {
        this.board = board;
    }

    public void UpdateGhost(Piece piece){

        this.ghostpiece = GetComponentInChildren<GhostPiece>();

        // create copy of piece
        ghostpiece.Initialize(board, this, piece);

        // update ghostmap to match ghostpiece

        ghostmap.ClearAllTiles();

        foreach(Vector3Int cell in ghostpiece.cells){
            ghostmap.SetTile(cell + ghostpiece.position, piece.data.tile);
        }

        
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
