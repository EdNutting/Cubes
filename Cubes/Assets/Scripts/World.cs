using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    GameObject[,,] Chunks;
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

        int bT = 0;
        for (int x = 0; x < Config.ChunkCount * Config.ChunkWidth; x++)
        {
            for (int y = 0; y < Config.ChunkCount * Config.ChunkHeight; y++)
            {
                for (int z = 0; z < Config.ChunkCount * Config.ChunkDepth; z++)
                {
                    Blocks[x, y, z] = new Block() { isSolid = true, isVisible = x % 3 == 0, blockType = bT };
                    bT = (bT + 1) % 4;
                }
            }
        }
    }

    void GenerateChunks()
    {
        Chunks = new GameObject[Config.ChunkCount, Config.ChunkCount, Config.ChunkCount];
        for (int cX = 0; cX < Config.ChunkCount; cX++)
        {
            for (int cY = 0; cY < Config.ChunkCount; cY++)
            {
                for (int cZ = 0; cZ < Config.ChunkCount; cZ++)
                {
                    int x = cX * Config.ChunkWidth;
                    int y = cY * Config.ChunkHeight;
                    int z = cZ * Config.ChunkDepth;

                    var chunk = GameObject.Instantiate(BaseChunk, new Vector3(x, y, z), Quaternion.identity);
                    Chunks[cX, cY, cZ] = chunk;

                    Chunk chunkComp = chunk.GetComponent<Chunk>();
                    chunkComp.world = this;
                    chunkComp.position = new Vector3Int(x, y, z);

                    chunk.gameObject.name = string.Format("Chunk {0}, {1}, {2}", cX, cY, cZ);
                }
            }
        }
    }
}
