using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBlock
{
    Vector3Int pos { get; }
    bool isSolid { get;  }
    bool isVisible { get; }
    int blockType { get; }
}

public class Block : IBlock
{ 
    public Vector3Int pos { get; set; }
    public bool isSolid { get; set; }
    public bool isVisible { get; set; }
    public int blockType { get; set; }


    public static bool BlockExists(IBlock[,,] blocks, Vector3Int pos)
    {
        return pos.x >= 0 && pos.x < blocks.GetLength(0)
            && pos.y >= 0 && pos.y < blocks.GetLength(1)
            && pos.z >= 0 && pos.z < blocks.GetLength(2);
    }

    public static bool IsBlockSolid(IBlock[,,] blocks, Vector3Int pos)
    {
        return BlockExists(blocks, pos) 
            && blocks[pos.x, pos.y, pos.z].isSolid;
    }

    public static bool IsBlockVisible(IBlock[,,] blocks, Vector3Int pos)
    {
        return BlockExists(blocks, pos)
            && blocks[pos.x, pos.y, pos.z].isVisible;
    }

    public static int GetBlockType(IBlock[,,] blocks, Vector3Int pos)
    {
        return BlockExists(blocks, pos) ? blocks[pos.x, pos.y, pos.z].blockType : -1;
    }

    public static BlockUVMap GetBlockUVMap(int blockType)
    {
        // 1 -- 3
        // |    |
        // 0 -- 2

        int textureCols = 2;
        int textureRows = 2;

        float xShift = 1.0f / textureCols;
        float yShift = 1.0f / textureRows;

        return new BlockUVMap
        {
            top = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(0, yShift),
                new Vector2(xShift, 0),
                new Vector2(xShift, yShift)
            },

            bottom = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(0, yShift),
                new Vector2(xShift, 0),
                new Vector2(xShift, yShift)
            },

            left = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(0, yShift),
                new Vector2(xShift, 0),
                new Vector2(xShift, yShift)
            },

            right = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(0, yShift),
                new Vector2(xShift, 0),
                new Vector2(xShift, yShift)
            },

            front = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(0, yShift),
                new Vector2(xShift, 0),
                new Vector2(xShift, yShift)
            },

            back = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(0, yShift),
                new Vector2(xShift, 0),
                new Vector2(xShift, yShift)
            }
        };
    }
}
