using Assets.Scripts.Core;
using Assets.Scripts.Core.State;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI.Prompt
{
	public class PromptController : MonoBehaviour
	{
		public TextMeshPro TopBox;

		public TextMeshPro BottomBox;

		public UIPanel DialogPanel;

		public UITexture DialogPromptImage;

		public UITexture DialogScreenshot;

		public PromptButton ButtonYes;

		public PromptButton ButtonNo;

		public Material TextMaterial;

		public Texture2D TextureEdit;

		public Texture2D TextureExit;

		public Texture2D TextureLoad;

		public Texture2D TextureSave;

		public Texture2D TextureTitle;

		public Texture2D TextureDelete;

		public Texture2D TextureLanguage;

		public Texture2D TextureEditJapanese;

		public Texture2D TextureExitJapanese;

		public Texture2D TextureLoadJapanese;

		public Texture2D TextureSaveJapanese;

		public Texture2D TextureTitleJapanese;

		public Texture2D TextureDeleteJapanese;

		private DialogCallback yesCallback;

		private DialogCallback noCallback;

		private PromptType pType;

		private IEnumerator LeaveWindow(bool affirmative)
		{
			this.ButtonYes.enabled = false;
			this.ButtonNo.enabled = false;
			TweenAlpha a = this.DialogPanel.GetComponent<TweenAlpha>();
			LeanTween.cancel(base.gameObject);
			LeanTween.value(base.gameObject, delegate(float f)
			{
				this.TextMaterial.SetColor("_FaceColor", new Color(0f, 0f, 0f, f));
			}, 1f, 0f, 0.3f);
			a.PlayReverse();
			if (!affirmative && this.noCallback != null)
			{
				this.noCallback();
			}
			yield return new WaitForSeconds(0.5f);
			GameSystem.Instance.PopStateStack();
			if (affirmative && this.yesCallback != null)
			{
				this.yesCallback();
			}
			UnityEngine.Object.Destroy(base.gameObject);
			yield break;
		}

		public void Hide(bool affirmative)
		{
			StartCoroutine(LeaveWindow(affirmative));
			(GameSystem.Instance.GetStateObject() as StateDialogPrompt)?.DisableInputActions();
		}

		public void SetScreenshotDetails(Texture2D image, string top, string bottom, string bottomjp)
		{
			Debug.Log("SetScreenshotDetails " + top);
			DialogScreenshot.gameObject.SetActive(value: true);
			DialogScreenshot.mainTexture = image;
			TopBox.gameObject.SetActive(value: true);
			BottomBox.gameObject.SetActive(value: true);
			TopBox.text = top;
			if (GameSystem.Instance.UseEnglishText)
			{
				BottomBox.text = bottom.Replace("\n", " ").TrimStart(' ', '\n');
			}
			else
			{
				BottomBox.text = bottomjp.Replace("\n", " ").TrimStart(' ', '\n');
			}
		}

		public void Open(PromptType type, DialogCallback onYes, DialogCallback onNo)
		{
			pType = type;
			if (!GameSystem.Instance.UseEnglishText)
			{
				switch (type)
				{
				case PromptType.DialogEdit:
					DialogPromptImage.mainTexture = TextureEditJapanese;
					break;
				case PromptType.DialogExit:
					DialogPromptImage.mainTexture = TextureExitJapanese;
					break;
				case PromptType.DialogLoad:
					DialogPromptImage.mainTexture = TextureLoadJapanese;
					break;
				case PromptType.DialogSave:
					DialogPromptImage.mainTexture = TextureSaveJapanese;
					break;
				case PromptType.DialogTitle:
					DialogPromptImage.mainTexture = TextureTitleJapanese;
					break;
				case PromptType.DialogDelete:
					DialogPromptImage.mainTexture = TextureDeleteJapanese;
					break;
				case PromptType.DialogLanguage:
					DialogPromptImage.mainTexture = TextureLanguage;
					break;
				}
			}
			else
			{
				switch (type)
				{
				case PromptType.DialogEdit:
					DialogPromptImage.mainTexture = TextureEdit;
					break;
				case PromptType.DialogExit:
					DialogPromptImage.mainTexture = TextureExit;
					break;
				case PromptType.DialogLoad:
					DialogPromptImage.mainTexture = TextureLoad;
					break;
				case PromptType.DialogSave:
					DialogPromptImage.mainTexture = TextureSave;
					break;
				case PromptType.DialogTitle:
					DialogPromptImage.mainTexture = TextureTitle;
					break;
				case PromptType.DialogDelete:
					DialogPromptImage.mainTexture = TextureDelete;
					break;
				case PromptType.DialogLanguage:
					DialogPromptImage.mainTexture = TextureLanguage;
					break;
				}
			}
			DialogPromptImage.MakePixelPerfect();
			if (type == PromptType.DialogExit || type == PromptType.DialogTitle || type == PromptType.DialogDelete)
			{
				DialogScreenshot.gameObject.SetActive(value: false);
				ButtonYes.transform.localPosition = new Vector3(-56f, -16f, 0f);
				ButtonNo.transform.localPosition = new Vector3(56f, -16f, 0f);
			}
			if (type != 0 && type != PromptType.DialogSave)
			{
				Object.Destroy(BottomBox.GetComponent<UIInput>());
			}
			TopBox.text = string.Empty;
			BottomBox.text = string.Empty;
			yesCallback = onYes;
			noCallback = onNo;
			if (type == PromptType.DialogLanguage)
			{
				DialogScreenshot.gameObject.SetActive(value: false);
				ButtonYes.transform.localPosition = new Vector3(-56f, -66f, 0f);
				ButtonNo.transform.localPosition = new Vector3(56f, -66f, 0f);
				ButtonYes.ChangeButtonImages("btn_lang_normal", "btn_lang_hover", "btn_lang_down");
				ButtonNo.ChangeButtonImages("btn_lang_normal_jp", "btn_lang_hover_jp", "btn_lang_down_jp");
			}
			ButtonYes.RegisterController(this);
			ButtonNo.RegisterController(this);
		}

		private void Start()
		{
			LeanTween.cancel(base.gameObject);
			LeanTween.value(base.gameObject, delegate(float f)
			{
				TextMaterial.SetColor("_FaceColor", new Color(0f, 0f, 0f, f));
			}, 0f, 1f, 0.2f);
		}
	}
}
