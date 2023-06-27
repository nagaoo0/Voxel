using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VPlayerInteraction : MonoBehaviour{

    public void InteractWithBlocks (Transform cam) {

        if (Input.GetMouseButtonDown (0)) {
            Ray ray = new Ray (cam.position, cam.forward);
            RaycastHit hitData;
            ray.origin = ray.GetPoint (1.0f);

            if (Physics.Raycast (ray, out hitData)) {
                //Debug.Log (hitData.point + ", " + hitData.normal + ", ");

                Chunk chunk;
                if(World.instance.GetChunkAtInteract(hitData.point,hitData.normal,false, out chunk)){

                    chunk.SetBlockAt(hitData.point,hitData.normal,Block.Stone,false);

                }

            }
        }
        if (Input.GetMouseButtonDown (1)) {
            Ray ray = new Ray (cam.position, cam.forward);
            RaycastHit hitData;
            ray.origin = ray.GetPoint (1.0f);

            if (Physics.Raycast (ray, out hitData)) {
                Debug.Log (hitData.point + ", " + hitData.normal + ", ");

                Chunk chunk;
                if(World.instance.GetChunkAtInteract(hitData.point,hitData.normal,true, out chunk)){

                    chunk.SetBlockAt(hitData.point,hitData.normal,Block.Stone,true);

                }

            }
        }
    }

}