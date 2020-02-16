using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    GameObject[] Chunks;
    public GameObject BaseChunk;

    public IBlock[,,] Blocks;

    void Start()
    {
        GenerateWorld();
    }

    void GenerateWorld()
    {
        GenerateBlockData();
        GenerateChunks();
    }

    void GenerateBlockData()
    {
        Blocks = new IBlock[ Config.ChunkCount * Config.ChunkWidth
                           , Config.ChunkCount * Config.ChunkHeight
                           , Config.ChunkCount * Config.ChunkDepth
                           ];

        for (int x = 0; x < Config.ChunkCount * Config.ChunkWidth; x++)
        {
            for (int y = 0; y < Config.ChunkCount * Config.ChunkHeight; y++)
            {
                for (int z = 0; z < Config.ChunkCount * Config.ChunkDepth; z++)
                {
                    Blocks[x, y, z] = new Block();
                }
            }
        }
    }

    void GenerateChunks()
    {
        Chunks = new GameObject[Config.ChunkCount];
        for (int i = 0; i < Config.ChunkCount; i++)
        {
            int x = i;
            int y = 0;
            int z = 0;

            var chunk = GameObject.Instantiate(BaseChunk, new Vector3(x * Config.ChunkWidth, y * Config.ChunkHeight, z * Config.ChunkDepth), Quaternion.identity);
            Chunks[i] = chunk;

            chunk.gameObject.name = string.Format("Chunk {0}, {1}, {2}", x, y, z);
        }
    }
}
