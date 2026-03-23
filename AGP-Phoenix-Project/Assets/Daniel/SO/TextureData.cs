//using System.Linq;
//using UnityEngine;

//[CreateAssetMenu(menuName = "TerrainSO/TextureData")]
//public class TextureData : UpdatableData
//{
//    const int textureSize = 512;
//    const TextureFormat textureFormat = TextureFormat.RGB565;

//    public Layer[] layers;

//    float savedMinHeight;
//    float savedMaxHeight;

//    public void ApplyToMaterial(Material material)
//    {
//        material.SetInt("_LayerCount", layers.Length);

//        material.SetColorArray("_BaseColours",
//            layers.Select(x => x.tint).ToArray());

//        material.SetFloatArray("_BaseStartHeights",
//            layers.Select(x => x.startHeight).ToArray());

//        material.SetFloatArray("_BaseBlends",
//            layers.Select(x => x.blendStrength).ToArray());

//        material.SetFloatArray("_BaseColourStrength",
//            layers.Select(x => x.tintStrength).ToArray());

//        material.SetFloatArray("_BaseTextureScales",
//            layers.Select(x => x.textureScale).ToArray());

//        Texture2DArray texturesArray =
//            GenerateTextureArray(layers.Select(x => x.texture).ToArray());

//        material.SetTexture("_BaseTextures", texturesArray);
        
//        if (savedMinHeight != 0 || savedMaxHeight != 0)
//        {
//            material.SetFloat("_MinHeight", savedMinHeight);
//            material.SetFloat("_MaxHeight", savedMaxHeight);
//        }
//    }

//    public void UpdateMeshHeights(Material material, float minHeight, float maxHeight)
//    {
//        savedMinHeight = minHeight;
//        savedMaxHeight = maxHeight;

//        material.SetFloat("_MinHeight", minHeight);
//        material.SetFloat("_MaxHeight", maxHeight);
//    }

//    Texture2DArray GenerateTextureArray(Texture2D[] textures)
//    {
//        Texture2DArray textureArray =
//            new Texture2DArray(textureSize, textureSize, textures.Length, textureFormat, true);

//        for (int i = 0; i < textures.Length; i++)
//        {
//            textureArray.SetPixels(textures[i].GetPixels(), i);
//        }

//        textureArray.Apply();
//        return textureArray;
//    }

//    [System.Serializable]
//    public class Layer
//    {
//        public Texture2D texture;

//        public Color tint;

//        [Range(0,1)]
//        public float tintStrength;

//        [Range(0,1)]
//        public float startHeight;

//        [Range(0,1)]
//        public float blendStrength;

//        public float textureScale;
//    }
//}