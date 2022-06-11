using Assets.Scripts.Core.Buriko;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI.CGGallery
{
	public class GalleryManager : MonoBehaviour
	{
		public struct GalleryEntry
		{
			public string[] ViewCG;

			public string[] DisplayCG;

			public GalleryEntry(string[] a, string[] b)
			{
				ViewCG = a;
				DisplayCG = b;
			}
		}

		public delegate void GalleryCloseCallback();

		public UITweener GalleryHeader;

		public UITweener GalleryPanel;

		public UITweener Background;

		public UITweener BlackZone;

		public UIPanel RootPanel;

		public UIGrid CGGrid;

		public GalleryButton ReturnButton;

		private Dictionary<int, string[]> galleryEntries = new Dictionary<int, string[]>
		{
			{
				0,
				new string[3]
				{
					"ev01",
					"ev01_02",
					"ev01_03"
				}
			},
			{
				1,
				new string[1]
				{
					"ev02"
				}
			},
			{
				2,
				new string[1]
				{
					"ev03"
				}
			},
			{
				3,
				new string[3]
				{
					"cut_004_10_10",
					"ev04a",
					"ev04b"
				}
			},
			{
				4,
				new string[2]
				{
					"ev05a",
					"ev05b"
				}
			},
			{
				5,
				new string[1]
				{
					"ev06"
				}
			},
			{
				6,
				new string[3]
				{
					"ev07a",
					"ev07b",
					"ev07c"
				}
			},
			{
				7,
				new string[1]
				{
					"ev08"
				}
			},
			{
				8,
				new string[3]
				{
					"ev09_01",
					"ev09_03",
					"ev09"
				}
			},
			{
				9,
				new string[1]
				{
					"evED01"
				}
			},
			{
				10,
				new string[1]
				{
					"evED02"
				}
			},
			{
				11,
				new string[1]
				{
					"evED03"
				}
			},
			{
				12,
				new string[1]
				{
					"evED04"
				}
			},
			{
				13,
				new string[1]
				{
					"evED05"
				}
			},
			{
				14,
				new string[1]
				{
					"evED01_03"
				}
			},
			{
				15,
				new string[4]
				{
					"ev10_01",
					"ev10_02",
					"ev10_03",
					"ev10_04"
				}
			}
		};

		private IEnumerator DoClose(GalleryCloseCallback callback)
		{
			yield return null;
			yield return null;
			GalleryHeader.PlayReverse();
			GalleryPanel.PlayReverse();
			Background.PlayReverse();
			yield return new WaitForSeconds(0.3f);
			callback?.Invoke();
			Object.Destroy(base.gameObject);
		}

		private IEnumerator DoHide()
		{
			yield return null;
			BlackZone.PlayForward();
			yield return new WaitForSeconds(0.3f);
		}

		private IEnumerator DoUnhide()
		{
			yield return null;
			BlackZone.PlayReverse();
			yield return new WaitForSeconds(0.3f);
		}

		public void Hide()
		{
			StartCoroutine(DoHide());
		}

		public void UnHide()
		{
			StartCoroutine(DoUnhide());
		}

		public void Close(GalleryCloseCallback callback)
		{
			StartCoroutine(DoClose(callback));
		}

		private IEnumerator DoOpen()
		{
			yield return null;
			yield return null;
			GalleryHeader.PlayForward();
			GalleryPanel.PlayForward();
			Background.PlayForward();
		}

		public void Open()
		{
			StartCoroutine(DoOpen());
			for (int i = 0; i < galleryEntries.Count; i++)
			{
				Transform child = CGGrid.transform.GetChild(i);
				if (BurikoMemory.Instance.SeenCG(galleryEntries[i][0]))
				{
					child.GetComponent<GalleryButton>().Prepare(this, i + 1);
					UISprite component = child.GetComponent<UISprite>();
					UIImageButton component2 = child.GetComponent<UIImageButton>();
					component2.normalSprite = "thum" + (i + 1).ToString("D3") + "-0";
					component2.hoverSprite = "thum" + (i + 1).ToString("D3") + "-1";
					component2.pressedSprite = "thum" + (i + 1).ToString("D3") + "-2";
					component.spriteName = "thum" + (i + 1).ToString("D3") + "-0";
				}
				else
				{
					child.gameObject.SetActive(value: false);
				}
			}
			ReturnButton.GetComponent<GalleryButton>().Prepare(this, 0);
		}

		private void Start()
		{
		}

		private void Update()
		{
		}
	}
}
