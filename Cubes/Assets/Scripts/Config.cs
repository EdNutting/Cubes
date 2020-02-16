using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Config
{
    public static int ChunkWidth = 5;
    public static int ChunkHeight = 5;
    public static int ChunkDepth = 5;

    public static int ChunkCount = 4;

    public static int TextureCols = 2;
    public static int TextureRows = 2;

    public static float TextureXShift = 1.0f / TextureCols;
    public static float TextureYShift = 1.0f / TextureRows;

    public static int BlockTypeCount = 4;

    public static BlockUVMap[] BlockUVMaps;

    static Config()
    {
        BlockUVMaps = new BlockUVMap[TextureCols * TextureRows];

        // 1 -- 3
        // |    |
        // 0 -- 2

        for (int i = 0; i < BlockTypeCount; i++)
        {
            int r = i / TextureCols;
            int c = i % TextureCols;

            BlockUVMaps[i] =
                new BlockUVMap
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
    }
}
