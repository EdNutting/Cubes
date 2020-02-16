using System.Collections;
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
    sealed class NeighbourVisibleStates
    {
        public bool visibleAbove;
        public bool visibleBelow;

        public bool visibleLeft;
        public bool visibleRight;

        public bool visibleForward;
        public bool visibleBackward;
    }

    Vector3[] vertices;
    int[] triangles;
    Vector2[] uv;

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
            triangles = triangles,
            uv = uv
        };
        meshFilter.mesh = mesh;
    }

    void GenerateBlocksMesh()
    {
        int visibleFaceCount = CountVisibleFaces();

        vertices = new Vector3[visibleFaceCount * 4];
        triangles = new int[visibleFaceCount * 2 * 3]; // faces * triangles per face * vertices per triangle
        uv = new Vector2[vertices.Length];

        int triangleIndex = 0;
        int vertexIndex = 0;

        for (int x = 0; x < Config.ChunkWidth; x++)
        {
            for (int y = 0; y < Config.ChunkHeight; y++)
            {
                for (int z = 0; z < Config.ChunkDepth; z++)
                {
                    if (Block.IsBlockVisible(world.Blocks, LocalPosToBlockPos(new Vector3Int(x, y, z))))
                    {
                        GenerateBlockMesh(x, y, z, ref triangleIndex, ref vertexIndex);
                    }
                }
            }
        }
    }

    void GenerateBlockMesh(int x, int y, int z, ref int triangleIndex, ref int vertexIndex)
    {
        NeighbourSolidStates solidStates = GetNeighbourSolidStates(x, y, z);
        NeighbourVisibleStates visibleStates = GetNeighbourVisibleStates(x, y, z);

        BlockUVMap uvMap = Block.GetBlockUVMap(Block.GetBlockType(world.Blocks, new Vector3Int(x, y, z)));

        if (!solidStates.solidBelow || !visibleStates.visibleBelow)
            GenerateFaceMesh(x, y, z, new Vector3Int(1, 0, 0), new Vector3Int(0, 0, 1), ref triangleIndex, ref vertexIndex, uvMap.bottom);

        if (!solidStates.solidAbove || !visibleStates.visibleAbove)
            GenerateFaceMesh(x, y + 1, z, new Vector3Int(0, 0, 1), new Vector3Int(1, 0, 0), ref triangleIndex, ref vertexIndex, uvMap.top);

        if (!solidStates.solidBackward || !visibleStates.visibleBackward)
            GenerateFaceMesh(x, y, z, new Vector3Int(0, 1, 0), new Vector3Int(1, 0, 0), ref triangleIndex, ref vertexIndex, uvMap.back);

        if (!solidStates.solidForward || !visibleStates.visibleForward)
            GenerateFaceMesh(x, y, z + 1, new Vector3Int(1, 0, 0), new Vector3Int(0, 1, 0), ref triangleIndex, ref vertexIndex, uvMap.front);

        if (!solidStates.solidLeft || !visibleStates.visibleLeft)
            GenerateFaceMesh(x, y, z, new Vector3Int(0, 0, 1), new Vector3Int(0, 1, 0), ref triangleIndex, ref vertexIndex, uvMap.left);

        if (!solidStates.solidRight || !visibleStates.visibleRight)
            GenerateFaceMesh(x + 1, y, z, new Vector3Int(0, 1, 0), new Vector3Int(0, 0, 1), ref triangleIndex, ref vertexIndex, uvMap.right);
    }

    void GenerateFaceMesh(int x, int y, int z, Vector3Int planeAxis1, Vector3Int planeAxis2, ref int triangleIndex, ref int vertexIndex, Vector2[] uvMap)
    {
        Vector3Int corner = new Vector3Int(x, y, z);

        Vector3Int v0 = corner;
        Vector3Int v1 = corner + planeAxis1;
        Vector3Int v2 = corner + planeAxis2;
        Vector3Int v3 = corner + planeAxis1 + planeAxis2;

        // 1 -- 3
        // |    |
        // 0 -- 2

        vertices[vertexIndex + 0] = new Vector3(v0.x, v0.y, v0.z);
        vertices[vertexIndex + 1] = new Vector3(v1.x, v1.y, v1.z);
        vertices[vertexIndex + 2] = new Vector3(v2.x, v2.y, v2.z);
        vertices[vertexIndex + 3] = new Vector3(v3.x, v3.y, v3.z);

        triangles[triangleIndex++] = vertexIndex;
        triangles[triangleIndex++] = vertexIndex + 1;
        triangles[triangleIndex++] = vertexIndex + 2;

        triangles[triangleIndex++] = vertexIndex + 1;
        triangles[triangleIndex++] = vertexIndex + 3;
        triangles[triangleIndex++] = vertexIndex + 2;

        uv[vertexIndex + 0] = uvMap[0];
        uv[vertexIndex + 1] = uvMap[1];
        uv[vertexIndex + 2] = uvMap[2];
        uv[vertexIndex + 3] = uvMap[3];

        vertexIndex += 4;
    }

    NeighbourSolidStates GetNeighbourSolidStates(int x, int y, int z)
    {
        return new NeighbourSolidStates
        { solidAbove = Block.IsBlockSolid(world.Blocks, LocalPosToBlockPos(new Vector3Int(x, y + 1, z)))
        , solidBelow = Block.IsBlockSolid(world.Blocks, LocalPosToBlockPos(new Vector3Int(x, y - 1, z)))
        , solidLeft = Block.IsBlockSolid(world.Blocks, LocalPosToBlockPos(new Vector3Int(x - 1, y, z)))
        , solidRight = Block.IsBlockSolid(world.Blocks, LocalPosToBlockPos(new Vector3Int(x + 1, y, z)))
        , solidForward = Block.IsBlockSolid(world.Blocks, LocalPosToBlockPos(new Vector3Int(x, y, z + 1)))
        , solidBackward = Block.IsBlockSolid(world.Blocks, LocalPosToBlockPos(new Vector3Int(x, y, z - 1)))
        };
    }

    NeighbourVisibleStates GetNeighbourVisibleStates(int x, int y, int z)
    {
        return new NeighbourVisibleStates
        { visibleAbove = Block.IsBlockVisible(world.Blocks, LocalPosToBlockPos(new Vector3Int(x, y + 1, z)))
        , visibleBelow = Block.IsBlockVisible(world.Blocks, LocalPosToBlockPos(new Vector3Int(x, y - 1, z)))
        , visibleLeft = Block.IsBlockVisible(world.Blocks, LocalPosToBlockPos(new Vector3Int(x - 1, y, z)))
        , visibleRight = Block.IsBlockVisible(world.Blocks, LocalPosToBlockPos(new Vector3Int(x + 1, y, z)))
        , visibleForward = Block.IsBlockVisible(world.Blocks, LocalPosToBlockPos(new Vector3Int(x, y, z + 1)))
        , visibleBackward = Block.IsBlockVisible(world.Blocks, LocalPosToBlockPos(new Vector3Int(x, y, z - 1)))
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
                    if (Block.IsBlockVisible(world.Blocks, LocalPosToBlockPos(new Vector3Int(x, y, z))))
                    {
                        NeighbourSolidStates solidStates = GetNeighbourSolidStates(x, y, z);
                        NeighbourVisibleStates visibleStates = GetNeighbourVisibleStates(x, y, z);

                        visibleFaces += solidStates.solidAbove && visibleStates.visibleAbove ? 0 : 1;
                        visibleFaces += solidStates.solidBelow && visibleStates.visibleBelow ? 0 : 1;
                        visibleFaces += solidStates.solidLeft && visibleStates.visibleLeft ? 0 : 1;
                        visibleFaces += solidStates.solidRight && visibleStates.visibleRight ? 0 : 1;
                        visibleFaces += solidStates.solidForward && visibleStates.visibleForward ? 0 : 1;
                        visibleFaces += solidStates.solidBackward && visibleStates.visibleBackward ? 0 : 1;
                    }
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
