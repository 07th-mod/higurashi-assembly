using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static MOD.Scripts.UI.MODMenuCommon;

namespace MOD.Scripts.UI
{
	class MODTabControl
	{
		public class TabProperties
		{
			public string name;
			public string description;
			public Action tabRenderFunction;

			public TabProperties(string name, string description, Action tabRenderFunction)
			{
				this.name = name;
				this.description = description;
				this.tabRenderFunction = tabRenderFunction;
			}
		}

		private int currentTab;
		private List<TabProperties> tabProperties;
		private readonly MODRadio radioTabs;

		public MODTabControl(List<TabProperties> tabProperties)
		{
			this.tabProperties = tabProperties;
			this.radioTabs = new MODRadio("", tabProperties.Select(x => new GUIContent(x.name, x.description)).ToArray());
		}

		public void OnGUI()
		{
			if (radioTabs.OnGUIFragment(this.currentTab, hideLabel: true) is int tab)
			{
				this.currentTab = tab;
			}

			// Probably not possible, but restrict the current tab to be within limits
			currentTab = currentTab % tabProperties.Count;

			HeadingLabel(tabProperties[currentTab].name);

			// Render the current tab
			tabProperties[currentTab].tabRenderFunction();
		}
	}
}
