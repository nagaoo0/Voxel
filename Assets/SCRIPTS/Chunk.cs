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
                        Mathf.PerlinNoise((x + position.x + World.Seed) / 128f, (z + position.z + World.Seed) / 128f) * 32f
                        + Mathf.PerlinNoise((x + position.x + World.Seed + 86) / 64f, (z + position.z + World.Seed + 86) / 64f) * 12f
                        + Mathf.PerlinNoise((x + position.x + World.Seed - 600) / 32f, (z + position.z + World.Seed - 600) / 32f) * 10f
                        + (Mathf.PerlinNoise((x + position.x + World.Seed + 5) / 512f, (z + position.z + World.Seed + 5) / 512f) * 40f)
                        * (Mathf.PerlinNoise((x + position.x + World.Seed + 200) / 124f, (z + position.z + World.Seed + 200) / 124f)
                        + (Mathf.PerlinNoise((x + position.x + World.Seed - 100) / 200f, (z + position.z + World.Seed - 100) / 200f) * 5f) / 10)

                        + 20
                        );

                    //Generate blocks
                    if (y + position.y > value)
                    {
                        if (y + position.y == value + 1 && value > 52 && value < 60 && Random.Range(0, 350) == 1)
                        {
                            StructureGenerator.GenerateRock(position, x, y, z, blocks);
                        }
                        else
                        if (y + position.y < 50 && blocks[index] == Block.Air)
                            blocks[index] = Block.Water;
                        else
                        {
                            index++;
                            continue;
                        }
                    }



                    if (value == y + position.y && y + position.y > 50 && blocks[index] == Block.Air)
                        blocks[index] = Block.Grass;

                    if (value >= (y + position.y) && y + position.y <= 50 && value - 4 + r / 2 < (y + position.y) && value < 53 && blocks[index] == Block.Air)
                        blocks[index] = Block.Sand;

                    else if (value > (y + position.y) && value - 8 + r < (y + position.y) && blocks[index] == Block.Air)
                        blocks[index] = Block.Dirt;

                    else if (value - 6 + r >= (y + position.y) && blocks[index] == Block.Air)
                        blocks[index] = Block.Stone;
                    index++;


                }
            }
        }

        StructureGenerator.GetWaitingBlocks(position, blocks);

    }

    public IEnumerator GenerateMesh()
    {

        MeshBuilder builder = new MeshBuilder(position, blocks);
        builder.Start();

        yield return new WaitUntil(() => builder.Update());

        mesh = builder.GetMesh(ref mesh);
        MeshColliderRegion.AddMeshCollider(this, mesh);

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
