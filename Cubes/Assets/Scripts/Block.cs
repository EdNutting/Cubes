using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBlock
{
    Vector3Int Position { get; }
    bool IsSolid { get;  }
    bool IsVisible { get; }
    int BlockType { get; }
}

public class Block : IBlock
{ 
    public Vector3Int Position { get; set; }
    public bool IsSolid { get; set; }
    public bool IsVisible { get; set; }
    public int BlockType { get; set; }


    public static bool BlockExists(IBlock[,,] blocks, Vector3Int position)
    {
        return position.x >= 0 && position.x < blocks.GetLength(0)
            && position.y >= 0 && position.y < blocks.GetLength(1)
            && position.z >= 0 && position.z < blocks.GetLength(2);
    }

    public static bool IsBlockSolid(IBlock[,,] blocks, Vector3Int position)
    {
        return BlockExists(blocks, position) 
            && blocks[position.x, position.y, position.z].IsSolid;
    }

    public static bool IsBlockVisible(IBlock[,,] blocks, Vector3Int position)
    {
        return BlockExists(blocks, position)
            && blocks[position.x, position.y, position.z].IsVisible;
    }

    public static int GetBlockType(IBlock[,,] blocks, Vector3Int position)
    {
        return BlockExists(blocks, position) ? blocks[position.x, position.y, position.z].BlockType : -1;
    }

    public static BlockUVMap GetBlockUVMap(int blockType)
    {
        return Config.Instance.BlockUVMaps[blockType];
    }
}
