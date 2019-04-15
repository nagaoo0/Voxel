using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshColliderRegion
{
    static Dictionary<Chunk, MeshCollider> chunkcolliderMap = new Dictionary<Chunk, MeshCollider>();
    static Queue<MeshCollider> colliderPool = new Queue<MeshCollider>();
    static GameObject parent;

    public static void Initialize()
    {

        parent = new GameObject("Collider Controller");

    }

    public static void AddMeshCollider (Chunk chunk, Mesh mesh){
        MeshCollider coll;
        if(chunkcolliderMap.TryGetValue(chunk, out coll)){
            coll.sharedMesh = mesh;
        } else {
            if(colliderPool.Count > 0){
                coll = colliderPool.Dequeue();
            } else {
                GameObject go = new GameObject();
                go.transform.SetParent(parent.transform);

                coll = go.AddComponent<MeshCollider>();
                coll.cookingOptions = MeshColliderCookingOptions.None;
            }

            coll.sharedMesh = mesh;
            chunkcolliderMap.Add (chunk, coll);
        }
    }

    public static void RemoveMeshCollider (Chunk chunk){
        MeshCollider coll;
        if(chunkcolliderMap.TryGetValue(chunk, out coll)){
            chunkcolliderMap.Remove(chunk);
            colliderPool.Enqueue(coll);
        }
    }
}
