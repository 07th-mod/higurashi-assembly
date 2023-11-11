using Assets.Scripts.Core;
using TMPro;
using UnityEngine;

public class TextRefresher : MonoBehaviour
{
	private bool isFullscreen;

	private int width;

	private bool language;

	private UILabel label;

	private TextMeshPro textMesh;

	private TextMeshProUGUI uiTextMesh;

	public string English;

	public string Japanese;

	private void UpdateText()
	{
		if (uiTextMesh != null)
		{
			uiTextMesh.text = (language ? English : Japanese);
			return;
		}
		if (textMesh != null)
		{
			textMesh.text = (language ? English : Japanese);
			return;
		}
		label.text = (language ? English : Japanese);
		label.UpdateNGUIText();
	}

	private void Start()
	{
		isFullscreen = Screen.fullScreen;
		width = Screen.width;
		language = GameSystem.Instance.UseEnglishText;
		label = GetComponent<UILabel>();
		textMesh = GetComponent<TextMeshPro>();
		uiTextMesh = GetComponent<TextMeshProUGUI>();
		UpdateText();
	}

	private void Update()
	{
		if (Screen.fullScreen != isFullscreen)
		{
			isFullscreen = Screen.fullScreen;
			UpdateText();
		}
		if (width != Screen.width)
		{
			width = Screen.width;
			UpdateText();
		}
		if (language != GameSystem.Instance.UseEnglishText)
		{
			language = GameSystem.Instance.UseEnglishText;
			UpdateText();
		}
	}
}
