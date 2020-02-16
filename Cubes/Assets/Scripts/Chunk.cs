using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A chunk is a single mesh of many blocks used to improve render efficiency.
/// </summary>
public class Chunk : MonoBehaviour
{
    /// <summary>
    /// The visible or solid states of the blocks surrounding the current block.
    /// </summary>
    sealed class NeighbourStates
    {
        public bool Above;
        public bool Below;

        public bool Left;
        public bool Right;

        public bool Forward;
        public bool Backward;
    }

    /* Construct the mesh data, then assign it to a new mesh. */
    Vector3[] _vertices;
    int[] _triangles;
    Vector2[] _uv;
    Mesh _mesh;

    World _world;

    /// <summary>
    /// Position of this chunk in global block coordinates.
    /// </summary>
    Vector3Int _position;

    public MeshFilter MeshFilter;

    /// <summary>
    /// Initialise the chunk.
    /// </summary>
    /// <param name="world">The world.</param>
    /// <param name="position">The position of this chunk in global block coordinates.</param>
    public void Init(World world, Vector3Int position)
    {
        _world = world;
        _position = position;

        gameObject.name = string.Format("Chunk {0}, {1}, {2}", position.x, position.y, position.z);
    }

    void Start()
    {
        GenerateBlocks();

        _mesh = new Mesh
        {
            vertices = _vertices,
            triangles = _triangles,
            uv = _uv
        };
        MeshFilter.mesh = _mesh;
    }

