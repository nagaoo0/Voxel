using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public static Vector3Int size = new Vector3Int(16, 16, 16);
    public Mesh mesh;
    public Vector3Int position;
    public bool ready = false;

    Block[] blocks;

    public Chunk(Vector3Int pos)
    {

        position = pos;

    }

    public void GenerateBlockArray()
    {

        blocks = new Block[size.x * size.y * size.z];
        int index = 0;

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    int r = Random.Range(-2, 4);

                    int value = Mathf.FloorToInt(
                        Mathf.PerlinNoise((x + position.x + 1000) / 128f, (z + position.z + 1000) / 128f) * 32f  
                        + Mathf.PerlinNoise((x + position.x + 1000 +86) / 64f, (z + position.z + 1000 +86) / 64f) * 12f 
                        + Mathf.PerlinNoise((x + position.x + 1000 -600) / 32f, (z + position.z + 1000 -600) / 32f) * 10f
                        + Mathf.PerlinNoise((x + position.x + 1000 +5) / 512f, (z + position.z + 1000 +5) / 512f) * 40f  
                        
                        + 16
                        );

                    if (value < y + position.y)
                    {
                        index++;
                        continue;
                    } 
                    
                    //if(y+position.y>50 && value > y + position.y)
                        //blocks[index] = Block.Water;

                    if (value == y + position.y)
                        blocks[index] = Block.Grass;


                    else if (value > (y + position.y) && value - 6 + r < (y + position.y))
                        blocks[index] = Block.Dirt;

                    else if (value - 6 + r >= (y + position.y))
                        blocks[index] = Block.Stone;
                    
                    index++;

                }
            }
        }

    }

    public IEnumerator GenerateMesh()
    {

        MeshBuilder builder = new MeshBuilder(position, blocks);
        builder.Start();

        yield return new WaitUntil(() => builder.Update());

        mesh = builder.GetMesh(ref mesh);
        ready = true;
        builder = null;
    }

    public Block GetBlockAt(int x, int y, int z)
    {
        x -= position.x;
        y -= position.y;
        z -= position.z;

        if (IsPointInChunk(x, y, z))
            return blocks[x * Chunk.size.y * Chunk.size.z + y * Chunk.size.z + z];
        return Block.Air;
    }

    bool IsPointInChunk(int x, int y, int z)
    {
        return x >= 0 && y >= 0 && z >= 0 && x < Chunk.size.x && y < Chunk.size.y && z < Chunk.size.z;
    }

}
