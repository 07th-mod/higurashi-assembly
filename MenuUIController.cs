using Assets.Scripts.Core;
using System.Collections;
using UnityEngine;

public class MenuUIController : MonoBehaviour
{
	public delegate void MenuCloseDelegate();

	public UIPanel Panel;

	private float time = 0.5f;

	private bool isClosing;
	public float PanelAlpha() => Panel.alpha;

	private IEnumerator LeaveMenuAnimation(MenuUIController.MenuCloseDelegate onClose, bool showMessage)
	{
		this.isClosing = true;
		if (this.time > 0f)
		{
			yield return new WaitForSeconds(this.time);
		}
		yield return null;
		yield return null;
		LeanTween.value(this.Panel.gameObject, delegate(float f)
		{
			this.Panel.alpha = f;
		}, 1f, 0f, 0.3f);
		yield return new WaitForSeconds(0.3f);
		if (showMessage)
		{
			GameSystem.Instance.MainUIController.FadeIn(0.3f);
			GameSystem.Instance.SceneController.RevealFace(0.3f);
			GameSystem.Instance.ExecuteActions();
			yield return new WaitForSeconds(0.3f);
		}
		if (onClose != null)
		{
			onClose();
		}
		UnityEngine.Object.Destroy(base.gameObject);
		yield break;
	}

	public void LeaveMenu(MenuCloseDelegate onClose, bool showMessage)
	{
		if (!isClosing && !(time > 0f))
		{
			StartCoroutine(LeaveMenuAnimation(onClose, showMessage));
		}
	}

	private void Start()
	{
		Panel.alpha = 0f;
		LeanTween.value(Panel.gameObject, delegate(float f)
		{
			Panel.alpha = f;
		}, 0f, 1f, 0.3f).setDelay(0.3f);
	}

	private void Update()
	{
		time -= Time.deltaTime;
	}
}
