using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public static Matrix4x4 id = Matrix4x4.identity;
    public Material material;
    public Texture texture;


    Chunk chunk;
    
    void Awake()
    {
       TextureController.Initialize("",texture);
    }
    
    void Start()
    {
        chunk = new Chunk(new Vector3Int(0,0,0));
        chunk.GenerateBlockArray();

        StartCoroutine (chunk.GenerateMesh());
    }

    void Update()
    {
        if (chunk.ready==true){
            Graphics.DrawMesh(chunk.mesh, id, material, 0);
        }
    }
}
