using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

public class World : MonoBehaviour
{
    public static Matrix4x4 id = Matrix4x4.identity;
    public Material material;
    public Texture texture;
    public static World instance;

    public static float maxViewDist = 64;
    public static int MaxChunksVisible;
    public Transform chunkloader;

    public static Vector3 viewerPosition;

    Dictionary<Vector3Int, Chunk> chunkPosMap;
    List<Chunk> ChunksVisibleLastUpdate = new List<Chunk>();

    public int Seed = 42069;
    public int radius = 2;
    public int height = 4;

    void Awake()
    {
        TextureController.Initialize("", texture);
        chunkPosMap = new Dictionary<Vector3Int, Chunk>();

        MeshColliderRegion.Initialize();

        instance = this;
        if (Seed == -1)
        {
            Seed = Random.Range(5000, 894563);
        }
        Debug.Log("Seed : " + Seed);
        Random.InitState(Seed);
    }

    void Start()
    {
        GenerateWorld();
        MaxChunksVisible = Mathf.RoundToInt(maxViewDist / Chunk.size.x);
        Debug.Log("ChunksLoadDistance " + MaxChunksVisible);
    }

    async void GenerateWorld()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = -radius; x < radius; x++)
            {
                for (int z = -radius; z < radius; z++)
                {
                    Chunk chunk = new Chunk(
                        new Vector3Int(x * Chunk.size.x, y * Chunk.size.y, z * Chunk.size.z)
                    );
                    //Debug.Log("Before generate block array");
                    await chunk.GenerateBlockArray();
                    //Debug.Log("After generate block array");
                    chunkPosMap.Add(chunk.position, chunk);

                    StartCoroutine(chunk.GenerateMesh());
                }
            }
        }

        foreach (Chunk ch in chunkPosMap.Values)
        {
            StartCoroutine(ch.GenerateMesh());
        }

        StartCoroutine(HandleChunkLoading());

        //Debug.Log("Start Coroutine Generate Mesh");
    }

    void Update()
    {
        //Each frame
        viewerPosition = new Vector3(
            chunkloader.position.x,
            chunkloader.position.y,
            chunkloader.position.z
        );
        //UpdateVisibleChunks();

        foreach (Chunk ch in chunkPosMap.Values)
        {
            if (ch.mesh != null && ch.mesh.vertexCount > 0 && ch.isVisible)
            {
                Graphics.DrawMesh(ch.mesh, id, material, 0);

                if (true)
                {
                    Color color = new Color(1f, 1f, 1f);
                    Vector3 pos = new Vector3(ch.position.x, ch.position.y, ch.position.z);

                    Debug.DrawLine(pos, pos + new Vector3(Chunk.size.x, 0, 0), color);
                    Debug.DrawLine(pos, pos + new Vector3(0, Chunk.size.y, 0), color);
                    Debug.DrawLine(pos, pos + new Vector3(0, 0, Chunk.size.z), color);

                    Debug.DrawLine(
                        pos + new Vector3(0, Chunk.size.y, 0),
                        pos + new Vector3(Chunk.size.x, Chunk.size.y, 0),
                        color
                    );
                    Debug.DrawLine(
                        pos + new Vector3(Chunk.size.x, 0, Chunk.size.y),
                        pos + new Vector3(Chunk.size.x, Chunk.size.y, Chunk.size.x),
                        color
                    );
                    Debug.DrawLine(
                        pos + new Vector3(0, Chunk.size.y, 0),
                        pos + new Vector3(0, Chunk.size.y, Chunk.size.z),
                        color
                    );

                    Debug.DrawLine(
                        pos + new Vector3(0, Chunk.size.y, Chunk.size.z),
                        pos + new Vector3(Chunk.size.x, Chunk.size.y, Chunk.size.z),
                        color
                    );
                    Debug.DrawLine(
                        pos + new Vector3(Chunk.size.x, Chunk.size.y, 0),
                        pos + new Vector3(Chunk.size.x, Chunk.size.y, Chunk.size.z),
                        color
                    );

                    Debug.DrawLine(
                        pos + new Vector3(0, 0, Chunk.size.z),
                        pos + new Vector3(0, Chunk.size.y, Chunk.size.z),
                        color
                    );
                    Debug.DrawLine(
                        pos + new Vector3(Chunk.size.x, 0, 0),
                        pos + new Vector3(Chunk.size.x, Chunk.size.y, 0),
                        color
                    );

                    Debug.DrawLine(
                        pos + new Vector3(0, 0, Chunk.size.z),
                        pos + new Vector3(Chunk.size.x, 0, Chunk.size.z),
                        color
                    );
                    Debug.DrawLine(
                        pos + new Vector3(Chunk.size.x, 0, 0),
                        pos + new Vector3(Chunk.size.x, 0, Chunk.size.z),
                        color
                    );
                }
            }
        }
    }

    IEnumerator HandleChunkLoading()
    {
        while (true)
        {
            UpdateVisibleChunks();
            yield return new WaitForSeconds(0.5f);
        }
    }

    void UpdateVisibleChunks()
    {
        for (int i = 0; i < ChunksVisibleLastUpdate.Count; i++)
        {
            if (ChunksVisibleLastUpdate[i] != null)
            {
                ChunksVisibleLastUpdate[i].SetVisible(false);
                //Debug.Log(ChunksVisibleLastUpdate[i].isVisible);
            }
        }

        ChunksVisibleLastUpdate.Clear();
        List<Chunk> newChunks = new List<Chunk>();

        Vector3Int currentChCoord = WorldToChunkCoords(
            Mathf.RoundToInt(viewerPosition.x),
            Mathf.RoundToInt(viewerPosition.y),
            Mathf.RoundToInt(viewerPosition.z)
        );

        for (int yoffset = -MaxChunksVisible; yoffset <= MaxChunksVisible; yoffset++)
        {
            for (int zoffset = -MaxChunksVisible; zoffset <= MaxChunksVisible; zoffset++)
            {
                for (int xoffset = -MaxChunksVisible; xoffset <= MaxChunksVisible; xoffset++)
                {
                    Vector3Int viewedChunkCoord = new Vector3Int(
                        currentChCoord.x + xoffset * Chunk.size.x,
                        currentChCoord.y + yoffset * Chunk.size.y,
                        currentChCoord.z + zoffset * Chunk.size.z
                    );

                    if (chunkPosMap.ContainsKey(viewedChunkCoord))
                    {
                        Chunk CurrenctCunck;
                        World.instance.GetChunkAt(
                            viewedChunkCoord.x,
                            viewedChunkCoord.y,
                            viewedChunkCoord.z,
                            out CurrenctCunck
                        );

                        if (CurrenctCunck != null)
                            //Debug.Log("loader " + viewedChunkCoord.x);
                            CurrenctCunck.UpdateChunk();
                        if (CurrenctCunck.isVisible)
                        {
                            ChunksVisibleLastUpdate.Add(CurrenctCunck);
                        }
                    }
                    else if (!chunkPosMap.ContainsKey(viewedChunkCoord))
                    {
                        Chunk CurrenctCunck = new Chunk(viewedChunkCoord);
                        newChunks.Add(CurrenctCunck);
                        //await CurrenctCunck.GenerateBlockArray();
                        //StartCoroutine (CurrenctCunck.GenerateMesh ());

                        CurrenctCunck.UpdateChunk();
                        if (CurrenctCunck.isVisible)
                        {
                            ChunksVisibleLastUpdate.Add(CurrenctCunck);
                        }
                    }
                }
            }
        }

        for (int i = 0; i < newChunks.Count; i++)
        {
            if (newChunks[i] != null)
            {
                newChunks[i].SetVisible(false);
                //Debug.Log(newChunks[i]);
                //StartCoroutine(newChunks[i].GenerateMesh());
                GenerateNewChunks(newChunks[i].position, newChunks[i]);
                
            }
        }

        foreach (Chunk ch in chunkPosMap.Values)
        {
            if (ch.isVisible)
            {
                StartCoroutine(ch.GenerateMesh());
                
            }
        }
    }

    private async void GenerateNewChunks(Vector3Int pos, Chunk chunk)
    {
        chunkPosMap.Add(pos, chunk);
        await chunk.GenerateBlockArray();
        //await chunk.AsyncMesher();
        StartCoroutine(chunk.GenerateMesh());
        //UpdateNeigbours(pos);
    }

    public void UpdateNeigbours(Vector3Int position)
    {
        Chunk[] neighbors = new Chunk[6];
        bool[] exists = new bool[6];

        exists[0] = World.instance.GetChunkAt(
            position.x,
            position.y,
            position.z + Chunk.size.z,
            out neighbors[0]
        );
        exists[1] = World.instance.GetChunkAt(
            position.x + Chunk.size.x,
            position.y,
            position.z,
            out neighbors[1]
        );
        exists[2] = World.instance.GetChunkAt(
            position.x,
            position.y,
            position.z - Chunk.size.z,
            out neighbors[2]
        );
        exists[3] = World.instance.GetChunkAt(
            position.x - Chunk.size.x,
            position.y,
            position.z,
            out neighbors[3]
        );

        exists[4] = World.instance.GetChunkAt(
            position.x,
            position.y + Chunk.size.y,
            position.z,
            out neighbors[4]
        );
        exists[5] = World.instance.GetChunkAt(
            position.x,
            position.y - Chunk.size.y,
            position.z,
            out neighbors[5]
        );

        foreach (Chunk n in neighbors)
        {
            if (n != null)
            {
                StartCoroutine(n.GenerateMesh());     
            }
        }
    }

    public bool GetChunkAt(int x, int y, int z, out Chunk chunk)
    {
        Vector3Int key = WorldToChunkCoords(x, y, z);

        return chunkPosMap.TryGetValue(key, out chunk);
    }

    public bool GetChunkAtInteract(Vector3 point, Vector3 normal, bool getAdj, out Chunk chunk)
    {
        Vector3Int key;
        if (getAdj)
        {
            key = WorldToChunkCoords(point.x + normal.x, point.y + normal.y, point.z + normal.z);
            return chunkPosMap.TryGetValue(key, out chunk);
        }
        key = WorldToChunkCoords(point.x, point.y, point.z);
        return chunkPosMap.TryGetValue(key, out chunk);
    }

    public static Vector3Int WorldToChunkCoords(int x, int y, int z)
    {
        return new Vector3Int(
            Mathf.FloorToInt(x / (float)Chunk.size.x) * Chunk.size.x,
            Mathf.FloorToInt(y / (float)Chunk.size.y) * Chunk.size.y,
            Mathf.FloorToInt(z / (float)Chunk.size.z) * Chunk.size.z
        );
    }

    public static Vector3Int WorldToChunkCoords(float x, float y, float z)
    {
        return new Vector3Int(
            Mathf.FloorToInt(x / (float)Chunk.size.x) * Chunk.size.x,
            Mathf.FloorToInt(y / (float)Chunk.size.y) * Chunk.size.y,
            Mathf.FloorToInt(z / (float)Chunk.size.z) * Chunk.size.z
        );
    }
}
