using Assets.Scripts.Core.Audio;
using UnityEngine;

namespace Assets.Scripts.UI.Config
{
	public class SwitchButtonObj : MonoBehaviour
	{
		public UISprite Sprite;

		public UIButton Button;

		private float cooldown;

		private SwitchButton controller;

		private void OnClick()
		{
			if (!(cooldown > 0f) && UICamera.currentTouchID >= -1)
			{
				AudioController.Instance.PlaySystemSound("wa_038.ogg", 1);
				controller.Click();
				cooldown = 0.2f;
			}
		}

		private void OnHover(bool ishover)
		{
			_ = cooldown;
			_ = 0f;
		}

		public void RegisterSwitchController(SwitchButton c)
		{
			controller = c;
		}

		public void UpdateValue(bool isOn)
		{
			if (Sprite == null)
			{
				Start();
			}
		}

		private void Start()
		{
			Sprite = GetComponent<UISprite>();
			Button = GetComponent<UIButton>();
		}

		private void Update()
		{
			if (cooldown > 0f)
			{
				cooldown -= Time.deltaTime;
			}
		}
	}
}
