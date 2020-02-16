using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    GameObject[] Chunks;
    public GameObject BaseChunk;

    void Start()
    {
        GenerateWorld();
    }

    void GenerateWorld()
    {
        int chunkCount = 2;

        Chunks = new GameObject[chunkCount];
        for (int i = 0; i < chunkCount; i++)
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
