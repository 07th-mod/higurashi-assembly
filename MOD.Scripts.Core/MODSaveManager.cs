using MOD.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace MOD.Scripts.Core
{
	public static class MODSaveManager
	{
		static int RequiredAttempts = 10;

		public static bool UnmoddedLoadAllowed()
		{
			if(RequiredAttempts > 0)
			{
				RequiredAttempts--;
				MODToaster.Show("WARNING: Unmodded save not supported.\nClick repeatedly to try anyway at your own risk." + $"({RequiredAttempts})");
				return false;
			}
			else
			{
				MODToaster.Show("WARNING: Allowing load of unmodded save...");
				return true;
			}
		}
	}
}
