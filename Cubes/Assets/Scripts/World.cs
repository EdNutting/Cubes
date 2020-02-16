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
        int GlobalChunkWidth = Config.ChunkCount * Config.ChunkWidth;
        int GlobalChunkHeight = Config.ChunkCount * Config.ChunkHeight;
        int GlobalChunkDepth = Config.ChunkCount * Config.ChunkDepth;

        Blocks = new IBlock[GlobalChunkWidth
                           , GlobalChunkHeight
                           , GlobalChunkDepth
                           ];

        int bT = 0;

        for (int x = 0; x < GlobalChunkWidth; x++)
        {
            for (int y = 0; y < GlobalChunkHeight; y++)
            {
                for (int z = 0; z < GlobalChunkDepth; z++)
                {
                    bT = (bT + 1) % 4;
                    float y_norm = (float)y / GlobalChunkHeight;
                    float x_norm = (float)x / GlobalChunkWidth;
                    float z_norm = (float)z / GlobalChunkDepth;
                    bool isLand = y_norm < GetHeight(x_norm, z_norm);
                    Blocks[x, y, z] = new Block() { isSolid = true, isVisible = isLand, blockType = bT };

                }
            }
        }
    }

    public static float GetHeight(float x, float z)
    {
        return Mathf.PerlinNoise(x, z);
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
