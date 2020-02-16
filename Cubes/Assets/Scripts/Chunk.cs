﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    sealed class NeighbourSolidStates
    {
        public bool solidAbove;
        public bool solidBelow;

        public bool solidLeft;
        public bool solidRight;

        public bool solidForward;
        public bool solidBackward;
    }

    Vector3[] vertices;
    int[] triangles;

    Mesh mesh;
    public MeshFilter meshFilter;

    public World world;

    public Vector3Int position;

    void Start()
    {
        GenerateBlocksMesh();

        mesh = new Mesh
        {
            vertices = vertices,
            triangles = triangles
        };
        meshFilter.mesh = mesh;
    }

    void GenerateBlocksMesh()
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
                    triangleIndex += GenerateBlockMesh(x, y, z, triangleIndex);
                }
            }
        }
    }

    int GenerateBlockMesh(int x, int y, int z, int triangleIndex)
    {
        int startTriangleIndex = triangleIndex;

        NeighbourSolidStates states = GetNeighbourSolidStates(x, y, z);

        triangleIndex += states.solidBelow ? 0 : GenerateFaceMesh(x, y, z, new Vector3Int(1, 0, 0), new Vector3Int(0, 0, 1), 3 * triangleIndex);
        triangleIndex += states.solidAbove ? 0 : GenerateFaceMesh(x, y + 1, z, new Vector3Int(0, 0, 1), new Vector3Int(1, 0, 0), 3 * triangleIndex);

        triangleIndex += states.solidBackward ? 0 : GenerateFaceMesh(x, y, z, new Vector3Int(0, 1, 0), new Vector3Int(1, 0, 0), 3 * triangleIndex);
        triangleIndex += states.solidForward ? 0 : GenerateFaceMesh(x, y, z + 1, new Vector3Int(1, 0, 0), new Vector3Int(0, 1, 0), 3 * triangleIndex);

        triangleIndex += states.solidLeft ? 0 : GenerateFaceMesh(x, y, z, new Vector3Int(0, 0, 1), new Vector3Int(0, 1, 0), 3 * triangleIndex);
        triangleIndex += states.solidRight ? 0 : GenerateFaceMesh(x + 1, y, z, new Vector3Int(0, 1, 0), new Vector3Int(0, 0, 1), 3 * triangleIndex);

        return triangleIndex - startTriangleIndex;
    }

    int GenerateFaceMesh(int x, int y, int z, Vector3Int planeAxis1, Vector3Int planeAxis2, int triangleStartIndex)
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

    NeighbourSolidStates GetNeighbourSolidStates(int x, int y, int z)
    {
        return new NeighbourSolidStates
        {
          solidAbove = Block.IsBlockSolid(world.Blocks, LocalPosToBlockPos(new Vector3Int(x, y + 1, z)))
        , solidBelow = Block.IsBlockSolid(world.Blocks, LocalPosToBlockPos(new Vector3Int(x, y - 1, z)))
        , solidLeft = Block.IsBlockSolid(world.Blocks, LocalPosToBlockPos(new Vector3Int(x - 1, y, z)))
        , solidRight = Block.IsBlockSolid(world.Blocks, LocalPosToBlockPos(new Vector3Int(x + 1, y, z)))
        , solidForward = Block.IsBlockSolid(world.Blocks, LocalPosToBlockPos(new Vector3Int(x, y, z + 1)))
        , solidBackward = Block.IsBlockSolid(world.Blocks, LocalPosToBlockPos(new Vector3Int(x, y, z - 1)))
        };
    }

    int CountVisibleFaces()
    {
        int visibleFaces = 0;

        for (int x = 0; x < Config.ChunkWidth; x++)
        {
            for (int y = 0; y < Config.ChunkHeight; y++)
            {
                for (int z = 0; z < Config.ChunkDepth; z++)
                {
                    NeighbourSolidStates states = GetNeighbourSolidStates(x, y, z);

                    visibleFaces += states.solidAbove ? 0 : 1;
                    visibleFaces += states.solidBelow ? 0 : 1;
                    visibleFaces += states.solidLeft ? 0 : 1;
                    visibleFaces += states.solidRight ? 0 : 1;
                    visibleFaces += states.solidForward ? 0 : 1;
                    visibleFaces += states.solidBackward ? 0 : 1;
                }
            }
        }

        return visibleFaces;
    }

    Vector3Int LocalPosToBlockPos(Vector3Int pos)
    {
        return position + pos;
    }

    Vector3Int BlockPosToLocalPos(Vector3Int pos)
    {
        return pos - position;
    }
}
