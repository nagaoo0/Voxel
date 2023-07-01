using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public static Vector3Int size = new Vector3Int(16, 16, 16);

    public bool isVisible = false;
    public static int BlockPerChunk = 16;

    public static float offset = (float)size.x / (float)BlockPerChunk;

    public static float waterLevel = 16 * offset;
    public Mesh mesh;
    public Vector3Int position;
    public bool ready = false;

    

    Block[] blocks;

    public Chunk(Vector3Int pos)
    {

        position = pos;
        //Debug.Log("Chunk build");

    }

    public void SetVisible(bool visible)
    {
        isVisible = visible;
    }

    public void GenerateBlockArray()
    {

        blocks = new Block[BlockPerChunk * BlockPerChunk * BlockPerChunk];
        int index = 0;

        for (int x = 0; x < BlockPerChunk; x++)
        {
            for (int y = 0; y < BlockPerChunk; y++)
            {
                for (int z = 0; z < BlockPerChunk; z++)
                {
                    float r = Random.Range(-2 * offset, 1 * offset);
                    //r = 0;

                    //if (index > 8*8){continue;}


                    float ox = x * offset;
                    float oy = y * offset;
                    float oz = z * offset;

                    Vector3 blockPosition = new Vector3 (ox + position.x, oy + position.y, oz + position.z );

                    float perlinMask2D = Mathf.PerlinNoise((ox + position.x + World.instance.Seed) / 128f, (oz + position.z + World.instance.Seed) / 128f) * 64f * offset +
                        Mathf.PerlinNoise((ox + position.x + World.instance.Seed + 54) / 64f, (oz + position.z + World.instance.Seed + 54) / 64f) * 12f +
                        (Mathf.PerlinNoise((ox + position.x + World.instance.Seed + 586) / 512f, (oz + position.z + World.instance.Seed + 586) / 512f) * 10f) *
                        (Mathf.PerlinNoise((ox + position.x + World.instance.Seed + 206) / 64f, (oz + position.z + World.instance.Seed + 206) / 64f));

                    float perlin2DValue = Mathf.PerlinNoise((ox + position.x + World.instance.Seed) / 128f, (oz + position.z + World.instance.Seed) / 128f) * 64f+
                        Mathf.PerlinNoise ((ox + position.x + World.instance.Seed + 86) / 64f, (oz + position.z + World.instance.Seed + 86) / 64f) * 12f +
                        Mathf.PerlinNoise ((ox + position.x + World.instance.Seed - 600) / 32f, (oz + position.z + World.instance.Seed - 600) / 32f) * 10f +
                        (Mathf.PerlinNoise ((ox + position.x + World.instance.Seed + 5) / 512f, (oz + position.z + World.instance.Seed + 5) / 512f) * 20f) *
                        (Mathf.PerlinNoise ((ox + position.x + World.instance.Seed + 200) / 124f, (oz + position.z + World.instance.Seed + 200) / 124f) +
                            (Mathf.PerlinNoise ((ox + position.x + World.instance.Seed - 100) / 200f, (oz + position.z + World.instance.Seed - 100) / 200f) * 5f) / 10)

                        
                    ;

                    perlin2DValue = Mathf.RoundToInt(perlin2DValue)  * offset +1; //+1 for testing

                    float VegetationMask = Mathf.PerlinNoise((ox + position.x + World.instance.Seed + 500) / 128f, (oz + position.z + World.instance.Seed + 900) / 128f);

                    float perlin3D = Perlin3D((ox + position.x + World.instance.Seed +46) / 64f, (oy + position.y + World.instance.Seed+841) / 64f, (oz + position.z + World.instance.Seed+452) / 64f);

                    if (perlinMask2D > perlin2DValue && perlin3D > 0.5 && perlin2DValue > 52)
                    {
                        if (oy + position.y <= perlinMask2D - 5 + r)
                        {
                            blocks[index] = Block.Stone;
                        }
                        if (oy + position.y > perlinMask2D - 5 + r / 2 && y + position.y < perlinMask2D)
                        {
                            blocks[index] = Block.Dirt;
                        }
                        if (oy + position.y > perlinMask2D-1 && y + position.y < perlinMask2D)
                        {
                            blocks[index] = Block.Grass;
                        }
                    }

                    //Rules
                    if (blocks[index] == Block.Air)
                    {
                        if (perlin2DValue - 6 * offset + r >= (oy + position.y) && blocks[index] == Block.Air)
                            blocks[index] = Block.Stone;

                        if (perlin2DValue > (oy + position.y) && perlin2DValue - 6 + r < (oy + position.y) && blocks[index] == Block.Air)
                            blocks[index] = Block.Dirt;
                    }

                    if (perlin2DValue == (oy + position.y) /* && oy + position.y >= waterLevel && blocks[index] == Block.Dirt */)
                        blocks[index] = Block.Grass;

                    if ((oy + position.y) > perlin2DValue - (Mathf.Clamp(waterLevel - perlin2DValue + offset * 4, 0, 4) - Mathf.Abs(r)) && (oy + position.y) <= perlin2DValue)
                        blocks[index] = Block.Sand;

                    if (perlin2DValue <= waterLevel && waterLevel > (oy + position.y) && (oy + position.y) > perlin2DValue - (waterLevel - perlin2DValue) && blocks[index] == Block.Air)
                        blocks[index] = Block.Water;

                    //Generate Structures
                    /* if (oy + position.y > perlin2DValue) {
                        if (oy + position.y == perlin2DValue + offset && perlin2DValue > 0 && perlin2DValue < 60 && Random.Range (0, 350) == 1) {
                            //StructureGenerator.GenerateRock (position, x, y, z, blocks);
                        } else
                        if (oy + position.y < 50 && blocks[index] == Block.Air)
                            blocks[index] = Block.Air;
                        else {
                            index++;
                            continue;
                        }
                    } */

                    if (perlin2DValue == (oy + position.y) && blocks[index] == Block.Grass)/*&& perlin2DValue - 1 < (oy + position.y)  && blocks[index] == Block.Air */
                    {
                        int Density = 20;
                        if (VegetationMask > 0.5 + r/10){
                            if (Random.Range (0, 1000/Density) == 1 )
                            {
                            //blocks[index] = Block.Water;
                            StructureGenerator.GenerateTree(position, x, y, z, blocks);
                            }                          
                        }
                            
                        //Debug.Log("Tree Generated");
                    }
                    
                    if(blocks[index] == Block.Stone){
                        if (perlin2DValue*0.75 + r > (oy + position.y)) {
                            float cracks = Mathf.Abs(perlin3D*2 - 1);
                            Debug.Log(cracks);
                            if (cracks < 0.05)
                            {
                                blocks[index] = Block.Air;
                            }
                        }
                    }
                    
                    
                    /// END RULES
                    index++;

                    



                }
            }
        }

        StructureGenerator.GetWaitingBlocks(position, blocks);

    }

    public void UpdateChunk()
    {
        Vector3 pos = new Vector3(position.x, position.y, position.z);
        Vector3 viewer = new Vector3(World.viewerPosition.x, World.viewerPosition.y, World.viewerPosition.z);

        float dist = Vector3.Distance(viewer, pos);
        if (dist <= World.maxViewDist)
            isVisible = true;
    }

    public IEnumerator GenerateMesh()
    {

        MeshBuilder builder = new MeshBuilder(position, blocks);
        builder.Start();

        yield return new WaitUntil(() => builder.Update());

        mesh = builder.GetMesh(ref mesh);
        if (mesh.vertexCount > 0)
            MeshColliderRegion.AddMeshCollider(this, mesh);
        //Debug.Log("generated mesh at : " + position);

        ready = true;
        builder = null;


    }

    public Block GetBlockAt(float x, float y, float z)
    {
        x -= position.x;
        y -= position.y;
        z -= position.z;

        if (IsPointInChunk(x, y, z))
            return blocks[(int)((x / offset) * Chunk.BlockPerChunk * Chunk.BlockPerChunk + (y / offset) * Chunk.BlockPerChunk + (z / offset))];
        return Block.Air;
    }

    public bool SetBlockAt(Vector3 point, Vector3 normal, Block block, bool setBlockMode)
    {

        if (setBlockMode == false)
        {
            int x = Mathf.FloorToInt(point.x) - position.x;
            int y = Mathf.FloorToInt(point.y) - position.y;
            int z = Mathf.FloorToInt(point.z) - position.z;

            if (normal.y > 0.5) y -= 1;
            if (normal.x > 0.5) x -= 1;
            if (normal.z > 0.5) z -= 1;

            if (IsPointInChunk(x, y, z))
            {
                blocks[x * Chunk.BlockPerChunk * Chunk.BlockPerChunk + y * Chunk.BlockPerChunk + z] = Block.Air;
                Chunk chunk = this;
                //Debug.Log ("changed block");
                ready = false;
                World.instance.StartCoroutine(chunk.GenerateMesh());
                return true;
            }
        }
        if (setBlockMode == true)
        {
            int x = Mathf.FloorToInt(point.x) - position.x;
            int y = Mathf.FloorToInt(point.y) - position.y;
            int z = Mathf.FloorToInt(point.z) - position.z;

            if (normal.y < -0.5) y -= 1;
            if (normal.x < -0.5) x -= 1;
            if (normal.z < -0.5) z -= 1;


            if (IsPointInChunk(x, y, z))
            {
                blocks[x * Chunk.BlockPerChunk * Chunk.BlockPerChunk + y * Chunk.BlockPerChunk + z] = block;
                Chunk chunk = this;
                //Debug.Log ("changed block");
                ready = false;
                World.instance.StartCoroutine(chunk.GenerateMesh());
                return true;
            }
        }
        return false;
    }

    bool IsPointInChunk(float x, float y, float z)
    {
        return x >= 0 && y >= 0 && z >= 0 && x < Chunk.size.x && y < Chunk.size.y && z < Chunk.size.z;
    }

    //3D Perlin Noise
    public static float Perlin3D(float x, float y, float z)
    {

        float ab = Mathf.PerlinNoise(x, y);
        float bc = Mathf.PerlinNoise(y, z);
        float ac = Mathf.PerlinNoise(x, z);

        float ba = Mathf.PerlinNoise(y, x);
        float cb = Mathf.PerlinNoise(z, y);
        float ca = Mathf.PerlinNoise(z, x);

        float abc = ab + bc + ac + ba + cb + ca;
        return abc / 6f;

    }
}