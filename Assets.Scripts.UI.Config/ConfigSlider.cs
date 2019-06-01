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
			switch (base.name)
			{
			case "0-WindowOpacity":
				num = (int)(GameSystem.Instance.MessageWindowOpacity * 100f);
				break;
			case "1-VoiceVolume":
				num = (int)(GameSystem.Instance.AudioController.VoiceVolume * 100f);
				break;
			case "2-BGMVolume":
				num = (int)(GameSystem.Instance.AudioController.BGMVolume * 100f);
				break;
			case "3-SEVolume":
				num = (int)(GameSystem.Instance.AudioController.SoundVolume * 100f);
				break;
			case "4-TextSpeed":
				num = GameSystem.Instance.TextController.TextSpeed;
				break;
			case "5-AutoPageSpeed":
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
				switch (base.name)
				{
				case "0-WindowOpacity":
					GameSystem.Instance.MessageWindowOpacity = laststep;
					GameSystem.Instance.MainUIController.SetWindowOpacity(laststep);
					BurikoMemory.Instance.SetGlobalFlag("GWindowOpacity", num);
					break;
				case "1-VoiceVolume":
					GameSystem.Instance.AudioController.VoiceVolume = laststep;
					GameSystem.Instance.AudioController.RefreshLayerVolumes();
					BurikoMemory.Instance.SetGlobalFlag("GVoiceVolume", num);
					break;
				case "2-BGMVolume":
					GameSystem.Instance.AudioController.BGMVolume = laststep;
					GameSystem.Instance.AudioController.RefreshLayerVolumes();
					BurikoMemory.Instance.SetGlobalFlag("GBGMVolume", num);
					break;
				case "3-SEVolume":
					GameSystem.Instance.AudioController.SoundVolume = laststep;
					GameSystem.Instance.AudioController.SystemVolume = laststep;
					GameSystem.Instance.AudioController.RefreshLayerVolumes();
					BurikoMemory.Instance.SetGlobalFlag("GSEVolume", num);
					break;
				case "4-TextSpeed":
					GameSystem.Instance.TextController.TextSpeed = num;
					BurikoMemory.Instance.SetGlobalFlag("GMessageSpeed", num);
					GameSystem.Instance.TextController.AutoSpeed = num;
					BurikoMemory.Instance.SetGlobalFlag("GAutoSpeed", num);
					break;
				case "5-AutoPageSpeed":
					GameSystem.Instance.TextController.AutoPageSpeed = num;
					BurikoMemory.Instance.SetGlobalFlag("GAutoAdvSpeed", num);
					break;
				}
			}
		}
	}
}
