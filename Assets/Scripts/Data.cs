using System.Collections.Generic;
using System.ComponentModel;
using Unity.Properties;
using UnityEngine;

public static class Data
{

    public static List<List<Tetromino>> allBags = generateBags();

    private static void GeneratePermutations(List<Tetromino> tetrominoes, int index, List<List<Tetromino>> permutations)
    {
        if (index == tetrominoes.Count - 1)
        {
            permutations.Add(new List<Tetromino>(tetrominoes));
            return;
        }

        for (int i = index; i < tetrominoes.Count; i++)
        {
            // Swap elements at index and i
            Tetromino temp = tetrominoes[index];
            tetrominoes[index] = tetrominoes[i];
            tetrominoes[i] = temp;

            // Recursively generate permutations for the remaining elements
            GeneratePermutations(tetrominoes, index + 1, permutations);

            // Swap elements back to backtrack
            temp = tetrominoes[index];
            tetrominoes[index] = tetrominoes[i];
            tetrominoes[i] = temp;
        }
    }

    public static List<List<Tetromino>> generateBags()
    {       
        List<List<Tetromino>> allBags = new List<List<Tetromino>>();

        GeneratePermutations(new List<Tetromino>() { Tetromino.I, Tetromino.J, Tetromino.L, Tetromino.O, Tetromino.S, Tetromino.T, Tetromino.Z }, 0, allBags);

        return allBags;
    }

