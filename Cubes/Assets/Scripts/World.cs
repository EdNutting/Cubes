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
        int GlobalChunkWidth = Config.Instance.ChunkCount * Config.Instance.ChunkWidth;
        int GlobalChunkHeight = Config.Instance.ChunkCount * Config.Instance.ChunkHeight;
        int GlobalChunkDepth = Config.Instance.ChunkCount * Config.Instance.ChunkDepth;

        Blocks = new IBlock[GlobalChunkWidth, GlobalChunkHeight, GlobalChunkDepth];

        int bT = 0;

        for (int x = 0; x < GlobalChunkWidth; x++)
        {
            for (int y = 0; y < GlobalChunkHeight; y++)
            {
                for (int z = 0; z < GlobalChunkDepth; z++)
                {
                    float y_norm = (float)y / GlobalChunkHeight;
                    float x_norm = (float)x / GlobalChunkWidth;
                    float z_norm = (float)z / GlobalChunkDepth;
                    bool isLand = y_norm < GetHeight(x_norm, z_norm);
                    Blocks[x, y, z] = new Block() { IsSolid = true, IsVisible = isLand, BlockType = bT };
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
        Chunks = new GameObject[Config.Instance.ChunkCount, Config.Instance.ChunkCount, Config.Instance.ChunkCount];
        for (int cX = 0; cX < Config.Instance.ChunkCount; cX++)
        {
            for (int cY = 0; cY < Config.Instance.ChunkCount; cY++)
            {
                for (int cZ = 0; cZ < Config.Instance.ChunkCount; cZ++)
                {
                    int x = cX * Config.Instance.ChunkWidth;
                    int y = cY * Config.Instance.ChunkHeight;
                    int z = cZ * Config.Instance.ChunkDepth;

                    var chunk = GameObject.Instantiate(BaseChunk, new Vector3(x, y, z), Quaternion.identity);
                    Chunks[cX, cY, cZ] = chunk;

                    Chunk chunkComp = chunk.GetComponent<Chunk>();
                    chunkComp.Init(this, new Vector3Int(x, y, z));
                }
            }
        }
    }
}
