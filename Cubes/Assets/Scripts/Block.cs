using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBlock
{
    Vector3Int pos { get; }
    bool isSolid { get;  }
}

public class Block : IBlock
{ 
    public Vector3Int pos { get; set; }
    public bool isSolid { get; set; }


    public static bool IsBlockSolid(IBlock[,,] blocks, Vector3Int pos)
    {
        if (  pos.x < 0 || pos.x >= blocks.GetLength(0) 
           || pos.y < 0 || pos.y >= blocks.GetLength(1)
           || pos.z < 0 || pos.z >= blocks.GetLength(2))
        {
            return false;
        }

        IBlock block = blocks[pos.x, pos.y, pos.z];
        return block.isSolid;
    }
}
