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
			// Game places the main menu at y +2, which puts them a bit too high in their boxes.
			if (System.Math.Abs(textMesh.transform.localPosition.y - 2f) < 0.1)
			{
				Vector3 localPosition = textMesh.transform.localPosition;
				localPosition.y = 1f;
				textMesh.transform.localPosition = localPosition;
			}
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

	public void SetFontSize(float size)
	{
		if (textMesh == null) { return; }
		textMesh.fontSize = size;
	}

	// See MainUIController for detailed comments on this
	public void SetFontWeight(float weight)
	{
		textMesh.fontSharedMaterial.SetFloat(TMPro.ShaderUtilities.ID_WeightNormal, weight);
	}

	public void SetFontOutlineWidth(float outlineWidth)
	{
		// Unsure why, but setting the outline width via the property "textMesh.outlineWidth" causes
		// the config menu text to disappear entirely, no matter what setting you use, even though
		// when using that same property for the main text window works fine.
		//
		// Setting the value in the shader directly seems to work.
		textMesh.fontSharedMaterial.SetFloat(TMPro.ShaderUtilities.ID_OutlineWidth, outlineWidth);
	}
}
