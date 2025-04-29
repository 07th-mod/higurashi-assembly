using Assets.Scripts.Core.AssetManagement;
using MOD.Debugging;
using System;
using System.Collections.Generic;
using System.Text;

namespace MOD.ImageMapping
{
	// This class contains any data that needs to be saved/restored from the save file
	// so that the Image Mapping works correctly after you load a save
	internal class MODImageMappingSaveData
	{
		// Image Mapping requires knowing the last voice file played. When you load a save,
		// you're placed in an arbitrary point in the script, so the last played voice file is not known.
		// So need to restore the last played voice file when the game was saved.
		public string ImageMappingLastVoiceNoExt { get; set; }

		public static MODImageMappingSaveData GetDataToSave(AssetManager assetManager)
		{
			return new MODImageMappingSaveData()
			{
				ImageMappingLastVoiceNoExt = assetManager.ImageMappingLastVoiceNoExt
			};
		}

		public static void LoadSavedData(MODImageMappingSaveData modImageMappingSaveData, AssetManager assetManager)
		{
			assetManager.ImageMappingLastVoiceNoExt = modImageMappingSaveData.ImageMappingLastVoiceNoExt;
			MODDebugSpriteMapping.RecordLastVoiceLoadedFromSaveFile(modImageMappingSaveData.ImageMappingLastVoiceNoExt);
		}
	}
}
