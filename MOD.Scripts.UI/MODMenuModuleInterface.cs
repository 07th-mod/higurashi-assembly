using System;
using System.Collections.Generic;
using System.Text;

namespace MOD.Scripts.UI
{
	interface MODMenuModuleInterface
	{
		/// <summary>
		/// Called every frame to draw the module
		/// </summary>
		void OnGUI();
		/// <summary>
		/// Called once just before the menu is shown
		/// Use for setup, or to do tasks that you don't want to execute every frame
		/// </summary>
		void OnBeforeMenuVisible();

		bool UserCanClose();

		string DefaultTooltip();
		string Heading();
	}
}
