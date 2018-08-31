using Assets.Scripts.Core;
using Assets.Scripts.Core.Audio;
using System.Collections;
using UnityEngine;

public class MenuUIController : MonoBehaviour
{
	public delegate void MenuCloseDelegate();

	public UIPanel Panel;

	private float time = 0.5f;

	private bool isClosing;

	private IEnumerator LeaveMenuAnimation(MenuCloseDelegate onClose, bool showMessage)
	{
		isClosing = true;
		if (time > 0f)
		{
			yield return (object)new WaitForSeconds(time);
		}
		AudioController.Instance.PlaySystemSound("sysse05.ogg");
		yield return (object)null;
		yield return (object)null;
		LeanTween.value(this.Panel.gameObject, delegate(float f)
		{
			this.Panel.alpha = f;
		}, 1f, 0f, 0.3f);
		yield return (object)new WaitForSeconds(0.3f);
		if (showMessage)
		{
			GameSystem.Instance.MainUIController.FadeIn(0.3f);
			GameSystem.Instance.SceneController.RevealFace(0.3f);
			GameSystem.Instance.ExecuteActions();
			yield return (object)new WaitForSeconds(0.3f);
		}
		onClose?.Invoke();
		Object.Destroy(base.gameObject);
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
		AudioController.Instance.PlaySystemSound("sysse05.ogg");
	}

	private void Update()
	{
		time -= Time.deltaTime;
	}
}
