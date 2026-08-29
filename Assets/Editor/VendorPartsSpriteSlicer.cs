using UnityEditor;
using UnityEngine;

namespace Editor
{
    public static class VendorPartsSpriteSlicer
    {
        private const string TargetFolder = "Assets/Resources/Appearance/VendorParts";
        private const float PixelsPerUnit = 16f;

        [MenuItem("Tools/Slice VendorParts Sprites")]
        public static void SliceVendorParts()
        {
            string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { TargetFolder });
            int count = 0;

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null) continue;

                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single; // grid c:1 r:1 = tek sprite, tüm texture
                importer.spritePixelsPerUnit = PixelsPerUnit;
                importer.filterMode = FilterMode.Point;
                importer.mipmapEnabled = false;

                TextureImporterSettings settings = new TextureImporterSettings();
                importer.ReadTextureSettings(settings);
                settings.spriteMeshType = SpriteMeshType.FullRect;
                importer.SetTextureSettings(settings);

                TextureImporterPlatformSettings platformSettings = importer.GetDefaultPlatformTextureSettings();
                platformSettings.format = TextureImporterFormat.RGBA32; // None ~ uncompressed
                platformSettings.textureCompression = TextureImporterCompression.Uncompressed;
                platformSettings.overridden = true;
                importer.SetPlatformTextureSettings(platformSettings);

                EditorUtility.SetDirty(importer);
                importer.SaveAndReimport();
                count++;
            }

            AssetDatabase.Refresh();
            Debug.Log($"VendorParts: {count} sprite güncellendi.");
        }
    }
}