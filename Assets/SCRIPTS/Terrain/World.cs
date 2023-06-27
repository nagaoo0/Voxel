using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {
    public static Matrix4x4 id = Matrix4x4.identity;
    public Material material;
    public Texture texture;
    public static World instance;

    public static float maxViewDist = 32;
    public static int MaxChunksVisible;
    public Transform chunkloader;

    public static Vector3 viewerPosition;

    Dictionary<Vector3Int, Chunk> chunkPosMap;
    List<Chunk> ChunksVisibleLastUpdate = new List<Chunk>();

    public int Seed = 42069;
    public int radius = 2;
    public int height = 4;

    void Awake () {
        TextureController.Initialize ("", texture);
        chunkPosMap = new Dictionary<Vector3Int, Chunk> ();

        MeshColliderRegion.Initialize ();

        instance = this;
        //Seed = Random.Range (8000, 9000);
        Debug.Log ("Seed : " + Seed);
    }

    void Start () {
        StartCoroutine (HandleChunkLoading());
        for (int y = 0; y < height; y++) {
            for (int x = -radius; x < radius; x++) {
                for (int z = -radius; z < radius; z++) {
                    Chunk chunk = new Chunk (new Vector3Int (x * Chunk.size.x, y * Chunk.size.y, z * Chunk.size.z));
                    chunk.GenerateBlockArray ();

                    chunkPosMap.Add (chunk.position, chunk);
                    Debug.Log("Init " + chunk.position.x);

                    
                    
                }
            }
        }

        
        MaxChunksVisible = Mathf.RoundToInt(maxViewDist/Chunk.size.x);
        Debug.Log("ChunksLoadDistance " + MaxChunksVisible);

        foreach (Chunk ch in chunkPosMap.Values) {
            StartCoroutine (ch.GenerateMesh ());
        }

        

    }

    void Update () {
        //Each frame
        viewerPosition = new Vector3(chunkloader.position.x,chunkloader.position.y,chunkloader.position.z);
        //UpdateVisibleChunks();

        foreach (Chunk ch in chunkPosMap.Values) {
            if (ch.mesh != null && ch.isVisible) {
                Graphics.DrawMesh (ch.mesh, id, material, 0);
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

    void UpdateVisibleChunks(){

        for (int i = 0; i< ChunksVisibleLastUpdate.Count; i++){
            if (ChunksVisibleLastUpdate[i] != null){
                ChunksVisibleLastUpdate[i].SetVisible(false);
                Debug.Log(ChunksVisibleLastUpdate[i].isVisible);    
            }
            
        }

        ChunksVisibleLastUpdate.Clear();
        List<Chunk> newChunks = new List<Chunk>();

        Vector3Int currentChCoord = WorldToChunkCoords (Mathf.RoundToInt(viewerPosition.x),
                                                        Mathf.RoundToInt(viewerPosition.y),
                                                        Mathf.RoundToInt(viewerPosition.z));
        
        for (int yoffset = -MaxChunksVisible; yoffset <= MaxChunksVisible; yoffset++)
        {
            for (int zoffset = -MaxChunksVisible; zoffset <= MaxChunksVisible; zoffset++)
            {
                for (int xoffset = -MaxChunksVisible; xoffset <= MaxChunksVisible; xoffset++)
                {
                    Vector3Int viewedChunkCoord = new Vector3Int(currentChCoord.x + xoffset*Chunk.size.x,
                                                                    currentChCoord.y + yoffset*Chunk.size.y,
                                                                    currentChCoord.z + zoffset*Chunk.size.z);
                
                    if(chunkPosMap.ContainsKey(viewedChunkCoord)){
                        Chunk CurrenctCunck;
                        World.instance.GetChunkAt(viewedChunkCoord.x, 
                                                    viewedChunkCoord.y, 
                                                    viewedChunkCoord.z, 
                                                    out CurrenctCunck);

                        if (CurrenctCunck!=null)
                            Debug.Log("loader " + viewedChunkCoord.x);
                            CurrenctCunck.UpdateChunk();
                            if(CurrenctCunck.isVisible){
                                ChunksVisibleLastUpdate.Add(CurrenctCunck);
                            }
                    }
                    /* else if(!chunkPosMap.ContainsKey(viewedChunkCoord))
                    {
                        Chunk CurrenctCunck = new Chunk (viewedChunkCoord);
                        chunkPosMap.Add (viewedChunkCoord, CurrenctCunck);
                        newChunks.Add(CurrenctCunck);
                        CurrenctCunck.GenerateBlockArray ();
                        //StartCoroutine (CurrenctCunck.GenerateMesh ());

                        CurrenctCunck.UpdateChunk();
                            if(CurrenctCunck.isVisible){
                                ChunksVisibleLastUpdate.Add(CurrenctCunck);
                            }
                    } */
                
                }   
            }
        }

       /*  for (int i = 0; i< newChunks.Count; i++){
            if (newChunks[i] != null){
                newChunks[i].SetVisible(false);
                Debug.Log(newChunks[i]);
                //StartCoroutine (newChunks[i].GenerateMesh ());    
            }
            
        } */
        
    }

    public bool GetChunkAt (int x, int y, int z, out Chunk chunk) {
        Vector3Int key = WorldToChunkCoords (x, y, z);

        return chunkPosMap.TryGetValue (key, out chunk);
    }

    public bool GetChunkAtInteract (Vector3 point, Vector3 normal, bool getAdj, out Chunk chunk) {
        Vector3Int key;
        if (getAdj) {
            key = WorldToChunkCoords (point.x + normal.x, point.y + normal.y, point.z + normal.z);
            return chunkPosMap.TryGetValue (key, out chunk);
        }
        key = WorldToChunkCoords (point.x, point.y, point.z);
        return chunkPosMap.TryGetValue (key, out chunk);
    }


    public static Vector3Int WorldToChunkCoords (int x, int y, int z) {
        return new Vector3Int (
            Mathf.FloorToInt (x / (float) Chunk.size.x) * Chunk.size.x,
            Mathf.FloorToInt (y / (float) Chunk.size.y) * Chunk.size.y,
            Mathf.FloorToInt (z / (float) Chunk.size.z) * Chunk.size.z
        );

    }

    public static Vector3Int WorldToChunkCoords (float x, float y, float z) {
        return new Vector3Int (
            Mathf.FloorToInt (x / (float) Chunk.size.x) * Chunk.size.x,
            Mathf.FloorToInt (y / (float) Chunk.size.y) * Chunk.size.y,
            Mathf.FloorToInt (z / (float) Chunk.size.z) * Chunk.size.z
        );

    }
}