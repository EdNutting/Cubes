using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBlock
{
    public Vector3Int pos { get; }
    public bool isSolid { get;  }
}

public class Block : IBlock
{ 
    public Vector3Int pos { get; set; }
    public bool isSolid { get; set; }
}
