using System;
using System.Collections.Generic;
using System.Text;

namespace MOD.Scripts.UI
{
	/// <summary>
	/// This class aids in enabling/disabling a group of flags at once
	/// </summary>
	class MODCustomFlagPreset
	{
		/// <summary>
		/// This special flag/key will be used to determine if the custom preset is enabled or not
		/// It should start with a $ sign, to avoid conflicting with normal flags.
		/// </summary>
		const string ENABLED_KEY = "$customPresetEnabled";

		/// <summary>
		/// This list contains flags which will be saved/restored with the custom preset
		/// </summary>
		List<string> FLAGS_TO_SERIALIZE = new List<string>
				{
					"GLipSync",
					"GBackgroundSet",
					"GArtStyle",
					"GADVMode",
					"GLinemodeSp",
					"GRyukishiMode",
					"GHideCG",
					"GStretchBackgrounds"
				};

		// All of this class's state is kept in this Dictionary, for easy serialization.
		// To restore the state, re-assign this variable with the new data.
		public Dictionary<string, int> Flags { get; set; }

		public MODCustomFlagPreset()
		{
			this.Flags = new Dictionary<string, int>();
		}

		/// <summary>
		/// This keeps track of whether the custom preset is enabled.
		/// </summary>
		public bool Enabled
		{
			get
			{
				if (Flags.TryGetValue(ENABLED_KEY, out int value))
				{
					return value == 1;
				}
				else
				{
					return false;
				}
			}
		}

		public void EnablePreset(bool restorePresetFromMemory)
		{
			if(restorePresetFromMemory)
			{
				RestorePresetFromMemory();
			}

			Flags[ENABLED_KEY] = 1;
		}

		/// <summary>
		/// Copy global flags to preset every time preset mod changes from enabled -> disabled.
		// This ensure flags are saved when you switch to another preset.
		// This doesn't handle the case when exiting the game, which must be handled manually using SavePresetToMemory().
		/// </summary>
		public void DisablePresetAndSavePresetToMemory()
		{
			// Save all preset-related flags to memory
			if(Enabled)
			{
				SavePresetToMemory();
			}

			// Set the special "enabled" key to false in memory
			Flags[ENABLED_KEY] = 0;
		}

		public void SavePresetToMemory()
		{
			foreach (string flagName in FLAGS_TO_SERIALIZE)
			{
				Flags[flagName] = Assets.Scripts.Core.Buriko.BurikoMemory.Instance.GetGlobalFlag(flagName).IntValue();
			}
		}

		private void RestorePresetFromMemory()
		{
			foreach (KeyValuePair<string, int> pair in Flags)
			{
				if (pair.Key.StartsWith("$"))
				{
					continue;
				}

				Assets.Scripts.Core.Buriko.BurikoMemory.Instance.SetGlobalFlag(pair.Key, pair.Value);
			}
		}
	}
}