    // public static readonly Dictionary<Tetromino, Vector2Int[]> Cells = new Dictionary<Tetromino, Vector2Int[]>()
    // {
    //     { Tetromino.I, new Vector2Int[] { new Vector2Int(-1, 0), new Vector2Int( 0, 0), new Vector2Int( 1, 0), new Vector2Int( 2, 0) } },
    //     { Tetromino.J, new Vector2Int[] { new Vector2Int(-1, 1), new Vector2Int(-1, 0), new Vector2Int( 0, 0), new Vector2Int( 1, 0) } },
    //     { Tetromino.L, new Vector2Int[] { new Vector2Int( 1, 1), new Vector2Int(-1, 0), new Vector2Int( 0, 0), new Vector2Int( 1, 0) } },
    //     { Tetromino.O, new Vector2Int[] { new Vector2Int( 0, 1), new Vector2Int( 1, 1), new Vector2Int( 0, 0), new Vector2Int( 1, 0) } },
    //     { Tetromino.S, new Vector2Int[] { new Vector2Int( 0, 1), new Vector2Int( 1, 1), new Vector2Int(-1, 0), new Vector2Int( 0, 0) } },
    //     { Tetromino.T, new Vector2Int[] { new Vector2Int( 0, 1), new Vector2Int(-1, 0), new Vector2Int( 0, 0), new Vector2Int( 1, 0) } },
    //     { Tetromino.Z, new Vector2Int[] { new Vector2Int(-1, 1), new Vector2Int( 0, 1), new Vector2Int( 0, 0), new Vector2Int( 1, 0) } },
    // };
    public static readonly Dictionary<Tetromino, List<Vector2Int[]>> Cells = new Dictionary<Tetromino, List<Vector2Int[]>>()
    {
        { Tetromino.I, new List<Vector2Int[]>
            {
                new Vector2Int[] { new Vector2Int(-1, 1), new Vector2Int( 0, 1), new Vector2Int( 1, 1), new Vector2Int( 2, 1) },
                new Vector2Int[] { new Vector2Int(1, -1), new Vector2Int( 1, 0), new Vector2Int( 1, 1), new Vector2Int( 1, 2) },
                new Vector2Int[] { new Vector2Int(-1, 0), new Vector2Int( 0, 0), new Vector2Int( 1, 0), new Vector2Int( 2, 0) },
                new Vector2Int[] { new Vector2Int(0, -1), new Vector2Int( 0, 0), new Vector2Int( 0, 1), new Vector2Int( 0, 2) }
            }
        },
        { Tetromino.J, new List<Vector2Int[]>
            {
                new Vector2Int[] { new Vector2Int(-1, 1), new Vector2Int(-1, 0), new Vector2Int( 0, 0), new Vector2Int( 1, 0) },
                new Vector2Int[] { new Vector2Int(1, 1), new Vector2Int(0, 1), new Vector2Int(0, 0), new Vector2Int(0, -1) },
                new Vector2Int[] { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, -1) },
                new Vector2Int[] { new Vector2Int(0, 1), new Vector2Int(0, 0), new Vector2Int(0, -1), new Vector2Int(-1, -1) }
            }
        },
        { Tetromino.L, new List<Vector2Int[]>
            {
                new Vector2Int[] { new Vector2Int(1, 1), new Vector2Int(-1, 0), new Vector2Int( 0, 0), new Vector2Int( 1, 0) },
                new Vector2Int[] { new Vector2Int(1, -1), new Vector2Int(0, -1), new Vector2Int(0, 0), new Vector2Int(0, 1) },
                new Vector2Int[] { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(-1, -1) },
                new Vector2Int[] { new Vector2Int(0, -1), new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(-1, 1) }
            }
        },
        { Tetromino.O, new List<Vector2Int[]>
            {
                new Vector2Int[] { new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int( 0, 0), new Vector2Int( 1, 0) },
                new Vector2Int[] { new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int( 0, 0), new Vector2Int( 1, 0) },
                new Vector2Int[] { new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int( 0, 0), new Vector2Int( 1, 0) },
                new Vector2Int[] { new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int( 0, 0), new Vector2Int( 1, 0) }
            }
        },
        { Tetromino.S, new List<Vector2Int[]>
            {
                new Vector2Int[] { new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(-1, 0), new Vector2Int( 0, 0) },
                new Vector2Int[] { new Vector2Int(0, 1), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, -1) },
                new Vector2Int[] { new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(0, 0), new Vector2Int(1, 0) },
                new Vector2Int[] { new Vector2Int(-1, 1), new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(0, -1) },
            }
        },
        { Tetromino.T, new List<Vector2Int[]>
            {
                new Vector2Int[] { new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int( 0, 0), new Vector2Int( 1, 0) },
                new Vector2Int[] { new Vector2Int(1, 0), new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, -1) },
                new Vector2Int[] { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, -1) },
                new Vector2Int[] { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, -1) }
            }
        },
        { Tetromino.Z, new List<Vector2Int[]>
            {
                new Vector2Int[] { new Vector2Int(-1, 1), new Vector2Int( 0, 1), new Vector2Int( 0, 0), new Vector2Int( 1, 0) },
                new Vector2Int[] { new Vector2Int(1, 1), new Vector2Int(1, 0), new Vector2Int(0, 0), new Vector2Int(0, -1) },
                new Vector2Int[] { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(0, -1), new Vector2Int(1, -1) },
                new Vector2Int[] { new Vector2Int(-1, -1), new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(0, 1) }
            }
        }
    };


    // public static readonly Vector2Int[,] WallKicksI = new Vector2Int[,] {
    //     { new Vector2Int(0, 0), new Vector2Int(-2, 0), new Vector2Int( 1, 0), new Vector2Int(-2,-1), new Vector2Int( 1, 2) }, // 0 -> 1
    //     { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int( 2, 0), new Vector2Int(-1, 2), new Vector2Int( 2,-1) }, // 1 -> 2
    //     { new Vector2Int(0, 0), new Vector2Int( 2, 0), new Vector2Int(-1, 0), new Vector2Int( 2, 1), new Vector2Int(-1,-2) }, // 2 -> 3
    //     { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int(-2, 0), new Vector2Int( 1,-2), new Vector2Int(-2, 1) }, // 3 -> 0
    // };

    // public static readonly Vector2Int[,] CounterWallKicksI = new Vector2Int[,] {
    //     { new Vector2Int(0, 0), new Vector2Int( 2, 0), new Vector2Int(-1, 0), new Vector2Int( 2, 1), new Vector2Int(-1,-2) }, // 1 -> 0
    //     { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int(-2, 0), new Vector2Int( 1,-2), new Vector2Int(-2, 1) }, // 2 -> 1
    //     { new Vector2Int(0, 0), new Vector2Int(-2, 0), new Vector2Int( 1, 0), new Vector2Int(-2,-1), new Vector2Int( 1, 2) }, // 3 -> 2
    //     { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int( 2, 0), new Vector2Int(-1, 2), new Vector2Int( 2,-1) }, // 0 -> 3
    // };


    // public static readonly Vector2Int[,] WallKicksJLOSTZ = new Vector2Int[,] {
    //     { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(0,-2), new Vector2Int(-1,-2) }, // 0 -> 1
    //     { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int( 1,-1), new Vector2Int(0, 2), new Vector2Int( 1, 2) }, // 1 -> 2
    //     { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int( 1, 1), new Vector2Int(0,-2), new Vector2Int( 1,-2) }, // 2 -> 3
    //     { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1,-1), new Vector2Int(0, 2), new Vector2Int(-1, 2) }, // 3 -> 0
    // };

    // public static readonly Vector2Int[,] CounterWallKicksJLOSTZ = new Vector2Int[,]{
    //     { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int( 1,-1), new Vector2Int(0, 2), new Vector2Int( 1, 2) }, // 1 -> 0
    //     { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(0,-2), new Vector2Int(-1,-2) }, // 2 -> 1
    //     { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1,-1), new Vector2Int(0, 2), new Vector2Int(-1, 2) }, // 3 -> 2
    //     { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int( 1, 1), new Vector2Int(0,-2), new Vector2Int( 1,-2) }, // 0 -> 3
    // };

    public static readonly List<Vector2Int[]> WallKicksI = new List<Vector2Int[]> {
        new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(-2, 0), new Vector2Int(1, 0), new Vector2Int(-2, -1), new Vector2Int(1, 2) }, // 0 -> 1
        new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(2, 0), new Vector2Int(-1, 2), new Vector2Int(2, -1) }, // 1 -> 2
        new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(2, 0), new Vector2Int(-1, 0), new Vector2Int(2, 1), new Vector2Int(-1, -2) }, // 2 -> 3
        new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(-2, 0), new Vector2Int(1, -2), new Vector2Int(-2, 1) }, // 3 -> 0
    };

    public static readonly List<Vector2Int[]> CounterWallKicksI = new List<Vector2Int[]> {
        new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(2, 0), new Vector2Int(-1, 2), new Vector2Int(2, -1) }, // 0 -> 3
        new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(2, 0), new Vector2Int(-1, 0), new Vector2Int(2, 1), new Vector2Int(-1, -2) }, // 1 -> 0
        new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(-2, 0), new Vector2Int(1, -2), new Vector2Int(-2, 1) }, // 2 -> 1
        new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(-2, 0), new Vector2Int(1, 0), new Vector2Int(-2, -1), new Vector2Int(1, 2) }, // 3 -> 2
        
    };

    public static readonly List<Vector2Int[]> WallKicksJLOSTZ = new List<Vector2Int[]> {
        new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(0, -2), new Vector2Int(-1, -2) }, // 0 -> 1
        new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, -1), new Vector2Int(0, 2), new Vector2Int(1, 2) }, // 1 -> 2
        new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(0, -2), new Vector2Int(1, -2) }, // 2 -> 3
        new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1, -1), new Vector2Int(0, 2), new Vector2Int(-1, 2) }, // 3 -> 0
    };

    public static readonly List<Vector2Int[]> CounterWallKicksJLOSTZ = new List<Vector2Int[]> {
        new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(0, -2), new Vector2Int(1, -2) }, // 0 -> 3
        new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, -1), new Vector2Int(0, 2), new Vector2Int(1, 2) }, // 1 -> 0
        new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(0, -2), new Vector2Int(-1, -2) }, // 2 -> 1
        new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1, -1), new Vector2Int(0, 2), new Vector2Int(-1, 2) }, // 3 -> 2
        
    };

}