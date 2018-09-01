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
			case "0-TextSpeed":
				num = GameSystem.Instance.TextController.TextSpeed;
				break;
			case "1-AutoSpeed":
				num = GameSystem.Instance.TextController.AutoSpeed;
				break;
			case "2-AutoPageSpeed":
				num = GameSystem.Instance.TextController.AutoPageSpeed;
				break;
			case "3-WindowOpacity":
				num = (int)(GameSystem.Instance.MessageWindowOpacity * 100f);
				break;
			case "4-BGMVolume":
				num = (int)(GameSystem.Instance.AudioController.BGMVolume * 100f);
				break;
			case "5-SEVolume":
				num = (int)(GameSystem.Instance.AudioController.SoundVolume * 100f);
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
				string name = base.name;
				if (name != null)
				{
					if (!(name == "0-TextSpeed"))
					{
						if (!(name == "1-AutoSpeed"))
						{
							if (!(name == "2-AutoPageSpeed"))
							{
								if (!(name == "3-WindowOpacity"))
								{
									if (!(name == "4-BGMVolume"))
									{
										if (name == "5-SEVolume")
										{
											GameSystem.Instance.AudioController.SoundVolume = laststep;
											GameSystem.Instance.AudioController.SystemVolume = laststep;
											GameSystem.Instance.AudioController.RefreshLayerVolumes();
											BurikoMemory.Instance.SetGlobalFlag("GSEVolume", num);
										}
									}
									else
									{
										GameSystem.Instance.AudioController.BGMVolume = laststep;
										GameSystem.Instance.AudioController.RefreshLayerVolumes();
										BurikoMemory.Instance.SetGlobalFlag("GBGMVolume", num);
									}
								}
								else
								{
									GameSystem.Instance.MessageWindowOpacity = laststep;
									GameSystem.Instance.MainUIController.SetWindowOpacity(laststep);
									BurikoMemory.Instance.SetGlobalFlag("GWindowOpacity", num);
								}
							}
							else
							{
								GameSystem.Instance.TextController.AutoPageSpeed = num;
								BurikoMemory.Instance.SetGlobalFlag("GAutoAdvSpeed", num);
							}
						}
						else
						{
							GameSystem.Instance.TextController.AutoSpeed = num;
							BurikoMemory.Instance.SetGlobalFlag("GAutoSpeed", num);
						}
					}
					else
					{
						GameSystem.Instance.TextController.TextSpeed = num;
						BurikoMemory.Instance.SetGlobalFlag("GMessageSpeed", num);
					}
				}
			}
		}
	}
}
