using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Block
{
    Air = 0x0000,
    Stone = 0x0001,
    Grass = 0x0002,
    Dirt = 0x0003,
    Water = 0x0004,
    Sand = 0x0005,
    Log = 0x0006,
    Leaves = 0x0007,
    WoodPlanks = 0x0008
}

public static class Block_Extentions{

    public static bool IsTransparent (this Block block){

        return block == Block.Air;
    

    }

}