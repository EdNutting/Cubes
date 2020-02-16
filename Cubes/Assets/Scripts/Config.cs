using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Config
{
    public static int ChunkWidth = 16;
    public static int ChunkHeight = 16;
    public static int ChunkDepth = 16;

    public static int ChunkCount = 10;

    public static int TextureCols = 2;
    public static int TextureRows = 2;

    public static float TextureXShift = 1.0f / TextureCols;
    public static float TextureYShift = 1.0f / TextureRows;

    public static int BlockTypeCount = 3;

    public static BlockUVMap[] BlockUVMaps;

    public enum BlockTypeIDs : int
    {
        Stone = 0,
        Grass = 1,
        Dirt = 2
    }

    static BlockUVMap GenerateBlockUVMap(Vector2Int[] positions)
    {
        return new BlockUVMap
        {
            top = new Vector2[]
            {
                new Vector2(TextureYShift * positions[0].x, TextureYShift * positions[0].y),
                new Vector2(TextureYShift * positions[0].x, TextureYShift * (positions[0].y + 1)),
                new Vector2(TextureXShift * (positions[0].x + 1), TextureYShift * positions[0].y),
                new Vector2(TextureXShift * (positions[0].x + 1), TextureYShift * (positions[0].y + 1))
            },

            bottom = new Vector2[]
            {
                new Vector2(TextureYShift * positions[1].x, TextureYShift * positions[1].y),
                new Vector2(TextureYShift * positions[1].x, TextureYShift * (positions[1].y + 1)),
                new Vector2(TextureXShift * (positions[1].x + 1), TextureYShift * positions[1].y),
                new Vector2(TextureXShift * (positions[1].x + 1), TextureYShift * (positions[1].y + 1))
            },

            left = new Vector2[]
            {
                new Vector2(TextureYShift * positions[2].x, TextureYShift * positions[2].y),
                new Vector2(TextureYShift * positions[2].x, TextureYShift * (positions[2].y + 1)),
                new Vector2(TextureXShift * (positions[2].x + 1), TextureYShift * positions[2].y),
                new Vector2(TextureXShift * (positions[2].x + 1), TextureYShift * (positions[2].y + 1))
            },

            right = new Vector2[]
            {
                new Vector2(TextureYShift * positions[3].x, TextureYShift * positions[3].y),
                new Vector2(TextureYShift * positions[3].x, TextureYShift * (positions[3].y + 1)),
                new Vector2(TextureXShift * (positions[3].x + 1), TextureYShift * positions[3].y),
                new Vector2(TextureXShift * (positions[3].x + 1), TextureYShift * (positions[3].y + 1))
            },

            front = new Vector2[]
            {
                new Vector2(TextureYShift * positions[4].x, TextureYShift * positions[4].y),
                new Vector2(TextureYShift * positions[4].x, TextureYShift * (positions[4].y + 1)),
                new Vector2(TextureXShift * (positions[4].x + 1), TextureYShift * positions[4].y),
                new Vector2(TextureXShift * (positions[4].x + 1), TextureYShift * (positions[4].y + 1))
            },

            back = new Vector2[]
            {
                new Vector2(TextureYShift * positions[5].x, TextureYShift * positions[5].y),
                new Vector2(TextureYShift * positions[5].x, TextureYShift * (positions[5].y + 1)),
                new Vector2(TextureXShift * (positions[5].x + 1), TextureYShift * positions[5].y),
                new Vector2(TextureXShift * (positions[5].x + 1), TextureYShift * (positions[5].y + 1))
            }
        };
    }

    static BlockUVMap GenerateBlockUVMap(int c, int r)
    {
        return new BlockUVMap
        {
            top = new Vector2[]
            {
                new Vector2(TextureYShift * c, TextureYShift * r),
                new Vector2(TextureYShift * c, TextureYShift * (r + 1)),
                new Vector2(TextureXShift * (c + 1), TextureYShift * r),
                new Vector2(TextureXShift * (c + 1), TextureYShift * (r + 1))
            },

            bottom = new Vector2[]
            {
                new Vector2(TextureYShift * c, TextureYShift * r),
                new Vector2(TextureYShift * c, TextureYShift * (r + 1)),
                new Vector2(TextureXShift * (c + 1), TextureYShift * r),
                new Vector2(TextureXShift * (c + 1), TextureYShift * (r + 1))
            },

            left = new Vector2[]
            {
                new Vector2(TextureYShift * c, TextureYShift * r),
                new Vector2(TextureYShift * c, TextureYShift * (r + 1)),
                new Vector2(TextureXShift * (c + 1), TextureYShift * r),
                new Vector2(TextureXShift * (c + 1), TextureYShift * (r + 1))
            },

            right = new Vector2[]
            {
                new Vector2(TextureYShift * c, TextureYShift * r),
                new Vector2(TextureYShift * c, TextureYShift * (r + 1)),
                new Vector2(TextureXShift * (c + 1), TextureYShift * r),
                new Vector2(TextureXShift * (c + 1), TextureYShift * (r + 1))
            },

            front = new Vector2[]
            {
                new Vector2(TextureYShift * c, TextureYShift * r),
                new Vector2(TextureYShift * c, TextureYShift * (r + 1)),
                new Vector2(TextureXShift * (c + 1), TextureYShift * r),
                new Vector2(TextureXShift * (c + 1), TextureYShift * (r + 1))
            },

            back = new Vector2[]
                {
                    new Vector2(TextureYShift * c, TextureYShift * r),
                    new Vector2(TextureYShift * c, TextureYShift * (r + 1)),
                    new Vector2(TextureXShift * (c + 1), TextureYShift * r),
                    new Vector2(TextureXShift * (c + 1), TextureYShift * (r + 1))
                }
        };
    }

    static Config()
    {
        BlockUVMaps = new BlockUVMap[BlockTypeCount];

        // 1 -- 3
        // |    |
        // 0 -- 2

        // Stone
        BlockUVMaps[(int)BlockTypeIDs.Stone] = GenerateBlockUVMap(0, 1);

        // Grass
        BlockUVMaps[(int)BlockTypeIDs.Grass] = GenerateBlockUVMap(new Vector2Int[] {
            new Vector2Int(0, 0),
            new Vector2Int(1, 0),
            new Vector2Int(1, 1),
            new Vector2Int(1, 1),
            new Vector2Int(1, 1),
            new Vector2Int(1, 1)
        });

        // Dirt
        BlockUVMaps[(int)BlockTypeIDs.Dirt] = GenerateBlockUVMap(1, 0);
    }
}
