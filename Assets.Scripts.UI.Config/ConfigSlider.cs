using Assets.Scripts.Core;
using Assets.Scripts.Core.Buriko;
using UnityEngine;

namespace Assets.Scripts.UI.Config
{
	public class ConfigSlider : MonoBehaviour
	{
		private UISlider slider;

		private float laststep = 0.5f;

		private bool isSet;

		public void Changestep(float change)
		{
			slider.value = Mathf.Clamp01(slider.value + change);
		}

		private void Set()
		{
			if (slider == null)
			{
				slider = GetComponent<UISlider>();
			}
			int num = 0;
			switch (base.name) // All slider labels match original order, functionality in comment
			{
			case "0-TextSpeed": // Window Opacity:
				num = (int)(GameSystem.Instance.MessageWindowOpacity * 100f);
				break;
			case "1-AutoSpeed": // Voice Volume:
				num = (int)(GameSystem.Instance.AudioController.VoiceVolume * 100f);
				break;
			case "2-AutoPageSpeed": // BGM Volume
				num = (int)(GameSystem.Instance.AudioController.BGMVolume * 100f);
				break;
			case "3-WindowOpacity": // SE Volume
				num = (int)(GameSystem.Instance.AudioController.SoundVolume * 100f);
				break;
			case "4-BGMVolume": // Text / Auto text speed
				num = GameSystem.Instance.TextController.TextSpeed;
				break;
			case "5-SEVolume":  // Auto Page Speed
				num = GameSystem.Instance.TextController.AutoPageSpeed;
				break;
			}
			slider.value = (float)num / 100f;
			laststep = slider.value;
			isSet = true;
		}

		private void Update()
		{
			if (slider == null)
			{
				slider = GetComponent<UISlider>();
			}
			if (!isSet)
			{
				Set();
			}
			if (!Mathf.Approximately(laststep, slider.value))
			{
				laststep = slider.value;
				int num = (int)(100f * laststep);
				switch (base.name) // All slider labels match original order, functionality in comment
				{
				case "0-TextSpeed": // Window Opacity
					GameSystem.Instance.MessageWindowOpacity = laststep;
					GameSystem.Instance.MainUIController.SetWindowOpacity(laststep);
					BurikoMemory.Instance.SetGlobalFlag("GWindowOpacity", num);
					break;
				case "1-AutoSpeed": // Voice Volume
					GameSystem.Instance.AudioController.VoiceVolume = laststep;
					GameSystem.Instance.AudioController.RefreshLayerVolumes();
					BurikoMemory.Instance.SetGlobalFlag("GVoiceVolume", num);
					break;
				case "2-AutoPageSpeed": // BGM Volume
					GameSystem.Instance.AudioController.BGMVolume = laststep;
					GameSystem.Instance.AudioController.RefreshLayerVolumes();
					BurikoMemory.Instance.SetGlobalFlag("GBGMVolume", num);
					break;
				case "3-WindowOpacity": // SE Volume
					GameSystem.Instance.AudioController.SoundVolume = laststep;
					GameSystem.Instance.AudioController.SystemVolume = laststep;
					GameSystem.Instance.AudioController.RefreshLayerVolumes();
					BurikoMemory.Instance.SetGlobalFlag("GSEVolume", num);
					break;
				case "4-BGMVolume": // Text / Auto text Speed
					GameSystem.Instance.TextController.TextSpeed = num;
					BurikoMemory.Instance.SetGlobalFlag("GMessageSpeed", num);
					GameSystem.Instance.TextController.AutoSpeed = num;
					BurikoMemory.Instance.SetGlobalFlag("GAutoSpeed", num);
					break;
				case "5-SEVolume": // Auto page speed
					GameSystem.Instance.TextController.AutoPageSpeed = num;
					BurikoMemory.Instance.SetGlobalFlag("GAutoAdvSpeed", num);
					break;
				}
			}
		}
	}
}
