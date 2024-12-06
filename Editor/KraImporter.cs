using System;
using System.IO;
using System.IO.Compression;
using UnityEditor;
using UnityEngine;
using UnityEditor.AssetImporters;
using UnityEngine.Serialization;

namespace HNL
{
    [ScriptedImporter(1, "kra")]
    public class KraImporter : ScriptedImporter
    {
        public enum TextureType
        {
            Texture2D,
            Sprite,
        }
        
        public TextureType textureType = TextureType.Texture2D;
        
        public SpriteSettings spriteSettings = new SpriteSettings();
        
        public TextureFormat textureFormat = TextureFormat.ARGB32;
        public int mipCount = -1;
        public bool isLinearColor = false;
        public bool createUninitialized = false;
        public bool useMipmapLimit = false;
        public string mipmapLimitGroupName = string.Empty;
        
        public TextureWrapMode wrapMode = TextureWrapMode.Repeat;
        public FilterMode filterMode = FilterMode.Bilinear;
        [Range(0, 16)] public int anisoLevel = 1;


        public override void OnImportAsset(AssetImportContext ctx)
        {
            var path = ctx.assetPath;

            using (ZipArchive archive = ZipFile.OpenRead(path))
            {
                var mergedImageEntry = archive.GetEntry("mergedimage.png");

                if (mergedImageEntry == null)
                {
                    Debug.LogWarning($"{path} 不含 mergedimage.png\n{path} has no mergedimage.png");
                }
                else
                {
                    using (DeflateStream stream = (DeflateStream)mergedImageEntry.Open())
                    {
                        int byteLength = (int)stream.BaseStream.Length;
                        byte[] imageByte = new byte[byteLength];
                        stream.Read(imageByte, 0, byteLength);

                        Texture2D texture = new Texture2D(
                            1,
                            1,
                            textureFormat,
                            mipCount,
                            isLinearColor,
                            createUninitialized,
                            new MipmapLimitDescriptor(useMipmapLimit, mipmapLimitGroupName)
                            );
                        ImageConversion.LoadImage(texture, imageByte);

                        texture.wrapMode = wrapMode;
                        texture.filterMode = filterMode;
                        texture.anisoLevel = anisoLevel;
                        texture.Apply();

                        ctx.AddObjectToAsset("merged image", texture, texture);
                        ctx.SetMainObject(texture);

                        if (textureType == TextureType.Sprite)
                        {
                            var sprite = Sprite.Create(
                                texture,
                                spriteSettings.GetRect(texture),
                                spriteSettings.pivot,
                                spriteSettings.pixelsPerUnit,
                                spriteSettings.extrude,
                                spriteSettings.meshType,
                                spriteSettings.GetBorder(texture)
                                );
                            sprite.name = Path.GetFileNameWithoutExtension(path);
                            sprite.hideFlags = HideFlags.None;
                            ctx.AddObjectToAsset("sprite", sprite);
                        }
                    }
                }
            }
        }
        
        [Serializable]
        public class SpriteSettings
        {
            public Rect normalizedRect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
            public Vector2 pivot = new Vector2(0.5f, 0.5f);
            public float pixelsPerUnit = 100.0f;
            public uint extrude = 0;
            public SpriteMeshType meshType = SpriteMeshType.Tight;
            public Vector4 normalizedBorder = Vector4.zero;
            public bool generateFallbackPhysicsShape = false;


            public Rect GetRect(int width, int height)
            {
                return new Rect(
                    normalizedRect.x * width,
                    normalizedRect.y * height,
                    normalizedRect.width * width,
                    normalizedRect.height * height
                );
            }
            
            public Rect GetRect(Texture2D texture)
            {
                return GetRect(texture.width, texture.height);
            }

            public Vector4 GetBorder(int width, int height)
            {
                return new Vector4(
                    normalizedBorder.x * width,
                    normalizedBorder.y * height,
                    normalizedBorder.z * width,
                    normalizedBorder.w * height
                );
            }

            public Vector4 GetBorder(Texture2D texture)
            {
                return GetBorder(texture.width, texture.height);
            }
            
        }
    }
}
