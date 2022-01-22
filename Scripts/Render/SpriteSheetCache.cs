using System.Collections;
using System.Collections.Generic;
using Unity.Assertions;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SpriteSheetCache : SingletonBase<SpriteSheetCache>
{
    Shader m_spriteSheetShader;
    Dictionary<string, KeyValuePair<Material, int>> materialNameMaterial = new();
    Dictionary<Material, string> materialToName = new();

    public void Init(Shader spriteSheetShader) { m_spriteSheetShader = spriteSheetShader; }

    public KeyValuePair<Material, float4[]> BakeSprites(Sprite[] sprites, string materialName)
    {
        Material material = new Material(m_spriteSheetShader);
        Texture texture = sprites[0].texture;
        material.mainTexture = texture;

        if (sprites.Length == 1)
            return BakeSprite(sprites[0], materialName);
        
        float w = texture.width;
        float h = texture.height;
        float4[] uvs = new float4[sprites.Length];
        int i = 0;
        foreach (Sprite s in sprites)
        {
            float tilingX = 1f / (w / s.rect.width);
            float tilingY = 1f / (h / s.rect.height);
            float OffsetX = tilingX * (s.rect.x / s.rect.width);
            float OffsetY = tilingY * (s.rect.y / s.rect.height);
            uvs[i].x = tilingX;
            uvs[i].y = tilingY;
            uvs[i].z = OffsetX;
            uvs[i].w = OffsetY;
            i++;
        }

        materialNameMaterial.Add(materialName, new KeyValuePair<Material, int>(material, sprites.Length));
        materialToName.Add(material, materialName);
        return new KeyValuePair<Material, float4[]>(material, uvs);
    }

    public KeyValuePair<Material, float4[]> BakeSprite(Sprite sprite, string materialName)
    {
        Material material = new Material(m_spriteSheetShader);
        Texture texture = sprite.texture;
        material.mainTexture = texture;
        float w = texture.width;
        float h = texture.height;

        const int length = 1;
        var uvs = new float4[length];
        float tilingX = 1f / (w / sprite.rect.width);
        float tilingY = 1f / (h / sprite.rect.height);
        float OffsetX = tilingX * (sprite.rect.x / sprite.rect.width);
        float OffsetY = tilingY * (sprite.rect.y / sprite.rect.height);
        uvs[0].x = tilingX;
        uvs[0].y = tilingY;
        uvs[0].z = OffsetX;
        uvs[0].w = OffsetY;
        materialNameMaterial.Add(materialName, new KeyValuePair<Material, int>(material, length));
        materialToName.Add(material, materialName);
        return new KeyValuePair<Material, float4[]>(material, uvs);
    }

    public int TotalLength() => materialNameMaterial.Count;

    public int GetLength(string spriteSheetName) => materialNameMaterial[spriteSheetName].Value;
    public Material GetMaterial(string spriteSheetName) => materialNameMaterial[spriteSheetName].Key;

    public string GetMaterialName(Material material) => materialToName[material];
    public int GetLenght(Material material) => GetLength(GetMaterialName(material));
}