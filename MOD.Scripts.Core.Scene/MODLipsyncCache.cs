using Assets.Scripts.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MOD.Scripts.Core.Scene
{
    class MODLipsyncCache
    {
        struct LastDrawInformation
        {
            public readonly int layer;
            public readonly string baseTextureName;

            public LastDrawInformation(int layer, string baseTextureName)
            {
                this.layer = layer;
                this.baseTextureName = baseTextureName;
            }
        }

        class LastDrawInformationManager
        {
            private static bool[] infoValid = new bool[MODSceneController.MAX_CHARACTERS];
            private static LastDrawInformation[] lastDrawInformation = new LastDrawInformation[MODSceneController.MAX_CHARACTERS];

            public static bool GetLastDrawInformation(int character, out LastDrawInformation drawInformation)
            {
                if (character < lastDrawInformation.Length && infoValid[character])
                {
                    drawInformation = lastDrawInformation[character];
                    return true;
                }

                drawInformation = new LastDrawInformation();
                return false;
            }

            public static void SetLastDrawInformation(int character, int layer, string baseTextureName)
            {
                if (character < lastDrawInformation.Length)
                {
                    lastDrawInformation[character] = new LastDrawInformation(layer, baseTextureName);
                    infoValid[character] = true;
                }
            }
        }

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

            /// <summary>
            /// The texture of the character with a half open mouth, ending with "1"
            /// For example, "aka_def_1.png"
            /// </summary>
            public Texture2D halfOpen_1;

            /// <summary>
            /// The texture of the character with a fully open mouth, ending with "2"
            /// For example, "aka_def_2.png"
            /// </summary>
            public Texture2D fullOpen_2;

            /// <summary>
            /// How long since this texture group has been used. This is incremented each time
            /// any character is drawn to the screen, and reset when the texture group is used.
            /// </summary>
            public int age;

            public TextureGroup(Texture2D baseTexture_0, Texture2D halfOpen_1, Texture2D fullOpen_2)
            {
                this.baseTexture_0 = baseTexture_0;
                this.halfOpen_1 = halfOpen_1;
                this.fullOpen_2 = fullOpen_2;

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
                if(halfOpen_1 != null)
                {
                    GameObject.Destroy(halfOpen_1);
                }
                if(fullOpen_2 != null)
                {
                    GameObject.Destroy(fullOpen_2);
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
                return $"TG[age: {age} base: {Print(baseTexture_0)} half: {Print(halfOpen_1)} full: {Print(fullOpen_2)}]";
            }
        }

        private static readonly Dictionary<string, TextureGroup> cache = new Dictionary<string, TextureGroup>();
        private static int maxTextureAge = 2;
        private static string debugLastEvent;

        public static void MODLipsyncCacheUpdate(Texture2D baseTexture, int character, int layer, string baseTextureName)
        {
            // This must always run even if lipsync is disabled, so that the lastDrawInformation is present if enabled later
            LastDrawInformationManager.SetLastDrawInformation(character, layer, baseTextureName);

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
            bool _ = LoadOrUseCache(baseTexture, character, out TextureGroup _);
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
        public static bool LoadOrUseCache(Texture2D maybeBaseTexture, int character, out TextureGroup textureGroup)
        {
            try
            {
                DebugLog($"Texture Cache count: {cache.Keys.Count}");

                if(!LastDrawInformationManager.GetLastDrawInformation(character, out LastDrawInformation info))
                {
                    textureGroup = null;
                    return false;
                }

                if (cache.TryGetValue(info.baseTextureName, out TextureGroup cachedTextures))
                {
                    DebugLog($"LoadOrUseCache() - Cache hit on [{info.baseTextureName}]");

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
                        cache.Remove(info.baseTextureName);

                        textureGroup = LoadWithoutCache(info, maybeBaseTexture);
                        return true;
                    }

                    // Since we just used this texture, reset its age to 0
                    cachedTextures.age = 0;

                    textureGroup = cachedTextures;
                    return true;
                }
                else
                {
                    textureGroup = LoadWithoutCache(info, maybeBaseTexture);
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.Log($"Lipsync LoadOrUseCache() ERROR: {e}");
                textureGroup = null;
                return false;
            }
        }
        private static TextureGroup LoadWithoutCache(LastDrawInformation info, Texture2D maybeBaseTexture)
        {
            Texture2D baseTexture = maybeBaseTexture;

            if (baseTexture == null)
            {
                DebugLog($"LoadOrUseCache() - loading base texture from scratch ");
                baseTexture = MODSceneController.LoadTextureWithFilters(info.layer, info.baseTextureName + "0");
            }

            DebugLog($"LoadOrUseCache() - updating cache with texture: {info.baseTextureName} on layer {info.layer}");
            TextureGroup textureGroup = new TextureGroup(
                baseTexture,
                MODSceneController.LoadTextureWithFilters(info.layer, info.baseTextureName + "1"),
                MODSceneController.LoadTextureWithFilters(info.layer, info.baseTextureName + "2")
            );

            cache.Add(info.baseTextureName, textureGroup);
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

            foreach (KeyValuePair<string, TextureGroup> kvp in cache)
            {
                sb.AppendLine($"---- {kvp.Key} ----\n{kvp.Value}\n");
            }

            return sb.ToString();
        }

        public static void SetMaxTextureAge(int maxAge)
        {
            maxTextureAge = maxAge;
        }
    }
}
