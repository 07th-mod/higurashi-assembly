using Assets.Scripts.Core;
using Assets.Scripts.Core.Buriko;
using Assets.Scripts.Core.State;
using Assets.Scripts.UI.Prompt;
using MOD.Scripts.Core.Localization;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.UI.SaveLoad
{
	public class SaveLoadManager : MonoBehaviour
	{
		public delegate void SaveLoadLeaveCallback();

		public static int EditSlot;

		public UIGrid SaveGrid;

		public UIGrid PageGrid;

		public SaveLoadQSave[] QuickSaveSlots;

		public UIPanel Panel;

		private SaveLoadEntry[] saveEntries;

		private SaveLoadPage[] savePages;

		private int curPage;

		private bool restoreUI;

		private IEnumerator LeaveSaveLoad(SaveLoadManager.SaveLoadLeaveCallback callback)
		{
			LeanTween.value(this.Panel.gameObject, delegate(float f)
			{
				this.Panel.alpha = f;
			}, 1f, 0f, 0.5f);
			if (this.restoreUI)
			{
				GameSystem.Instance.MainUIController.FadeIn(0.3f);
				GameSystem.Instance.SceneController.RevealFace(0.3f);
				GameSystem.Instance.ExecuteActions();
			}
			yield return new WaitForSeconds(0.5f);
			if (callback != null)
			{
				callback();
			}
			UnityEngine.Object.Destroy(base.gameObject);
			UnityEngine.Object.Destroy(this);
			yield break;
		}

		public void Leave(SaveLoadLeaveCallback callback)
		{
			StartCoroutine(LeaveSaveLoad(callback));
		}

		private IEnumerator SaveCoroutine()
		{
			yield return null;
			yield return null;
			this.ChangePage(this.curPage);
			yield break;
		}

		private void DoSave(int slot)
		{
			Debug.Log("DoSave: " + slot);
			BurikoScriptSystem.Instance.SaveGame(slot);
			StartCoroutine(SaveCoroutine());
		}

		public void Save(int slot)
		{
			if (BurikoMemory.Instance.GetGlobalFlag("GUsePrompts").BoolValue())
			{
				StateDialogPrompt prompt = new StateDialogPrompt(PromptType.DialogSave, delegate
				{
					DoSave(slot);
				}, null);
				GameSystem.Instance.PushStateObject(prompt);
				GameSystem.Instance.RegisterAction(delegate
				{
					PromptController p = prompt.GetPromptController();
					if (p == null)
					{
						Debug.LogWarning("Prompt does not exist!");
					}
					else
					{
						GameSystem.Instance.SceneController.GetScreenshot(delegate(Texture2D tex)
						{
							DateTime now = DateTime.Now;
							string fullText = GameSystem.Instance.TextController.GetFullText(1);
							string fullText2 = GameSystem.Instance.TextController.GetFullText(0);
							p.SetScreenshotDetails(tex, now.ToString(Loc.dateTimeFormat, Loc.cultureInfo), fullText, fullText2);
						});
					}
				});
				GameSystem.Instance.ExecuteActions();
			}
			else
			{
				DoSave(slot);
			}
		}

		public void RefreshList()
		{
			int page = curPage;
			ChangePage(-1);
			ChangePage(page);
		}

		public void ChangePage(int page)
		{
			curPage = page;
			for (int i = 0; i < SaveGrid.transform.childCount; i++)
			{
				saveEntries[i].LoadSlot(page * 10 + i);
			}
			for (int j = 0; j < PageGrid.transform.childCount; j++)
			{
				if (j == page)
				{
					savePages[j].Disable();
				}
				else
				{
					savePages[j].Enable();
				}
			}
		}

		public bool CanSave()
		{
			return GameSystem.Instance.CanSave;
		}

		public bool CanLoad()
		{
			return GameSystem.Instance.CanLoad;
		}

		private IEnumerator DoOpen()
		{
			this.Panel.alpha = 0f;
			yield return null;
			yield return null;
			LeanTween.value(this.Panel.gameObject, delegate(float f)
			{
				this.Panel.alpha = f;
			}, 0f, 1f, 0.5f);
			yield break;
		}

		public void Open(bool doRestoreUI)
		{
			restoreUI = doRestoreUI;
			saveEntries = new SaveLoadEntry[10];
			for (int i = 0; i < SaveGrid.transform.childCount; i++)
			{
				saveEntries[i] = SaveGrid.transform.GetChild(i).GetComponent<SaveLoadEntry>();
				saveEntries[i].Prepare(this);
				saveEntries[i].LoadSlot(i);
			}
			for (int j = 0; j < QuickSaveSlots.Length; j++)
			{
				QuickSaveSlots[j].Prepare(this);
				QuickSaveSlots[j].LoadSlot(BurikoScriptSystem.Instance.GetQSaveSlotByMostRecent(j));
			}
			savePages = new SaveLoadPage[10];
			for (int k = 0; k < PageGrid.transform.childCount; k++)
			{
				savePages[k] = PageGrid.transform.GetChild(k).GetComponent<SaveLoadPage>();
				savePages[k].Setup(k + 1, this);
			}
			StartCoroutine(DoOpen());
		}

		private void Update()
		{
		}
	}
}
