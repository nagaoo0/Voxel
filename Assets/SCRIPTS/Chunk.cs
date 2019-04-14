using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public static Vector3Int size = new Vector3Int (32,128,32);
    public Mesh mesh;
    public Vector3Int position;
    public bool ready = false;

    Block[] blocks;

    public Chunk (Vector3Int pos){

        position = pos;

    }

    public void GenerateBlockArray(){

        blocks = new Block[size.x*size.y*size.z];
        int index = 0;

        for (int x = 0; x < size.x; x++){
            for (int y=0; y<size.y; y++){
                for(int z=0; z<size.z; z++){

                    int value=Mathf.FloorToInt(Mathf.PerlinNoise(x/128f,z/128f)*30f+30);
                    int r = Random.Range(-2,2);
                
                    if(value < y){
                        index++;
                        continue;
                    }

                    if(value==y)
                        blocks [index] = Block.Grass;
                    

                    else if(value > y && value-4+r < y)
                        blocks [index] = Block.Dirt;

                    else if(value-4+r >= y)
                        blocks [index] = Block.Stone;

                    index++;

                }
            }
        }

    }

    public IEnumerator GenerateMesh () {

        MeshBuilder builder = new MeshBuilder(position, blocks);
        builder.Start();

        yield return new WaitUntil(() => builder.Update());

        mesh = builder.GetMesh(ref mesh);
        ready = true;
        builder = null;
    }

}
