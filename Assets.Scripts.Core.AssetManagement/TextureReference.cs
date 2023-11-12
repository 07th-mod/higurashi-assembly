using UnityEngine;

namespace Assets.Scripts.Core.AssetManagement
{
	public class TextureReference
	{
		public Texture2D Texture;

		public int Count;

		// For the mod, we sometimes use the texture path to decide what effects should be applied to a texture
		// When retrieving a texture via a cached TextureReference, the path is not evaluated, so we need to
		// also cache the texture path.
		public string MODTexturePath;
	}
}
