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

	public string English;

	public string Japanese;

	private void UpdateText()
	{
		if (textMesh != null)
		{
			textMesh.text = ((!language) ? Japanese : English);
		}
		else
		{
			label.text = ((!language) ? Japanese : English);
			label.UpdateNGUIText();
		}
	}

	private void Start()
	{
		isFullscreen = Screen.fullScreen;
		width = Screen.width;
		language = GameSystem.Instance.UseEnglishText;
		label = GetComponent<UILabel>();
		textMesh = GetComponent<TextMeshPro>();
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
