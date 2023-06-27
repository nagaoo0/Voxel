using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk {
    public static Vector3Int size = new Vector3Int (8, 8, 8);
    public static int BlockPerChunk = 32;
    float offset = (float)size.x/(float)BlockPerChunk;
    public Mesh mesh;
    public Vector3Int position;
    public bool ready = false;

    Block[] blocks;

    public Chunk (Vector3Int pos) {

        position = pos;

    }

    public void GenerateBlockArray () {

        blocks = new Block[BlockPerChunk* BlockPerChunk* BlockPerChunk * 2];
        int index = 0;

        for (int x = 0; x < BlockPerChunk; x++) {
            for (int y = 0; y < BlockPerChunk; y++) {
                for (int z = 0; z < BlockPerChunk; z++) {
                    int r = Random.Range (-2, 4);
                    r = 0;

                    //if (index > 64){continue;}
                    float ox = x*offset;
                    float oy = y*offset;
                    float oz = z*offset;

                    float mounts = Mathf.PerlinNoise ((ox + position.x + World.instance.Seed) / 128f, (oz + position.z + World.instance.Seed) / 128f) * 80f +
                        Mathf.PerlinNoise ((ox + position.x + World.instance.Seed + 54) / 64f, (oz + position.z + World.instance.Seed + 54) / 64f) * 12f +
                        (Mathf.PerlinNoise ((ox + position.x + World.instance.Seed + 586) / 512f, (oz + position.z + World.instance.Seed + 586) / 512f) * 10f) *
                        (Mathf.PerlinNoise ((ox + position.x + World.instance.Seed + 206) / 64f, (oz + position.z + World.instance.Seed + 206) / 64f));

                    float value = Mathf.PerlinNoise ((ox + position.x + World.instance.Seed) / 128f, (oz + position.z + World.instance.Seed) / 128f) * 36f +
                        Mathf.PerlinNoise ((ox + position.x + World.instance.Seed + 86) / 64f, (oz + position.z + World.instance.Seed + 86) / 64f) * 12f +
                        Mathf.PerlinNoise ((ox + position.x + World.instance.Seed - 600) / 32f, (oz + position.z + World.instance.Seed - 600) / 32f) * 10f +
                        (Mathf.PerlinNoise ((ox + position.x + World.instance.Seed + 5) / 512f, (oz + position.z + World.instance.Seed + 5) / 512f) * 40f) *
                        (Mathf.PerlinNoise ((ox + position.x + World.instance.Seed + 200) / 124f, (oz + position.z + World.instance.Seed + 200) / 124f) +
                            (Mathf.PerlinNoise ((ox + position.x + World.instance.Seed - 100) / 200f, (oz + position.z + World.instance.Seed - 100) / 200f) * 5f) / 10)

                        +
                        16
                    ;

           /*          float perlin3D = Perlin3D ((ox + position.x) * 0.05f, (oy + position.y) * 0.045f, (oz + position.z) * 0.05f);
                    if (mounts > value && perlin3D > 0.5 && value > 52) {
                        if (oy + position.y <= mounts - 5 + r) {
                            blocks[index] = Block.Stone;
                        }
                        if (oy + position.y > mounts - 5 + r / 2 && y + position.y < mounts) {
                            blocks[index] = Block.Stone;
                        }
                        if (oy + position.y == mounts) {
                            blocks[index] = Block.Stone;
                        }
                    } */

                    //Generate blocks
               /*      if (oy + position.y > value) {
                        if (oy + position.y == value + offset && value > 52 && value < 60 && Random.Range (0, 350) == 1) {
                            StructureGenerator.GenerateRock (position, x, y, z, blocks);
                        } else
                        if (oy + position.y < 50 && blocks[index] == Block.Air)
                            blocks[index] = Block.Air;
                        else {
                            index++;
                            continue;
                        }
                    } */

                    /* if (value > 54 && value > mounts && value == y + position.y && Random.Range (0, 300) == 1 && blocks[index] == Block.Air) {
                        StructureGenerator.GenerateTree (position, x, y, z, blocks);
                    } */
/* 
                    if (value == y + position.y && oy + position.y > 50 && blocks[index] == Block.Air)
                        blocks[index] = Block.Grass;

                    if (value >= (oy + position.y) && oy + position.y <= 50 && value - 4 + r / 2 < (oy + position.y) && value < 53 && blocks[index] == Block.Air)
                        blocks[index] = Block.Sand;

                    else if (value > (oy + position.y) && value - 8 + r < (oy + position.y) && blocks[index] == Block.Air)
                        blocks[index] = Block.Dirt;

                    else  */
                    if (value - 6*offset + r >= (oy + position.y) && blocks[index] == Block.Air)
                        blocks[index] = Block.Stone;
                    index++;

                }
            }
        }

        StructureGenerator.GetWaitingBlocks (position, blocks);

    }

    public IEnumerator GenerateMesh () {

        MeshBuilder builder = new MeshBuilder (position, blocks);
        builder.Start ();

        yield return new WaitUntil (() => builder.Update ());

        mesh = builder.GetMesh (ref mesh);
        MeshColliderRegion.AddMeshCollider (this, mesh);

        ready = true;
        builder = null;

        //Debug.Log ("generated mesh at : " + position);
    }

    public Block GetBlockAt (int x, int y, int z) {
        x -= position.x;
        y -= position.y;
        z -= position.z;

        if (IsPointInChunk (x, y, z))
            return blocks[x * Chunk.BlockPerChunk * Chunk.BlockPerChunk + y * Chunk.BlockPerChunk + z];
        return Block.Air;
    }

    public bool SetBlockAt (Vector3 point, Vector3 normal, Block block, bool setBlockMode) {

        if (setBlockMode == false) {
            int x = Mathf.FloorToInt (point.x) - position.x;
            int y = Mathf.FloorToInt (point.y) - position.y;
            int z = Mathf.FloorToInt (point.z) - position.z;

            if (normal.y > 0.5) y -= 1;
            if (normal.x > 0.5) x -= 1;
            if (normal.z > 0.5) z -= 1;

            if (IsPointInChunk (x, y, z)) {
                blocks[x * Chunk.BlockPerChunk * Chunk.BlockPerChunk + y * Chunk.BlockPerChunk + z] = Block.Air;
                Chunk chunk = this;
                //Debug.Log ("changed block");
                ready = false;
                World.instance.StartCoroutine (chunk.GenerateMesh ());
                return true;
            }
        }
        if (setBlockMode == true) {
            int x = Mathf.FloorToInt (point.x) - position.x;
            int y = Mathf.FloorToInt (point.y) - position.y;
            int z = Mathf.FloorToInt (point.z) - position.z;

            if (normal.y < -0.5) y -= 1;
            if (normal.x < -0.5) x -= 1;
            if (normal.z < -0.5) z -= 1;


            if (IsPointInChunk (x, y, z)) {
                blocks[x * Chunk.BlockPerChunk * Chunk.BlockPerChunk + y * Chunk.BlockPerChunk + z] = block;
                Chunk chunk = this;
                //Debug.Log ("changed block");
                ready = false;
                World.instance.StartCoroutine (chunk.GenerateMesh ());
                return true;
            }
        }
        return false;
    }

    bool IsPointInChunk (int x, int y, int z) {
        return x >= 0 && y >= 0 && z >= 0 && x < Chunk.size.x && y < Chunk.size.y && z < Chunk.size.z;
    }

    //3D Perlin Noise
    public static float Perlin3D (float x, float y, float z) {

        float ab = Mathf.PerlinNoise (x, y);
        float bc = Mathf.PerlinNoise (y, z);
        float ac = Mathf.PerlinNoise (x, z);

        float ba = Mathf.PerlinNoise (y, x);
        float cb = Mathf.PerlinNoise (z, y);
        float ca = Mathf.PerlinNoise (z, x);

        float abc = ab + bc + ac + ba + cb + ca;
        return abc / 6f;

    }
}