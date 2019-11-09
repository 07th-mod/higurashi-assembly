using Assets.Scripts.Core;
using Assets.Scripts.Core.AssetManagement;
using Assets.Scripts.Core.Buriko;
using MOD.Scripts.Core;
using UnityEngine;

namespace Assets.Scripts.UI.Config
{
	public class SwitchButton : MonoBehaviour
	{
		public SwitchButtonObj Obj1;

		public SwitchButtonObj Obj2;

		public ConfigButtonType Type;

		public bool HideInactive;

		public bool IsVoiceSwitch;

		public string GlobalValue = string.Empty;

		private bool isReady;

		private void UpdateButtonValues()
		{
			bool flag = false;
			switch (Type)
			{
			case ConfigButtonType.SlowSkip:
				flag = GameSystem.Instance.SkipModeDelay;
				break;
			case ConfigButtonType.SkipUnread:
				flag = GameSystem.Instance.SkipUnreadMessages;
				break;
			case ConfigButtonType.ClickDuringAuto:
				flag = GameSystem.Instance.ClickDuringAuto;
				break;
			case ConfigButtonType.UsePrompts:
				flag = GameSystem.Instance.UsePrompts;
				break;
			case ConfigButtonType.RightClickMode:
				flag = GameSystem.Instance.RightClickMenu;
				break;
			case ConfigButtonType.FullscreenMode:
				flag = Screen.fullScreen;
				break;
			case ConfigButtonType.ClickToCutVoice:
				flag = GameSystem.Instance.StopVoiceOnClick;
				break;
			case ConfigButtonType.UseSystemSound:
				flag = GameSystem.Instance.UseSystemSounds;
				break;
			case ConfigButtonType.ArtStyle:
				flag = AssetManager.Instance.CurrentArtsetIndex > 0;
				break;
			case ConfigButtonType.Language:
				flag = GameSystem.Instance.UseEnglishText;
				break;
			case ConfigButtonType.AutoHideUI:
				flag = BurikoMemory.Instance.GetGlobalFlag("GHideButtons").BoolValue();
				break;
			}
			if (IsVoiceSwitch)
			{
				flag = BurikoMemory.Instance.GetGlobalFlag(GlobalValue).BoolValue();
			}
			if (flag)
			{
				if (HideInactive)
				{
					Obj1.gameObject.SetActive(value: false);
					Obj2.gameObject.SetActive(value: true);
				}
				else
				{
					Obj1.Button.isEnabled = false;
					Obj2.Button.isEnabled = true;
				}
			}
			else if (HideInactive)
			{
				Obj1.gameObject.SetActive(value: true);
				Obj2.gameObject.SetActive(value: false);
			}
			else
			{
				Obj1.Button.isEnabled = true;
				Obj2.Button.isEnabled = false;
			}
		}

		public void Click()
		{
			int val = 0;
			switch (Type)
			{
			case ConfigButtonType.SlowSkip:
				GameSystem.Instance.SkipModeDelay = !GameSystem.Instance.SkipModeDelay;
				if (GameSystem.Instance.SkipModeDelay)
				{
					val = 1;
				}
				break;
			case ConfigButtonType.SkipUnread:
				GameSystem.Instance.SkipUnreadMessages = !GameSystem.Instance.SkipUnreadMessages;
				if (GameSystem.Instance.SkipUnreadMessages)
				{
					val = 1;
				}
				break;
			case ConfigButtonType.ClickDuringAuto:
				GameSystem.Instance.ClickDuringAuto = !GameSystem.Instance.ClickDuringAuto;
				if (GameSystem.Instance.ClickDuringAuto)
				{
					val = 1;
				}
				break;
			case ConfigButtonType.UsePrompts:
				GameSystem.Instance.UsePrompts = !GameSystem.Instance.UsePrompts;
				if (GameSystem.Instance.UsePrompts)
				{
					val = 1;
				}
				break;
			case ConfigButtonType.RightClickMode:
				GameSystem.Instance.RightClickMenu = !GameSystem.Instance.RightClickMenu;
				if (GameSystem.Instance.RightClickMenu)
				{
					val = 1;
				}
				break;
			case ConfigButtonType.FullscreenMode:
				GameSystem.Instance.GoFullscreen();
				break;
			case ConfigButtonType.ClickToCutVoice:
				GameSystem.Instance.StopVoiceOnClick = !GameSystem.Instance.StopVoiceOnClick;
				if (GameSystem.Instance.StopVoiceOnClick)
				{
					val = 1;
				}
				break;
			case ConfigButtonType.UseSystemSound:
				GameSystem.Instance.UseSystemSounds = !GameSystem.Instance.UseSystemSounds;
				if (GameSystem.Instance.UseSystemSounds)
				{
					val = 1;
				}
				break;
			case ConfigButtonType.Language:
				GameSystem.Instance.UseEnglishText = !GameSystem.Instance.UseEnglishText;
				if (GameSystem.Instance.UseEnglishText)
				{
					val = 1;
				}
				GameSystem.Instance.TextController.SwapLanguages();
				break;
			case ConfigButtonType.ArtStyle:
				MODSystem.instance.modTextureController.ToggleArtStyle(allowMoreThan2: false);
				break;
			case ConfigButtonType.AutoHideUI:
			{
				bool flag = BurikoMemory.Instance.GetGlobalFlag("GHideButtons").BoolValue();
				BurikoMemory.Instance.SetGlobalFlag("GHideButtons", (!flag) ? 1 : 0);
				break;
			}
			}
			if (IsVoiceSwitch)
			{
				val = ((BurikoMemory.Instance.GetGlobalFlag(GlobalValue).IntValue() == 0) ? 1 : 0);
			}
			if (GlobalValue != string.Empty)
			{
				BurikoMemory.Instance.SetGlobalFlag(GlobalValue, val);
			}
			UpdateButtonValues();
		}

		private void Prepare()
		{
			// One of the buttons in the config menu has the wrong disabledColor and I don't feel like trying to modify it in the asset bundle
			if (Obj1.Button.disabledColor.r < 0.9)
			{
				Obj1.Button.disabledColor = Obj2.Button.disabledColor;
			}

			Obj1.RegisterSwitchController(this);
			Obj2.RegisterSwitchController(this);
			UpdateButtonValues();
			isReady = true;
		}

		private void Update()
		{
			if (!isReady)
			{
				Prepare();
			}
		}
	}
}
