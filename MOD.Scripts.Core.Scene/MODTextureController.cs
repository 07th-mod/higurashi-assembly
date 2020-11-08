using Assets.Scripts.Core;
using Assets.Scripts.Core.AssetManagement;
using Assets.Scripts.Core.Buriko;
using Assets.Scripts.Core.Scene;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MOD.Scripts.Core.Scene
{
	public class MODTextureController
	{
		private IDictionary<int, string> _layerTexture;

		public MODTextureController()
		{
			Initialize();
		}

		public void Initialize()
		{
			_layerTexture = new Dictionary<int, string>();
		}

		public void StoreLayerTexture(int layer, string texture)
		{
			_layerTexture[layer] = texture;
		}

		public void RestoreTextures()
		{
			foreach (int key in _layerTexture.Keys)
			{
				string text = _layerTexture[key];
				Layer layer = GameSystem.Instance.SceneController.GetLayer(key);
				if (!string.IsNullOrEmpty(layer.PrimaryName))
				{
					if (BurikoMemory.Instance.GetGlobalFlag("GArtStyle").IntValue() == 0 && BurikoMemory.Instance.GetGlobalFlag("GLipSync").IntValue() == 1)
					{
						text = text.Substring(0, text.Length - 1) + "0";
					}
					layer.PrimaryName = text;
				}
			}
		}

		public void ToggleArtStyle(bool allowMoreThan2 = true)
		{
			int max = allowMoreThan2 ? AssetManager.Instance.ArtsetCount : 2;
			AssetManager.Instance.CurrentArtsetIndex += 1;
			if (AssetManager.Instance.CurrentArtsetIndex >= max)
			{
				AssetManager.Instance.CurrentArtsetIndex = 0;
			}

			SetArtStyle(AssetManager.Instance.CurrentArtsetIndex);
		}

		public void SetArtStyle(int artSetIndex)
		{
			if(artSetIndex >= 0 && artSetIndex < AssetManager.Instance.ArtsetCount)
			{
				AssetManager.Instance.CurrentArtsetIndex = artSetIndex;
				BurikoMemory.Instance.SetGlobalFlag("GArtStyle", AssetManager.Instance.CurrentArtsetIndex);
				BurikoMemory.Instance.SetGlobalFlag("GBackgroundSet", 0);
				RestoreTextures();
				GameSystem.Instance.SceneController.ReloadAllImages();
				UI.MODToaster.Show($"Art Style: {AssetManager.Instance.CurrentArtset.nameEN}");
			}
		}

		public int GetArtStyle() => AssetManager.Instance.CurrentArtsetIndex;

		public GUIContent[] GetArtStyleDescriptions()
		{
			return AssetManager.Instance.Artsets.Select(x => new GUIContent(x.nameEN, x.nameEN)).ToArray();
		}
	}
}
