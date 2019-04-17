using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureGenerator
{

    public static void GenerateRock(Vector3Int pos, int x, int y, int z, Block[] blocks)
    {

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                for (int k =0; k<3; k++){
                if (IsPointInBounds(x + i, y-k, z + j))
                {
                    blocks[((x + i) * Chunk.size.y * Chunk.size.z + (y-k) * Chunk.size.z + (z + j))] = Block.Stone;
                }
                else
                {
                    Vector3Int neghborChunkPos = World.WorldToChunkCoords(pos.x+i+x,pos.y+y-k,pos.z+j+z);

                    List<BlockPositions> list;
                    if (WaitingBlocks.TryGetValue(neghborChunkPos, out list)){
                        list.Add(new BlockPositions(Block.Stone, PositionToIndex(pos.x+i+x,pos.y+y-k,pos.z+j+z)));
                    } else {
                        list = new List<BlockPositions> ();
                        list.Add(new BlockPositions(Block.Stone, PositionToIndex(pos.x+i+x,pos.y+y-k,pos.z+j+z)));

                        WaitingBlocks.Add(neghborChunkPos,list);
                    }
                }
                }
            }
        }

    }

   
    public struct BlockPositions
    {
        public Block block;
        public int index;

        public BlockPositions(Block block, int index){
            this.block = block;
            this.index = index;
        }

    }

    static Dictionary<Vector3Int, List<BlockPositions>> WaitingBlocks = new Dictionary<Vector3Int, List<BlockPositions>>();

    public static void GetWaitingBlocks(Vector3Int pos, Block[] blocks)
    {
        //Pull off blosk from DS
        //Put it in blosk array
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

    static int PositionToIndex(int x, int y, int z){
        Vector3Int chunkPos = World.WorldToChunkCoords(x,y,z);

        x -= chunkPos.x;
        y -= chunkPos.y;
        z -= chunkPos.z;

        return x * Chunk.size.y * Chunk.size.z + y * Chunk.size.z + z;

    }

}
