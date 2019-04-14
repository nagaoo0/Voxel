using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public static Matrix4x4 id = Matrix4x4.identity;
    public Material material;
    public Texture texture;
    public static World instance;

    Dictionary<Vector3Int, Chunk> chunkPosMap;

    public int radius = 2;
    public int height = 4;

    void Awake()
    {
        TextureController.Initialize("", texture);
        chunkPosMap = new Dictionary<Vector3Int, Chunk>();

        instance = this;
    }

    void Start()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = -radius; x < radius + 1; x++)
            {
                for (int z = -radius; z < radius + 1; z++)
                {
                    Chunk chunk = new Chunk(new Vector3Int(x * Chunk.size.x, y * Chunk.size.y, z * Chunk.size.z));
                    chunk.GenerateBlockArray();

                    chunkPosMap.Add(chunk.position, chunk);
                    //Debug.Log(chunk.position.y);
                }
            }
        }

        foreach (Chunk ch in chunkPosMap.Values)
        {
            StartCoroutine(ch.GenerateMesh());
        }


    }

    public bool GetChunkAt(int x, int y, int z, out Chunk chunk)
    {
        Vector3Int key = WorldToChunkCoords(x, y, z);

        return chunkPosMap.TryGetValue(key, out chunk);
    }

    void Update()
    {
        foreach (Chunk ch in chunkPosMap.Values)
        {
            if (ch.ready == true)
            {
                Graphics.DrawMesh(ch.mesh, id, material, 0);
            }
        }
    }

    public static Vector3Int WorldToChunkCoords(int x, int y, int z)
    {
        return new Vector3Int(
            Mathf.FloorToInt(x / (float)Chunk.size.x) * Chunk.size.x,
            Mathf.FloorToInt(y / (float)Chunk.size.y) * Chunk.size.y,
            Mathf.FloorToInt(z / (float)Chunk.size.z) * Chunk.size.z
            );

    }
}
