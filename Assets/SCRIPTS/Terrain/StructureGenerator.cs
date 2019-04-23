using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureGenerator
{

    public static void GenerateRock(Vector3Int pos, int x, int y, int z, Block[] blocks)
    {
        int r = Mathf.FloorToInt(Random.Range(1, 4));
        for (int i = 0; i < r; i++)
        {
            for (int j = 0; j < r; j++)
            {
                for (int k = 0; k < r; k++)
                {
                    if (IsPointInBounds(x + i, y - k, z + j))
                    {
                        blocks[((x + i) * Chunk.size.y * Chunk.size.z + (y - k) * Chunk.size.z + (z + j))] = Block.Stone;
                    }
                    else
                    {
                        Vector3Int neghborChunkPos = World.WorldToChunkCoords(pos.x + i + x, pos.y + y - k, pos.z + j + z);

                        List<BlockPositions> list;
                        if (WaitingBlocks.TryGetValue(neghborChunkPos, out list))
                        {
                            list.Add(new BlockPositions(Block.Stone, PositionToIndex(pos.x + i + x, pos.y + y - k, pos.z + j + z)));
                        }
                        else
                        {
                            list = new List<BlockPositions>();
                            list.Add(new BlockPositions(Block.Stone, PositionToIndex(pos.x + i + x, pos.y + y - k, pos.z + j + z)));

                            WaitingBlocks.Add(neghborChunkPos, list);
                        }
                    }
                }
            }
        }

    }


    public static void GenerateTree(Vector3Int pos, int x, int y, int z, Block[] blocks)
    {
        int r = Mathf.FloorToInt(Random.Range(6, 10));

        //Leaves
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < 3; k++)
                {
                    if (IsPointInBounds(x + i, y - k + r, z + j))
                    {
                        blocks[((x + i) * Chunk.size.y * Chunk.size.z + (y - k + r) * Chunk.size.z + (z + j))] = Block.Leaves;
                    }
                    else
                    {
                        Vector3Int neghborChunkPos = World.WorldToChunkCoords(pos.x + i + x, pos.y + y - k + r, pos.z + j + z);

                        List<BlockPositions> list;
                        if (WaitingBlocks.TryGetValue(neghborChunkPos, out list))
                        {
                            list.Add(new BlockPositions(Block.Leaves, PositionToIndex(pos.x + i + x, pos.y + y - k + r, pos.z + j + z)));
                        }
                        else
                        {
                            list = new List<BlockPositions>();
                            list.Add(new BlockPositions(Block.Leaves, PositionToIndex(pos.x + i + x, pos.y + y - k + r, pos.z + j + z)));

                            WaitingBlocks.Add(neghborChunkPos, list);
                        }
                    }
                }
            }
        }


        for (int t = 0; t < 3; t++)
        {
            int l1 = Mathf.FloorToInt(Random.Range(4, 6));
            int o1 =  Mathf.FloorToInt(Random.Range(-1, 1));
            int o2 = Mathf.FloorToInt(Random.Range(-3, 1));
            int o3 = Mathf.FloorToInt(Random.Range(-1, 1));
            for (int i = -l1 / 2; i <= l1 / 2; i++)
            {
                for (int j = 0; j <= l1 / 2; j++)
                {
                    for (int k = -l1 / 2; k <= l1 / 2; k++)
                    {
                        if (Random.Range(0, 5) == 1 && i == -l1 / 2 || i == l1 / 2 && j == -l1 / 2 || j == l1){
                            continue;
                        }
                        if (IsPointInBounds(x + i + o1, y + j + r + o2, z + k + o3))
                        {
                            blocks[((x + i + o1) * Chunk.size.y * Chunk.size.z + (y + j + r + o2) * Chunk.size.z + (z + k + o3))] = Block.Leaves;
                        }
                        else
                        {
                            Vector3Int neghborChunkPos = World.WorldToChunkCoords(pos.x + x + i + o1, pos.y + y + j + r + o2, pos.z + z + k + o3);

                            List<BlockPositions> list;
                            if (WaitingBlocks.TryGetValue(neghborChunkPos, out list))
                            {
                                list.Add(new BlockPositions(Block.Leaves, PositionToIndex(pos.x + x + i + o1, pos.y + y + j + r + o2, pos.z + z + k + o3)));
                            }
                            else
                            {
                                list = new List<BlockPositions>();
                                list.Add(new BlockPositions(Block.Leaves, PositionToIndex(pos.x + x + i + o1, pos.y + y + j + r + o2, pos.z + z + k + o3)));

                                WaitingBlocks.Add(neghborChunkPos, list);
                            }
                        }
                    }
                }
            }
        }
        //LOG
        //Debug.Log("Tree");

        for (int j = 0; j < r; j++)
        {
            if (IsPointInBounds(x, y + j, z))
            {
                blocks[((x) * Chunk.size.y * Chunk.size.z + (y + j) * Chunk.size.z + (z))] = Block.Log;
            }
            else
            {
                Vector3Int neghborChunkPos = World.WorldToChunkCoords(pos.x + x, pos.y + y + j, pos.z + z);

                List<BlockPositions> list;
                if (WaitingBlocks.TryGetValue(neghborChunkPos, out list))
                {
                    list.Add(new BlockPositions(Block.Log, PositionToIndex(pos.x + x, pos.y + y + j, pos.z + z)));
                }
                else
                {
                    list = new List<BlockPositions>();
                    list.Add(new BlockPositions(Block.Log, PositionToIndex(pos.x + x, pos.y + y + j, pos.z + z)));

                    WaitingBlocks.Add(neghborChunkPos, list);
                }
            }
        }


    }

    public struct BlockPositions
    {
        public Block block;
        public int index;

        public BlockPositions(Block block, int index)
        {
            this.block = block;
            this.index = index;
        }

    }

    static Dictionary<Vector3Int, List<BlockPositions>> WaitingBlocks = new Dictionary<Vector3Int, List<BlockPositions>>();

    public static void GetWaitingBlocks(Vector3Int pos, Block[] blocks)
    {

        List<BlockPositions> blockPositions;
        if (WaitingBlocks.TryGetValue(pos, out blockPositions))
        {
            for (int i = 0; i < blockPositions.Count; i++)
            {
                blocks[blockPositions[i].index] = blockPositions[i].block;
            }
        }

    }

    static bool IsPointInBounds(int x, int y, int z)
    {
        return x >= 0 && y >= 0 && z >= 0 && x < Chunk.size.x && y < Chunk.size.y && z < Chunk.size.z;
    }

    static int PositionToIndex(int x, int y, int z)
    {
        Vector3Int chunkPos = World.WorldToChunkCoords(x, y, z);

        x -= chunkPos.x;
        y -= chunkPos.y;
        z -= chunkPos.z;

        return x * Chunk.size.y * Chunk.size.z + y * Chunk.size.z + z;

    }

}
