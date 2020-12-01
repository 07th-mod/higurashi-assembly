using System;
using System.Collections.Generic;
using System.Text;

namespace MOD.Scripts.UI
{
	public enum GUISound
	{
		Click,
		LoudBang,
		Disable,
		Enable,
		Pluck0,
		Pluck1,
		Pluck2,
		Pluck3,
		Pluck4,
		Pluck5,
	}

	class MODSound
	{
		public static string GetSoundPathFromEnum(GUISound sound)
		{
			switch (sound)
			{
				case GUISound.Click:
					return "wa_038.ogg";
				case GUISound.LoudBang:
					return "wa_040.ogg";
				case GUISound.Disable:
					return "switchsound/disable.ogg";
				case GUISound.Enable:
					return "switchsound/enable.ogg";
				case GUISound.Pluck0:
					return "switchsound/0.ogg";
				case GUISound.Pluck1:
					return "switchsound/1.ogg";
				case GUISound.Pluck2:
					return "switchsound/2.ogg";
				case GUISound.Pluck3:
					return "switchsound/3.ogg";
				case GUISound.Pluck4:
					return "switchsound/4.ogg";
				case GUISound.Pluck5:
					return "switchsound/5.ogg";
			}

			return "wa_038.ogg";
		}
	}
}
