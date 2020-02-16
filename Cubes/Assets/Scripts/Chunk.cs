using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    Vector3[] vertices;
    int[] triangles;

    Mesh mesh;
    public MeshFilter meshFilter;

    void Start()
    {
        GenerateBlocks();

        mesh = new Mesh
        {
            vertices = vertices,
            triangles = triangles
        };
        meshFilter.mesh = mesh;
    }

    void GenerateBlocks()
    {
        int visibleFaceCount = CountVisibleFaces();

        vertices = new Vector3[(1 + Config.ChunkWidth) * (1 + Config.ChunkHeight) * (1 + Config.ChunkDepth)];
        triangles = new int[visibleFaceCount * 2 * 3]; // faces * triangles per face * vertices per triangle

        GenerateVertices();

        int triangleIndex = 0;

        for (int x = 0; x < Config.ChunkWidth; x++)
        {
            for (int y = 0; y < Config.ChunkHeight; y++)
            {
                for (int z = 0; z < Config.ChunkDepth; z++)
                {
                    triangleIndex += GenerateBlock(x, y, z, triangleIndex);
                }
            }
        }
    }

    int GenerateBlock(int x, int y, int z, int triangleIndex)
    {
        int startTriangleIndex = triangleIndex;

        triangleIndex += GenerateFace(x, y, z, new Vector3Int(1, 0, 0), new Vector3Int(0, 0, 1), 3 * triangleIndex);
        triangleIndex += GenerateFace(x, y + 1, z, new Vector3Int(0, 0, 1), new Vector3Int(1, 0, 0), 3 * triangleIndex);

        triangleIndex += GenerateFace(x, y, z, new Vector3Int(0, 1, 0), new Vector3Int(1, 0, 0), 3 * triangleIndex);
        triangleIndex += GenerateFace(x, y, z + 1, new Vector3Int(1, 0, 0), new Vector3Int(0, 1, 0), 3 * triangleIndex);

        triangleIndex += GenerateFace(x, y, z, new Vector3Int(0, 0, 1), new Vector3Int(0, 1, 0), 3 * triangleIndex);
        triangleIndex += GenerateFace(x + 1, y, z, new Vector3Int(0, 1, 0), new Vector3Int(0, 0, 1), 3 * triangleIndex);

        return triangleIndex - startTriangleIndex;
    }

    int GenerateFace(int x, int y, int z, Vector3Int planeAxis1, Vector3Int planeAxis2, int triangleStartIndex)
    {
        Vector3Int corner = new Vector3Int(x, y, z);

        Vector3Int v1 = corner;
        Vector3Int v2 = corner + planeAxis1;
        Vector3Int v3 = corner + planeAxis2;

        Vector3Int v4 = corner + planeAxis1;
        Vector3Int v5 = corner + planeAxis1 + planeAxis2;
        Vector3Int v6 = corner + planeAxis2;

        triangles[0 + triangleStartIndex] = CalculateVertexIndex(v1);
        triangles[1 + triangleStartIndex] = CalculateVertexIndex(v2);
        triangles[2 + triangleStartIndex] = CalculateVertexIndex(v3);

        triangles[3 + triangleStartIndex] = CalculateVertexIndex(v4);
        triangles[4 + triangleStartIndex] = CalculateVertexIndex(v5);
        triangles[5 + triangleStartIndex] = CalculateVertexIndex(v6);

        return 2;
    }

    int CalculateVertexIndex(Vector3Int v)
    {
        return CalculateVertexIndex(v.x, v.y, v.z);
    }

    int CalculateVertexIndex(int x, int y, int z)
    {
        int index = y * (1 + Config.ChunkWidth) * (1 + Config.ChunkDepth);
        index += z * (1 + Config.ChunkWidth);
        index += x;
        return index;
    }

    void GenerateVertices()
    {
        for (int x = 0; x <= Config.ChunkWidth; x++)
        {
            for (int y = 0; y <= Config.ChunkHeight; y++)
            {
                for (int z = 0; z <= Config.ChunkDepth; z++)
                {
                    int index = CalculateVertexIndex(x, y, z);
                    vertices[index] = new Vector3(x, y, z);
                }
            }
        }
    }

    int CountVisibleFaces()
    {
        int blocks = Config.ChunkWidth * Config.ChunkHeight * Config.ChunkDepth;
        return blocks * 6;
    }
}
