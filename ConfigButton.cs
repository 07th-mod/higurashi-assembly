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
		if (!(cooldown > 0f) && UICamera.currentTouchID >= -1)
		{
			switch (base.name)
			{
			case "Return":
				GameSystem.Instance.LeaveConfigScreen(null);
				break;
			}
			AudioController.Instance.PlaySystemSound("wa_038.ogg", 1);
			cooldown = 1f;
		}
	}

	private void OnHover(bool ishover)
	{
		if (!(cooldown > 0f))
		{
		}
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
