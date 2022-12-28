using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace SmokGnu.SpriteSheetRenderer.Render
{
    public class SpriteSheetCache
    {
        readonly Shader m_spriteSheetShader;
        readonly Dictionary<string, KeyValuePair<Material, int>> materialNameMaterial = new();
        readonly Dictionary<Material, string> materialToName = new();

        public SpriteSheetCache(Shader spriteSheetShader)
        {
            m_spriteSheetShader = spriteSheetShader;
        }

        public KeyValuePair<Material, float4[]> BakeSprites(Sprite[] sprites, string materialName)
        {
            var material = new Material(m_spriteSheetShader);
            Texture texture = sprites[0].texture;
            material.mainTexture = texture;

            if (sprites.Length == 1)
                return BakeSprite(sprites[0], materialName);

            float w = texture.width;
            float h = texture.height;
            var uvs = new float4[sprites.Length];
            var i = 0;
            foreach (var s in sprites)
            {
                var tilingX = 1f / (w / s.rect.width);
                var tilingY = 1f / (h / s.rect.height);
                var OffsetX = tilingX * (s.rect.x / s.rect.width);
                var OffsetY = tilingY * (s.rect.y / s.rect.height);
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
            var material = new Material(m_spriteSheetShader);
            Texture texture = sprite.texture;
            material.mainTexture = texture;
            float w = texture.width;
            float h = texture.height;

            const int length = 1;
            var uvs = new float4[length];
            var tilingX = 1f / (w / sprite.rect.width);
            var tilingY = 1f / (h / sprite.rect.height);
            var OffsetX = tilingX * (sprite.rect.x / sprite.rect.width);
            var OffsetY = tilingY * (sprite.rect.y / sprite.rect.height);
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
        public int GetLength(Material material) => GetLength(GetMaterialName(material));
    }
}