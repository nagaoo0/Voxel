using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureController
{
    public static Dictionary<string, Vector2[]> textureMap = new Dictionary<string, Vector2[]>();

    public static void Initialize(string texturePath, Texture texture)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites");
        float offset = 0.1f;

        foreach (Sprite s in sprites)
        {

            Vector2[] uvs = new Vector2[4];

            uvs[0] = new Vector2((s.rect.xMin / texture.width) + offset, (s.rect.yMin / texture.height) + offset); //bottom left
            uvs[1] = new Vector2((s.rect.xMax / texture.width) - offset, (s.rect.yMin / texture.height) + offset); //bottom right
            uvs[2] = new Vector2((s.rect.xMin / texture.width) + offset, (s.rect.yMax / texture.height) - offset); //top left
            uvs[3] = new Vector2((s.rect.xMax / texture.width) - offset, (s.rect.yMax / texture.height) - offset); //top right

            textureMap.Add(s.name, uvs);


        }
        //Debug.Log(sprites.Length);
    }

    public static bool AddTextures(Block block, Direction direction, int index, Vector2[] uvs)
    {

        string key = FastGetKey(block, direction);

        Vector2[] text;

        if (textureMap.TryGetValue(key, out text))
        {
            uvs[index] = text[0];
            uvs[index + 1] = text[1];
            uvs[index + 2] = text[2];
            uvs[index + 3] = text[3];

            return true;
        }

        text = textureMap["default"];

        uvs[index] = text[0];
        uvs[index + 1] = text[1];
        uvs[index + 2] = text[2];
        uvs[index + 3] = text[3];

        return false;
    }

    static string FastGetKey(Block block, Direction direction)
    {

        if (block == Block.Stone)
            return "stone";

        if (block == Block.Water)
            return "water";

        if (block == Block.Sand)
            return "sand";

        if (block == Block.Dirt)
            return "dirt";

        if (block == Block.Grass)
        {
            if (direction == Direction.Up)
                return "grassTop";
            if (direction == Direction.Down)
                return "dirt";
            return "grassTop2";
        }

        if (block == Block.Log)
        {
            if (direction == Direction.Up)
                return "LogTop";
            if (direction == Direction.Down)
                return "LogTop";

            return "Log";
        }

        if (block == Block.Leaves)
        {
            return "Leaves1";
        }

        return "default";

    }
}
