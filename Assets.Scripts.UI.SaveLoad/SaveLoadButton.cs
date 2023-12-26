using Assets.Scripts.Core;
using Assets.Scripts.Core.AssetManagement;
using Assets.Scripts.Core.Audio;
using Assets.Scripts.Core.Buriko;
using Assets.Scripts.Core.State;
using Assets.Scripts.UI.Prompt;
using System.Collections.Generic;
using MOD.Scripts.Core.Localization;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.UI.SaveLoad
{
	public class SaveLoadButton : MonoBehaviour
	{
		private int slot;

		private float time = 0.45f;

		private bool isEnabled = true;

		private GameSystem gameSystem;

		private SaveLoadManager manager;

		private void OnClick()
		{
			if (gameSystem == null)
			{
				gameSystem = GameSystem.Instance;
			}
			if (gameSystem.GameState != GameState.SaveLoadScreen || time > 0f || UICamera.currentTouchID < -1 || !isEnabled)
			{
				return;
			}
			StateSaveLoad state = gameSystem.GetStateObject() as StateSaveLoad;
			if (state == null)
			{
				return;
			}
			string name = base.name;
			switch (name)
			{
				case "Save":
				case "0-Save":
					manager.Save(slot);
					goto IL_02e0;
				case "Load":
				case "1-Load":
					goto IL_0158;
				case "2-Edit":
					goto IL_01cb;
				case "3-Delete":
					goto IL_025a;
				case "Return":
					goto IL_02c0;
				default:
					goto IL_02d1;
			}
			IL_0158:
			SaveEntry d5 = BurikoScriptSystem.Instance.GetSaveInfo(slot);
			if (d5 == null)
			{
				return;
			}
			StateDialogPrompt state3 = new StateDialogPrompt(PromptType.DialogLoad, delegate
			{
				state.Leave(delegate
				{
					StateTitle stateTitle = gameSystem.GetStateObject() as StateTitle;
					if (!manager.CanSave())
					{
						stateTitle?.RequestLeaveImmediate();
					}
					(gameSystem.GetStateObject() as StateChapterScreen)?.RequestLeaveImmediate();
					GameSystem.Instance.ScriptSystem.LoadGame(slot);
				});
			}, null);
			gameSystem.PushStateObject(state3);
			gameSystem.RegisterAction(delegate
			{
				PromptController promptController3 = state3.GetPromptController();
				if (!(promptController3 == null))
				{
					string fileNameWithoutExtension3 = Path.GetFileNameWithoutExtension(d5.Path);
					Texture2D image3 = AssetManager.Instance.LoadScreenshot(fileNameWithoutExtension3 + ".png");
					promptController3.SetScreenshotDetails(image3, d5.Time.ToString(Loc.dateTimeFormat, Loc.cultureInfo), d5.Text, d5.TextJp);
				}
			});
			gameSystem.ExecuteActions();
			goto IL_02e0;
			IL_025a:
			SaveEntry d4 = BurikoScriptSystem.Instance.GetSaveInfo(slot);
			if (d4 == null)
			{
				return;
			}
			StateDialogPrompt prompt = new StateDialogPrompt(PromptType.DialogDelete, delegate
			{
				BurikoScriptSystem.Instance.DeleteSave(slot);
				manager.RefreshList();
			}, null);
			GameSystem.Instance.PushStateObject(prompt);
			GameSystem.Instance.RegisterAction(delegate
			{
				PromptController promptController2 = prompt.GetPromptController();
				if (!(promptController2 == null))
				{
					string fileNameWithoutExtension2 = Path.GetFileNameWithoutExtension(d4.Path);
					Texture2D image2 = AssetManager.Instance.LoadScreenshot(fileNameWithoutExtension2 + ".png");
					promptController2.SetScreenshotDetails(image2, d4.Time.ToString(Loc.dateTimeFormat, Loc.cultureInfo), d4.Text, d4.TextJp);
				}
			});
			goto IL_02e0;
			IL_02e0:
			AudioController.Instance.PlaySystemSound("wa_038.ogg", 1);
			time = 0.45f;
			return;
			IL_01cb:
			SaveEntry d3 = BurikoScriptSystem.Instance.GetSaveInfo(slot);
			if (d3 == null)
			{
				return;
			}
			SaveLoadManager.EditSlot = slot;
			StateDialogPrompt state2 = new StateDialogPrompt(PromptType.DialogEdit, delegate
			{
			}, null);
			gameSystem.PushStateObject(state2);
			gameSystem.RegisterAction(delegate
			{
				PromptController promptController = state2.GetPromptController();
				if (!(promptController == null))
				{
					string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(d3.Path);
					Texture2D image = AssetManager.Instance.LoadScreenshot(fileNameWithoutExtension + ".png");
					promptController.SetScreenshotDetails(image, d3.Time.ToString(Loc.dateTimeFormat, Loc.cultureInfo), d3.Text, d3.TextJp);
				}
			});
			gameSystem.ExecuteActions();
			goto IL_02e0;
			IL_02d1:
			Debug.LogWarning("Unhandled button action!");
			goto IL_02e0;
			IL_02c0:
			state.Leave(null);
			goto IL_02e0;
		}

		private void OnHover(bool hover)
		{
			if (gameSystem == null)
			{
				gameSystem = GameSystem.Instance;
			}
			if (gameSystem.GameState == GameState.SaveLoadScreen)
			{
			}
		}

		public void Prepare(int slotnum, SaveLoadManager mg)
		{
			manager = mg;
			gameSystem = GameSystem.Instance;
			slot = slotnum;
		}

		private void Update()
		{
			if (time > 0f)
			{
				time -= Time.deltaTime;
			}
		}
	}
}
