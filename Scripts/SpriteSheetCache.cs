﻿using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SpriteSheetCache : SingletonBase<SpriteSheetCache>
{
    private Dictionary<string, KeyValuePair<Material, int>> materialNameMaterial = new Dictionary<string, KeyValuePair<Material, int>>();
    private Dictionary<Material, string> materialToName = new Dictionary<Material, string>();
    public Dictionary<Entity, SpriteSheetAnimator> entityAnimator = new Dictionary<Entity, SpriteSheetAnimator>();
    Shader m_spriteSheetShader;

    public void Init(Shader spriteSheetShader)
    {
        m_spriteSheetShader = spriteSheetShader;
    }

    public KeyValuePair<Material, float4[]> BakeSprites(Sprite[] sprites, string materialName)
    {
        Material material = new Material(m_spriteSheetShader);
        Texture texture = sprites[0].texture;
        material.mainTexture = texture;
        if (sprites.Length == 1)
        {
            float w = texture.width;
            float h = texture.height;
            float4[] uvs = new float4[sprites.Length];
            float tilingX = 1f / (w / sprites[0].rect.width);
            float tilingY = 1f / (h / sprites[0].rect.height);
            float OffsetX = tilingX * (sprites[0].rect.x / sprites[0].rect.width);
            float OffsetY = tilingY * (sprites[0].rect.y / sprites[0].rect.height);
            uvs[0].x = tilingX;
            uvs[0].y = tilingY;
            uvs[0].z = OffsetX;
            uvs[0].w = OffsetY;
            materialNameMaterial.Add(materialName, new KeyValuePair<Material, int>(material, sprites.Length));
            materialToName.Add(material, materialName);
            return new KeyValuePair<Material, float4[]>(material, uvs);
        }
        else
        {
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
    }

    public int TotalLength() => materialNameMaterial.Count;

    public int GetLength(string spriteSheetName) => materialNameMaterial[spriteSheetName].Value;
    public Material GetMaterial(string spriteSheetName) => materialNameMaterial[spriteSheetName].Key;

    public SpriteSheetAnimator GetAnimator(Entity e) => entityAnimator[e];
    public string GetMaterialName(Material material) => materialToName[material];
    public int GetLenght(Material material) => GetLength(GetMaterialName(material));

}