    /// <summary>
    /// Generates vertices, triangles and uv map for the blocks in this chunk.
    /// </summary>
    void GenerateBlocks()
    {
        // Pre-compute the number of faces that will be visible
        //  Enables us to create the vertices, triangles and uv arrays
        //  with exactly the right size, rather than using `List`s
        int visibleFaceCount = CountVisibleFaces();

        _vertices = new Vector3[visibleFaceCount * 4];
        _triangles = new int[visibleFaceCount * 2 * 3]; // faces * triangles per face * vertices per triangle
        _uv = new Vector2[_vertices.Length];

        int triangleIndex = 0;
        int vertexIndex = 0;

        for (int x = 0; x < Config.Instance.ChunkWidth; x++)
        {
            for (int y = 0; y < Config.Instance.ChunkHeight; y++)
            {
                for (int z = 0; z < Config.Instance.ChunkDepth; z++)
                {
                    if (Block.IsBlockVisible(_world.Blocks, TranslateLocalToGlobalBlockPosition(new Vector3Int(x, y, z))))
                    {
                        GenerateBlock(x, y, z, ref triangleIndex, ref vertexIndex);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Generate the vertices, triangles and uv map for a given block.
    /// 
    /// Must be called in-sequence as per GenerateBlocks.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="triangleIndex"></param>
    /// <param name="vertexIndex"></param>
    void GenerateBlock(int x, int y, int z, ref int triangleIndex, ref int vertexIndex)
    {
        NeighbourStates solidStates = GetNeighbourSolidStates(x, y, z);
        NeighbourStates visibleStates = GetNeighbourStates(x, y, z);

        BlockUVMap uvMap = Block.GetBlockUVMap(Block.GetBlockType(_world.Blocks, new Vector3Int(x, y, z)));

        if (!solidStates.Below || !visibleStates.Below)
            GenerateFace(x, y, z, new Vector3Int(1, 0, 0), new Vector3Int(0, 0, 1), ref triangleIndex, ref vertexIndex, uvMap.bottom, false);

        if (!solidStates.Above || !visibleStates.Above)
            GenerateFace(x, y + 1, z, new Vector3Int(0, 0, 1), new Vector3Int(1, 0, 0), ref triangleIndex, ref vertexIndex, uvMap.top, false);

        if (!solidStates.Backward || !visibleStates.Backward)
            GenerateFace(x, y, z, new Vector3Int(0, 1, 0), new Vector3Int(1, 0, 0), ref triangleIndex, ref vertexIndex, uvMap.back, false);

        if (!solidStates.Forward || !visibleStates.Forward)
            GenerateFace(x, y, z + 1, new Vector3Int(1, 0, 0), new Vector3Int(0, 1, 0), ref triangleIndex, ref vertexIndex, uvMap.front, true);

        if (!solidStates.Left || !visibleStates.Left)
            GenerateFace(x, y, z, new Vector3Int(0, 0, 1), new Vector3Int(0, 1, 0), ref triangleIndex, ref vertexIndex, uvMap.left, true);

        if (!solidStates.Right || !visibleStates.Right)
            GenerateFace(x + 1, y, z, new Vector3Int(0, 1, 0), new Vector3Int(0, 0, 1), ref triangleIndex, ref vertexIndex, uvMap.right, false);
    }

    /// <summary>
    /// Generate a single face of a block.
    /// 
    /// Must be called in sequence as per GenerateBlock.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="planeAxis1">A unit-vector for one of the axes of the plane of the face.</param>
    /// <param name="planeAxis2">The other unit-vector of the plane.</param>
    /// <param name="triangleIndex"></param>
    /// <param name="vertexIndex"></param>
    /// <param name="uvMap">The UV map coordinates for this face.</param>
    /// <param name="rotateOrder">Uses the alternate rotation of the texture for this face.</param>
    void GenerateFace(int x, int y, int z, Vector3Int planeAxis1, Vector3Int planeAxis2, ref int triangleIndex, ref int vertexIndex, Vector2[] uvMap, bool rotateOrder)
    {
        Vector3Int corner = new Vector3Int(x, y, z);

        /* Vertices of a face in the following layout:

           1 -- 3
           |    |
           0 -- 2
        */

        Vector3Int vertex0 = corner;
        Vector3Int vertex1 = corner + planeAxis1;
        Vector3Int vertex2 = corner + planeAxis2;
        Vector3Int vertex3 = corner + planeAxis1 + planeAxis2;

        _vertices[vertexIndex + 0] = new Vector3(vertex0.x, vertex0.y, vertex0.z);
        _vertices[vertexIndex + 1] = new Vector3(vertex1.x, vertex1.y, vertex1.z);
        _vertices[vertexIndex + 2] = new Vector3(vertex2.x, vertex2.y, vertex2.z);
        _vertices[vertexIndex + 3] = new Vector3(vertex3.x, vertex3.y, vertex3.z);

        _triangles[triangleIndex++] = vertexIndex;
        _triangles[triangleIndex++] = vertexIndex + 1;
        _triangles[triangleIndex++] = vertexIndex + 2;

        _triangles[triangleIndex++] = vertexIndex + 1;
        _triangles[triangleIndex++] = vertexIndex + 3;
        _triangles[triangleIndex++] = vertexIndex + 2;

        if (rotateOrder)
        {
            _uv[vertexIndex + 0] = uvMap[0];
            _uv[vertexIndex + 1] = uvMap[2];
            _uv[vertexIndex + 2] = uvMap[1];
            _uv[vertexIndex + 3] = uvMap[3];
        }
        else
        {
            _uv[vertexIndex + 0] = uvMap[0];
            _uv[vertexIndex + 1] = uvMap[1];
            _uv[vertexIndex + 2] = uvMap[2];
            _uv[vertexIndex + 3] = uvMap[3];
        }

        vertexIndex += 4;
    }

    NeighbourStates GetNeighbourSolidStates(int x, int y, int z)
    {
        return new NeighbourStates
        { Above = Block.IsBlockSolid(_world.Blocks, TranslateLocalToGlobalBlockPosition(new Vector3Int(x, y + 1, z)))
        , Below = Block.IsBlockSolid(_world.Blocks, TranslateLocalToGlobalBlockPosition(new Vector3Int(x, y - 1, z)))
        , Left = Block.IsBlockSolid(_world.Blocks, TranslateLocalToGlobalBlockPosition(new Vector3Int(x - 1, y, z)))
        , Right = Block.IsBlockSolid(_world.Blocks, TranslateLocalToGlobalBlockPosition(new Vector3Int(x + 1, y, z)))
        , Forward = Block.IsBlockSolid(_world.Blocks, TranslateLocalToGlobalBlockPosition(new Vector3Int(x, y, z + 1)))
        , Backward = Block.IsBlockSolid(_world.Blocks, TranslateLocalToGlobalBlockPosition(new Vector3Int(x, y, z - 1)))
        };
    }

    NeighbourStates GetNeighbourStates(int x, int y, int z)
    {
        return new NeighbourStates
        { Above = Block.IsBlockVisible(_world.Blocks, TranslateLocalToGlobalBlockPosition(new Vector3Int(x, y + 1, z)))
        , Below = Block.IsBlockVisible(_world.Blocks, TranslateLocalToGlobalBlockPosition(new Vector3Int(x, y - 1, z)))
        , Left = Block.IsBlockVisible(_world.Blocks, TranslateLocalToGlobalBlockPosition(new Vector3Int(x - 1, y, z)))
        , Right = Block.IsBlockVisible(_world.Blocks, TranslateLocalToGlobalBlockPosition(new Vector3Int(x + 1, y, z)))
        , Forward = Block.IsBlockVisible(_world.Blocks, TranslateLocalToGlobalBlockPosition(new Vector3Int(x, y, z + 1)))
        , Backward = Block.IsBlockVisible(_world.Blocks, TranslateLocalToGlobalBlockPosition(new Vector3Int(x, y, z - 1)))
        };
    }

    int CountVisibleFaces()
    {
        int visibleFaces = 0;

        for (int x = 0; x < Config.Instance.ChunkWidth; x++)
        {
            for (int y = 0; y < Config.Instance.ChunkHeight; y++)
            {
                for (int z = 0; z < Config.Instance.ChunkDepth; z++)
                {
                    if (Block.IsBlockVisible(_world.Blocks, TranslateLocalToGlobalBlockPosition(new Vector3Int(x, y, z))))
                    {
                        NeighbourStates solidStates = GetNeighbourSolidStates(x, y, z);
                        NeighbourStates visibleStates = GetNeighbourStates(x, y, z);

                        visibleFaces += solidStates.Above && visibleStates.Above ? 0 : 1;
                        visibleFaces += solidStates.Below && visibleStates.Below ? 0 : 1;
                        visibleFaces += solidStates.Left && visibleStates.Left ? 0 : 1;
                        visibleFaces += solidStates.Right && visibleStates.Right ? 0 : 1;
                        visibleFaces += solidStates.Forward && visibleStates.Forward ? 0 : 1;
                        visibleFaces += solidStates.Backward && visibleStates.Backward ? 0 : 1;
                    }
                }
            }
        }

        return visibleFaces;
    }

    Vector3Int TranslateLocalToGlobalBlockPosition(Vector3Int localPosition)
    {
        return _position + localPosition;
    }
}
