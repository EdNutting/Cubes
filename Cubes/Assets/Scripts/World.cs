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
        int GlobalChunkWidth = Config.Instance.ChunksToGeneratePerAxis * Config.Instance.ChunkWidth;
        int GlobalChunkHeight = Config.Instance.ChunksToGeneratePerAxis * Config.Instance.ChunkHeight;
        int GlobalChunkDepth = Config.Instance.ChunksToGeneratePerAxis * Config.Instance.ChunkDepth;

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
                    Blocks[x, y, z] = new Block() { 
                        Position = new Vector3Int(x, y, z), 
                        IsSolid = true, 
                        IsVisible = isLand, 
                        BlockType = bT 
                    };
                }
            }
        }
    }

    public static float GetHeight(float x, float z)
    {
        return Mathf.PerlinNoise(x, z);
    }

    (Vector3Int, Vector3Int) FindCoordinateLimits()
    {
        Vector3Int min = new Vector3Int();
        Vector3Int max = new Vector3Int();

        for (int x = 0; x < Blocks.GetLength(0); x++)
        {
            for (int y = 0; y < Blocks.GetLength(1); y++)
            {
                for (int z = 0; z < Blocks.GetLength(0); z++)
                {
                    Vector3Int position = Blocks[x, y, z].Position;
                    if (position.x < min.x)
                    {
                        min.x = position.x;
                    }
                    if (position.y < min.y)
                    {
                        min.y = position.y;
                    }
                    if (position.z < min.z)
                    {
                        min.z = position.z;
                    }

                    if (position.x > max.x)
                    {
                        max.x = position.x;
                    }
                    if (position.y > max.y)
                    {
                        max.y = position.y;
                    }
                    if (position.z > max.z)
                    {
                        max.z = position.z;
                    }
                }
            }
        }

        return (min, max);
    }

    void GenerateChunks()
    {
        Vector3Int minCoords, maxCoords;
        (minCoords, maxCoords) = FindCoordinateLimits();

        int WidthInBlocks = 1 + maxCoords.x - minCoords.x;
        int HeightInBlocks = 1 + maxCoords.y - minCoords.y;
        int DepthInBlocks = 1 + maxCoords.z - minCoords.z;

        int WidthInChunks = Mathf.CeilToInt(WidthInBlocks / Config.Instance.ChunkWidth);
        int HeightInChunks = Mathf.CeilToInt(HeightInBlocks / Config.Instance.ChunkHeight);
        int DepthInChunks = Mathf.CeilToInt(DepthInBlocks / Config.Instance.ChunkDepth);

        Chunks = new GameObject[WidthInChunks, HeightInChunks, DepthInChunks];
        for (int cX = 0; cX < WidthInChunks; cX++)
        {
            for (int cY = 0; cY < HeightInChunks; cY++)
            {
                for (int cZ = 0; cZ < DepthInChunks; cZ++)
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
