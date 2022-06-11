using Assets.Scripts.Core;
using Assets.Scripts.Core.Audio;
using Assets.Scripts.UI.Config;
using UnityEngine;

public class ConfigButton : MonoBehaviour
{
	public ConfigManager ConfigManager;

	private float cooldown;

	private void OnClick()
	{
		if (cooldown > 0f || UICamera.currentTouchID < -1)
		{
			return;
		}
		string name = base.name;
		if (!(name == "Return"))
		{
			if (name == "Switch")
			{
			}
		}
		else
		{
			GameSystem.Instance.LeaveConfigScreen(null);
		}
		AudioController.Instance.PlaySystemSound("wa_038.ogg", 1);
		cooldown = 1f;
	}

	private void OnHover(bool ishover)
	{
		_ = cooldown;
		_ = 0f;
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (cooldown > 0f)
		{
			cooldown -= Time.deltaTime;
		}
	}
}
