using Assets.Scripts.Core;
using Assets.Scripts.Core.AssetManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MOD.Scripts.Core.Scene
{
    // As Hou+ already provides some texture caching, some of the logic for this file has been reworked
    // Now instead of applying caching ontop of Unity's caching system, it applies caching ontop of
    // the game engine's caching.
    class MODLipsyncCache
    {
        public class TextureGroup
        {
            // Note: the textures in this class can become null at any time, if
            // Destroy() is called somewhere else in the game code on this texture
            // This typically happens when a ReleaseTextures() is called on a layer
            // (when the character is cleared from the screen)

            /// <summary>
            /// The texture of the character with a closed mouth, ending with "0".
            /// For example, "aka_def_0.png"
            /// </summary>
            public Texture2D baseTexture_0;
            public string baseTexture_0_name;

            /// <summary>
            /// The texture of the character with a half open mouth, ending with "1"
            /// For example, "aka_def_1.png"
            /// </summary>
            public Texture2D halfOpen_1;
            public string halfOpen_1_name;

            /// <summary>
            /// The texture of the character with a fully open mouth, ending with "2"
            /// For example, "aka_def_2.png"
            /// </summary>
            public Texture2D fullOpen_2;
            public string fullOpen_2_name;

            /// <summary>
            /// How long since this texture group has been used. This is incremented each time
            /// any character is drawn to the screen, and reset when the texture group is used.
            /// </summary>
            public int age;

            public TextureGroup(
                Texture2D baseTexture_0,
                string baseTexture_0_name,
                Texture2D halfOpen_1,
                string halfOpen_1_name,
                Texture2D fullOpen_2,
                string fullOpen_2_name)
            {
                this.baseTexture_0 = baseTexture_0;
                this.baseTexture_0_name = baseTexture_0_name;
                this.halfOpen_1 = halfOpen_1;
                this.halfOpen_1_name = halfOpen_1_name;
                this.fullOpen_2 = fullOpen_2;
                this.fullOpen_2_name = fullOpen_2_name;

                age = 0;
            }

            public bool NeedsClean()
            {
                return baseTexture_0 == null  || halfOpen_1 == null || fullOpen_2 == null;
            }

            // Note: it is assumed that by the time this function is called, the lipsync animation is completed/not playing
            // In this state, the layer will contain baseTexture_0, which the game will clean up automatically
            // We then manually clean up the other two textures.
            //
            // I have also tested not calling Destroy() at all, and the game seems to clean up the textures anyway,
            // so failing to call Destroy in some circumstances is probably fine.
            public void DestroyTextures()
            {
                TryDestroyTexture(baseTexture_0_name, baseTexture_0);
                TryDestroyTexture(halfOpen_1_name, halfOpen_1);
                TryDestroyTexture(fullOpen_2_name, fullOpen_2);
            }

            private void TryDestroyTexture(string textureName, Texture2D texture)
            {
                if(texture != null)
                {
                    AssetManager.Instance.ReleaseTexture(textureName, texture);
                }
            }

            private string Print(Texture2D tex)
            {
                if(tex != null)
                {
                    return tex.name;
                }

                return "texture is null";
            }

            public override string ToString()
            {
                return $"TG[age: {age} base: {baseTexture_0_name}/{Print(baseTexture_0)} half: {halfOpen_1_name}/{Print(halfOpen_1)} full: {fullOpen_2_name}/{Print(fullOpen_2)}]";
            }
        }

        private static readonly Dictionary<string, TextureGroup> cache = new Dictionary<string, TextureGroup>();
        private static int maxTextureAge = 2;
        private static string debugLastEvent;

        public static void MODLipsyncCacheUpdate(int character)
        {
            // If lipsync not enabled, do not do any caching
            if (!MODSystem.instance.modSceneController.MODLipSyncIsEnabled())
            {
                return;
            }

            // Firstly, tidy up the texture cache, by clearing any texture groups
            // where any one of the textures in that group have been Destroy()ed
            // Also clear any textures which are too old
            List<string> texturesToRemove = new List<string>();
            foreach (string key in cache.Keys)
            {
                TextureGroup tex = cache[key];

                // NOTE: This if statement is usually never called, but I have left
                // it in just in case the base texture is deleted when we still want to use it
                // See the note at the top of the 'TextureGroup' class.
                if (tex.NeedsClean())
                {
                    tex.DestroyTextures();
                    texturesToRemove.Add(key);
                }

                if (tex.age > maxTextureAge)
                {
                    tex.DestroyTextures();
                    texturesToRemove.Add(key);
                }
                else
                {
                    tex.age++;
                }
            }

            foreach (string key in texturesToRemove)
            {
                cache.Remove(key);
            }

            // Do not cache while skipping
            if (GameSystem.Instance.IsSkipping)
            {
                return;
            }

            //Now pre-load the textures for the character that is about to be drawn
            LoadOrUseCache(character, out TextureGroup _);
        }

        /// <summary>
        /// Loads the mouth textures, attempting to load from the given layer OR cache if possible
        ///
        /// This function loads the mouth textures in the following manner:
        /// - If the mouth textures already exist in the cache, it will use those
        /// - Otherwise for the base texture (mouth closed):
        ///   - it will try to take the texture from the "layerWithCharacter" layer
        ///   - if that fails, it will try to load it from scratch (from disk)
        /// - For the other mouth textures, it will just load them from disk
        ///
        /// NOTE: The passed in 'layer' must be a layer containing the loaded character sprite
        /// the base texture for the character will be sourced from this layer.
        /// You may need to use GameSystem.Instance.RegisterAction(delegate {}) to ensure things
        /// happen in the correct order.
        /// Set 'layer' to null if you don't want to load the base texture from an existing layer
        /// </summary>
        /// <param name="maybeBaseTexture">This function will use this argument as the 'base' lipsync texture.
        /// If you don't have access to the base lipsync texture, pass in null to load it from from disk.</param>
        /// <param name="character">The number of the character whose textures you want to load.
        /// This is the same character number used in the game scripts.</param>
        /// <returns></returns>
        public static bool LoadOrUseCache(int character, out TextureGroup textureGroup)
        {
            DebugLog($"Texture Cache count: {cache.Keys.Count}");
            string textureName = MODSystem.instance.modSceneController.GetBaseTextureName(character);

            // If the textureName is null, we won't know what texture to load, so just give up
            if (textureName == null)
            {
                MODLogger.Log($"WARNING: textureName = null for character {character}", true);
                textureGroup = null;
                return false;
            }

            try
            {
                if (cache.TryGetValue(textureName, out TextureGroup cachedTextures))
                {
                    DebugLog($"LoadOrUseCache() - Cache hit on [{textureName}]");

                    // This branch happens if the texture group exists in the cache, but one or more of the textures
                    // have been Destroy()ed (set to null).
                    //
                    // I've managed to hit this branch once? before when skippping and clicking at the same time,
                    // otherwise it doesn't seem to happen
                    if (cachedTextures.NeedsClean())
                    {
                        Assets.Scripts.Core.Logger.LogError($"WARNING on LoadOrUseCache() - retrieved texture but it was Destroy()ed");

                        // Clean up the texture, then reload it from disk
                        cachedTextures.DestroyTextures();
                        cache.Remove(textureName);
                        textureGroup = LoadWithoutCache(textureName, character);
                        return true;
                    }

                    // Since we just used this texture, reset its age to 0
                    cachedTextures.age = 0;

                    textureGroup = cachedTextures;
                    return true;
                }
                else
                {
                    textureGroup = LoadWithoutCache(textureName, character);
                    return true;
                }
            }
            catch(Exception)
            {
                MODLogger.Log($"WARNING: LoadOrUseCache failed! character: {character}", true);
                textureGroup = null;
                return false;
            }
        }

        private static TextureGroup LoadWithoutCache(string textureName, int character)
        {
            DebugLog($"LoadOrUseCache() - updating cache with char: {character}");

            Texture2D baseTexture = MODSystem.instance.modSceneController.MODLipSyncPrepare(character, "0", out string baseTextureName);
            Texture2D halfOpenTexture = MODSystem.instance.modSceneController.MODLipSyncPrepare(character, "1", out string halfOpenTextureName);
            Texture2D fullOpenTexture = MODSystem.instance.modSceneController.MODLipSyncPrepare(character, "2", out string fullOpenTextureName);

            TextureGroup textureGroup = new TextureGroup(
                baseTexture,
                baseTextureName,
                halfOpenTexture,
                halfOpenTextureName,
                fullOpenTexture,
                fullOpenTextureName
            );

            cache.Add(textureName, textureGroup);
            return textureGroup;
        }

        private static void DebugLog(string text)
        {
            MODUtility.FlagMonitorOnlyLog(text);
            debugLastEvent = text;
        }

        public static string DebugInfo()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"LIPSYNC CACHE [Num Entries: {cache.Count()} Max Age: {maxTextureAge}]");
            sb.AppendLine(debugLastEvent);

            foreach ((string key, TextureGroup value) in cache)
            {
                sb.AppendLine($"---- {key} ----\n{value}\n");
            }

            return sb.ToString();
        }

        public static void SetMaxTextureAge(int maxAge)
        {
            maxTextureAge = maxAge;
        }
    }
}
