using Assets.Scripts.Core;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MOD.Scripts.UI
{
	/// <summary>
	/// This class is responsible for showing toast noficiations
	/// </summary>
	class MODToaster
	{
		static MODToaster Instance;
		private MODStyleManager styleManager;
		string toastText;
		MODSimpleTimer toastNotificationTimer;

		public MODToaster(MODStyleManager styleManager)
		{
			this.styleManager = styleManager;
			this.toastText = "";
			this.toastNotificationTimer = new MODSimpleTimer();

			Instance = this;
		}

		public void Update()
		{
			toastNotificationTimer.Update();
		}

		public void OnGUIFragment()
		{
			if (!toastNotificationTimer.Finished())
			{
				// This scrolls the toast notification off the window when it's nearly finished
				float toastYPosition = Math.Min(50f, 200f * toastNotificationTimer.timeLeft - 50f);
				float toastWidth = 700f;
				float toastXPosition = (Screen.width - toastWidth) / 2.0f;
				GUILayout.BeginArea(new Rect(toastXPosition, toastYPosition, 700f, 200f));
				GUILayout.Box(toastText, toastText.Length > 30 ? styleManager.smallLabelStyle : styleManager.labelStyle);
				GUILayout.EndArea();
			}
		}

		/// <summary>
		/// Displays a toast notification. It will appear ontop of everything else on the screen.
		/// </summary>
		/// <param name="toastText">The text to display in the toast</param>
		/// <param name="toastDuration">The duration the toast will be shown for.
		/// The toast will slide off the screen for the last part of this duration.</param>
		public static void Show(string toastText, GUISound? maybeSound = GUISound.Click, float toastDuration = 3)
		{
			if(Instance == null)
			{
				return;
			}

			Instance.toastText = toastText;
			Instance.toastNotificationTimer.Start(toastDuration);
			if (maybeSound is GUISound sound)
			{
				GameSystem.Instance.AudioController.PlaySystemSound(MODSound.GetSoundPathFromEnum(sound));
			}
		}

		public static void Show(string toastText, bool isEnable, float toastDuration = 3)
		{
			Show(toastText, isEnable ? GUISound.Enable : GUISound.Disable, toastDuration);
		}

		public static void Show(string toastText, int numberedSound, float toastDuration = 3)
		{
			GUISound sound = GUISound.Click;
			switch (numberedSound)
			{
				case 0:
					sound = GUISound.Pluck0;
					break;
				case 1:
					sound = GUISound.Pluck1;
					break;
				case 2:
					sound = GUISound.Pluck2;
					break;
				case 3:
					sound = GUISound.Pluck3;
					break;
				case 4:
					sound = GUISound.Pluck4;
					break;
				case 5:
					sound = GUISound.Pluck5;
					break;
			}

			Show(toastText, sound, toastDuration);
		}

	}
}
