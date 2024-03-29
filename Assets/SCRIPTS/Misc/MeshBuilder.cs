﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshBuilder : ThreadedProcess {
    //0000 0000 faces
    byte[] faces = new byte[Chunk.BlockPerChunk * Chunk.BlockPerChunk * Chunk.BlockPerChunk];
    Vector3[] vertices;
    Vector2[] uvs;
    int[] triangles;

    Vector3Int position;
    Block[] blocks;

    int sizeEstimate = 0;
    int vertexIndex = 0, trianglesIndex = 0;
    bool isVisible = true;
    //float offset = Chunk.size.x/Chunk.BlockPerChunk;
    float offset = (float)Chunk.size.x/(float)Chunk.BlockPerChunk;

    public MeshBuilder (Vector3Int pos, Block[] blocks) {

        this.position = pos;
        this.blocks = blocks;

    }

    public override void ThreadFunction () {

        //generate faces
        int index = 0;

        Chunk[] neighbors = new Chunk[6];
        bool[] exists = new bool[6];

        exists[0] = World.instance.GetChunkAt (position.x, position.y, position.z + Chunk.size.z, out neighbors[0]);
        exists[1] = World.instance.GetChunkAt (position.x + Chunk.size.x, position.y, position.z, out neighbors[1]);
        exists[2] = World.instance.GetChunkAt (position.x, position.y, position.z - Chunk.size.z, out neighbors[2]);
        exists[3] = World.instance.GetChunkAt (position.x - Chunk.size.x, position.y, position.z, out neighbors[3]);

        exists[4] = World.instance.GetChunkAt (position.x, position.y + Chunk.size.y, position.z, out neighbors[4]);
        exists[5] = World.instance.GetChunkAt (position.x, position.y - Chunk.size.y, position.z, out neighbors[5]);

        for (int x = 0; x < Chunk.BlockPerChunk; x++) {
            for (int y = 0; y < Chunk.BlockPerChunk; y++) {
                for (int z = 0; z < Chunk.BlockPerChunk; z++) {

                    float ox = x*offset;
                    float oy = y*offset;
                    float oz = z*offset;

                    if (blocks[index].IsTransparent ()) {
                        faces[index] = 0;
                        index++;
                        continue;
                    }

                    //check oz
                    if (z > 0 && blocks[index - 1] == Block.Air) {
                        faces[index] |= (byte) Direction.South;
                        sizeEstimate += 4;
                    }

                    if (z < Chunk.BlockPerChunk - 1 && blocks[index + 1] == Block.Air) {
                        faces[index] |= (byte) Direction.North;
                        sizeEstimate += 4;
                    }

                    if (z == 0 && (exists[2] == false || neighbors[2].GetBlockAt (position.x + ox, position.y + oy, position.z + oz - 1) == Block.Air)) {
                        faces[index] |= (byte) Direction.South;
                        sizeEstimate += 4;
                    }

                    if (z == Chunk.BlockPerChunk - 1 && (exists[0] == false || (neighbors[0].GetBlockAt (position.x + ox, position.y + oy, position.z + oz + 1) == Block.Air))) {
                        faces[index] |= (byte) Direction.North;
                        sizeEstimate += 4;
                    }

                    //Check ox
                    if (x > 0 && blocks[index - Chunk.BlockPerChunk * Chunk.BlockPerChunk] == Block.Air) {
                        faces[index] |= (byte) Direction.West;
                        sizeEstimate += 4;
                    }

                    if (x < Chunk.BlockPerChunk - 1 && blocks[index + Chunk.BlockPerChunk * Chunk.BlockPerChunk] == Block.Air) {
                        faces[index] |= (byte) Direction.East;
                        sizeEstimate += 4;
                    }

                    if (x == 0 && (exists[3] == false || neighbors[3].GetBlockAt (position.x + ox - 1, position.y + oy, position.z + oz) == Block.Air)) {
                        faces[index] |= (byte) Direction.West;
                        sizeEstimate += 4;
                    }

                    if (x == Chunk.BlockPerChunk - 1 && (exists[1] == false || neighbors[1].GetBlockAt (position.x + ox + 1, position.y + oy, position.z + oz) == Block.Air)) {
                        faces[index] |= (byte) Direction.East;
                        sizeEstimate += 4;
                    }

                    //Check oy

                    if (y > 0 && blocks[index - Chunk.BlockPerChunk] == Block.Air) {
                        faces[index] |= (byte) Direction.Down;
                        sizeEstimate += 4;
                    }

                    if (y < Chunk.BlockPerChunk - 1 && blocks[index + Chunk.BlockPerChunk] == Block.Air) {
                        faces[index] |= (byte) Direction.Up;
                        sizeEstimate += 4;
                    }

                    if (y == 0 && (exists[5] == false || neighbors[5].GetBlockAt (position.x + ox, position.y + oy - 1, position.z + oz) == Block.Air)) {
                        faces[index] |= (byte) Direction.Down;
                        sizeEstimate += 4;
                    }

                    if (y == Chunk.BlockPerChunk - 1 && (exists[4] == false || neighbors[4].GetBlockAt (position.x + ox, position.y + oy + 1, position.z + oz) == Block.Air)) {
                        faces[index] |= (byte) Direction.Up;
                        sizeEstimate += 4;
                    }

                    index++;
                }
            }
        }

        index = 0;

        vertices = new Vector3[sizeEstimate];
        uvs = new Vector2[sizeEstimate];
        triangles = new int[(int) (sizeEstimate * 1.5f)];

        //generate mesh
        for (int x = 0; x < Chunk.BlockPerChunk; x++) {
            for (int y = 0; y < Chunk.BlockPerChunk; y++) {
                for (int z = 0; z < Chunk.BlockPerChunk; z++) {

                    if (faces[index] == 0) {
                        index++;
                        continue;
                    }
                    
                    float ox = x*offset;
                    float oy = y*offset;
                    float oz = z*offset;
                    

                    if ((faces[index] & (byte) Direction.South) != 0) {
                        vertices[vertexIndex] = new Vector3 (ox + position.x, oy + position.y, oz + position.z);
                        vertices[vertexIndex + 1] = new Vector3 (ox + position.x + offset, oy + position.y + 0, oz + position.z);
                        vertices[vertexIndex + 2] = new Vector3 (ox + position.x + 0, oy + position.y + offset, oz + position.z);
                        vertices[vertexIndex + 3] = new Vector3 (ox + position.x + offset, oy + position.y + offset, oz + position.z);

                        triangles[trianglesIndex] = vertexIndex + 1;
                        triangles[trianglesIndex + 1] = vertexIndex + 2;
                        triangles[trianglesIndex + 2] = vertexIndex + 3;

                        triangles[trianglesIndex + 3] = vertexIndex + 1;
                        triangles[trianglesIndex + 4] = vertexIndex;
                        triangles[trianglesIndex + 5] = vertexIndex + 2;

                        TextureController.AddTextures (blocks[index], Direction.South, vertexIndex, uvs);

                        vertexIndex += 4;
                        trianglesIndex += 6;
                    }

                    if ((faces[index] & (byte) Direction.North) != 0) {
                        vertices[vertexIndex] = new Vector3 (ox + position.x, oy + position.y, oz + position.z + offset);
                        vertices[vertexIndex + 1] = new Vector3 (ox + position.x + offset, oy + position.y + 0, oz + position.z + offset);
                        vertices[vertexIndex + 2] = new Vector3 (ox + position.x + 0, oy + position.y + offset, oz + position.z + offset);
                        vertices[vertexIndex + 3] = new Vector3 (ox + position.x + offset, oy + position.y + offset, oz + position.z + offset);

                        triangles[trianglesIndex] = vertexIndex + 1;
                        triangles[trianglesIndex + 1] = vertexIndex + 2;
                        triangles[trianglesIndex + 2] = vertexIndex;

                        triangles[trianglesIndex + 3] = vertexIndex + 1;
                        triangles[trianglesIndex + 4] = vertexIndex + 3;
                        triangles[trianglesIndex + 5] = vertexIndex + 2;

                        TextureController.AddTextures (blocks[index], Direction.North, vertexIndex, uvs);

                        vertexIndex += 4;
                        trianglesIndex += 6;
                    }

                    if ((faces[index] & (byte) Direction.East) != 0) {
                        vertices[vertexIndex + 0] = new Vector3 (ox + position.x + offset, oy + position.y, oz + position.z);
                        vertices[vertexIndex + 1] = new Vector3 (ox + position.x + offset, oy + position.y, oz + position.z + offset);
                        vertices[vertexIndex + 2] = new Vector3 (ox + position.x + offset, oy + position.y + offset, oz + position.z);
                        vertices[vertexIndex + 3] = new Vector3 (ox + position.x + offset, oy + position.y + offset, oz + position.z + offset);

                        triangles[trianglesIndex + 3] = vertexIndex + 1;
                        triangles[trianglesIndex + 4] = vertexIndex + 2;
                        triangles[trianglesIndex + 5] = vertexIndex + 3;

                        triangles[trianglesIndex + 0] = vertexIndex + 1;
                        triangles[trianglesIndex + 1] = vertexIndex;
                        triangles[trianglesIndex + 2] = vertexIndex + 2;

                        TextureController.AddTextures (blocks[index], Direction.East, vertexIndex, uvs);

                        vertexIndex += 4;
                        trianglesIndex += 6;
                    }

                    if ((faces[index] & (byte) Direction.West) != 0) {
                        vertices[vertexIndex] = new Vector3 (ox + position.x, oy + position.y, oz + position.z);
                        vertices[vertexIndex + 1] = new Vector3 (ox + position.x, oy + position.y, oz + position.z + offset);
                        vertices[vertexIndex + 2] = new Vector3 (ox + position.x, oy + position.y + offset, oz + position.z);
                        vertices[vertexIndex + 3] = new Vector3 (ox + position.x, oy + position.y + offset, oz + position.z + offset);

                        triangles[trianglesIndex] = vertexIndex + 1;
                        triangles[trianglesIndex + 1] = vertexIndex + 2;
                        triangles[trianglesIndex + 2] = vertexIndex;

                        triangles[trianglesIndex + 3] = vertexIndex + 1;
                        triangles[trianglesIndex + 4] = vertexIndex + 3;
                        triangles[trianglesIndex + 5] = vertexIndex + 2;

                        TextureController.AddTextures (blocks[index], Direction.West, vertexIndex, uvs);

                        vertexIndex += 4;
                        trianglesIndex += 6;
                    }

                    if ((faces[index] & (byte) Direction.Up) != 0) {
                        vertices[vertexIndex] = new Vector3 (ox + position.x, oy + position.y + offset, oz + position.z);
                        vertices[vertexIndex + 1] = new Vector3 (ox + position.x, oy + position.y + offset, oz + position.z + offset);
                        vertices[vertexIndex + 2] = new Vector3 (ox + position.x + offset, oy + position.y + offset, oz + position.z);
                        vertices[vertexIndex + 3] = new Vector3 (ox + position.x + offset, oy + position.y + offset, oz + position.z + offset);

                        triangles[trianglesIndex] = vertexIndex + 1;
                        triangles[trianglesIndex + 1] = vertexIndex + 2;
                        triangles[trianglesIndex + 2] = vertexIndex;

                        triangles[trianglesIndex + 3] = vertexIndex + 1;
                        triangles[trianglesIndex + 4] = vertexIndex + 3;
                        triangles[trianglesIndex + 5] = vertexIndex + 2;

                        TextureController.AddTextures (blocks[index], Direction.Up, vertexIndex, uvs);

                        vertexIndex += 4;
                        trianglesIndex += 6;
                    }

                    if ((faces[index] & (byte) Direction.Down) != 0) {
                        vertices[vertexIndex] = new Vector3 (ox + position.x, oy + position.y, oz + position.z);
                        vertices[vertexIndex + 1] = new Vector3 (ox + position.x, oy + position.y, oz + position.z + offset);
                        vertices[vertexIndex + 2] = new Vector3 (ox + position.x + offset, oy + position.y, oz + position.z);
                        vertices[vertexIndex + 3] = new Vector3 (ox + position.x + offset, oy + position.y, oz + position.z + offset);

                        triangles[trianglesIndex] = vertexIndex + 1;
                        triangles[trianglesIndex + 1] = vertexIndex + 2;
                        triangles[trianglesIndex + 2] = vertexIndex + 3;

                        triangles[trianglesIndex + 3] = vertexIndex + 1;
                        triangles[trianglesIndex + 4] = vertexIndex;
                        triangles[trianglesIndex + 5] = vertexIndex + 2;

                        TextureController.AddTextures (blocks[index], Direction.Down, vertexIndex, uvs);

                        vertexIndex += 4;
                        trianglesIndex += 6;
                    }

                    index++;

                }
            }
        }

    }

    public Mesh GetMesh (ref Mesh copy) {

        if (copy == null)
            copy = new Mesh ();
        else copy.Clear ();

        //Debug.Log(vertexIndex);
        if (isVisible == false || vertexIndex == 0) return copy;

        if (vertexIndex > 65000) copy.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        copy.vertices = vertices;
        copy.uv = uvs;
        copy.triangles = triangles;

        copy.RecalculateNormals ();
        copy.RecalculateBounds ();

        return copy;
    }

}