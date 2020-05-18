using Assets.Scripts.Core;
using Assets.Scripts.Core.AssetManagement;
using Assets.Scripts.Core.State;
using UnityEngine;

namespace Assets.Scripts.UI.Fragments
{
	internal class FragmentEntryButton : MonoBehaviour
	{
		public UISprite overlaySprite;

		private FragmentDataEntry fragment;

		private FragmentsManager manager;

		private UISprite sprite;

		private UIButton button;

		private bool isHover;

		public void Init(FragmentDataEntry f, FragmentsManager mg)
		{
			fragment = f;
			manager = mg;
			if (button == null)
			{
				button = GetComponent<UIButton>();
				sprite = GetComponent<UISprite>();
			}
			string name = $"k_{fragment.Id}na_normal";
			UISpriteData uISpriteData = sprite.atlas.GetSprite(name);
			if (f.ViewType == FragmentViewType.Broken)
			{
				overlaySprite.gameObject.SetActive(value: true);
				overlaySprite.spriteName = "fragment_fail";
			}
			if (f.ViewType == FragmentViewType.BrokenButFixable)
			{
				overlaySprite.gameObject.SetActive(value: true);
				overlaySprite.spriteName = "fragment_recoverable";
			}
			if (f.ViewType == FragmentViewType.Viewed)
			{
				overlaySprite.gameObject.SetActive(value: true);
				overlaySprite.spriteName = "fragment_complete";
			}
			if (f.ViewType == FragmentViewType.Unviewed)
			{
				overlaySprite.gameObject.SetActive(value: false);
			}
			if (uISpriteData != null && AssetManager.Instance.UseNewArt)
			{
				button.normalSprite = $"k_{fragment.Id}na_normal";
				button.hoverSprite = $"k_{fragment.Id}na_hover";
				button.pressedSprite = $"k_{fragment.Id}na_hover";
				button.disabledSprite = $"k_{fragment.Id}na_normal";
			}
			else
			{
				button.normalSprite = $"k_{fragment.Id}_normal";
				button.hoverSprite = $"k_{fragment.Id}_hover";
				button.pressedSprite = $"k_{fragment.Id}_hover";
				button.disabledSprite = $"k_{fragment.Id}_normal";
			}
		}

		private void OnClick()
		{
			if (UICamera.currentTouchID >= -1 && fragment != null && manager.isActive && GameSystem.Instance.GameState == GameState.FragmentScreen)
			{
				(GameSystem.Instance.GetStateObject() as StateFragmentList)?.OpenFragment(fragment);
			}
		}

		private void OnHover(bool hover)
		{
			isHover = hover;
			if (fragment != null)
			{
				if (!isHover)
				{
					manager.ClearTitle();
				}
				else if (GameSystem.Instance.GameState == GameState.FragmentScreen)
				{
					manager.ShowTitle((!GameSystem.Instance.UseEnglishText) ? fragment.TitleJp : fragment.Title);
					manager.ShowDescription((!GameSystem.Instance.UseEnglishText) ? fragment.DescriptionJp : fragment.Description);
				}
			}
		}

		public void Reset()
		{
			fragment = null;
			button = GetComponent<UIButton>();
			sprite = GetComponent<UISprite>();
			button.normalSprite = "tipslocked";
			button.hoverSprite = "tipslocked";
			button.pressedSprite = "tipslocked";
			button.disabledSprite = "tipslocked";
		}
	}
}
