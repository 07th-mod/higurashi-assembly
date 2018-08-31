using UnityEngine;

namespace TMPro
{
	public static class TMPro_EventManager
	{
		public delegate void COMPUTE_DT_EVENT_HANDLER(object Sender, Compute_DT_EventArgs e);

		public delegate void MaterialProperty_Event_Handler(bool isChanged, Material mat);

		public delegate void FontProperty_Event_Handler(bool isChanged, TextMeshProFont font);

		public delegate void SpriteAssetProperty_Event_Handler(bool isChanged, Object obj);

		public delegate void TextMeshProProperty_Event_Handler(bool isChanged, TextMeshPro obj);

		public delegate void DragAndDrop_Event_Handler(GameObject sender, Material currentMaterial, Material newMaterial);

		public delegate void OnPreRenderObject_Event_Handler();

		public static event COMPUTE_DT_EVENT_HANDLER COMPUTE_DT_EVENT;

		public static event MaterialProperty_Event_Handler MATERIAL_PROPERTY_EVENT;

		public static event FontProperty_Event_Handler FONT_PROPERTY_EVENT;

		public static event SpriteAssetProperty_Event_Handler SPRITE_ASSET_PROPERTY_EVENT;

		public static event TextMeshProProperty_Event_Handler TEXTMESHPRO_PROPERTY_EVENT;

		public static event DragAndDrop_Event_Handler DRAG_AND_DROP_MATERIAL_EVENT;

		public static event OnPreRenderObject_Event_Handler OnPreRenderObject_Event;

		public static void ON_PRE_RENDER_OBJECT_CHANGED()
		{
			if (TMPro_EventManager.OnPreRenderObject_Event != null)
			{
				TMPro_EventManager.OnPreRenderObject_Event();
			}
		}

		public static void ON_MATERIAL_PROPERTY_CHANGED(bool isChanged, Material mat)
		{
			if (TMPro_EventManager.MATERIAL_PROPERTY_EVENT != null)
			{
				TMPro_EventManager.MATERIAL_PROPERTY_EVENT(isChanged, mat);
			}
		}

		public static void ON_FONT_PROPERTY_CHANGED(bool isChanged, TextMeshProFont font)
		{
			if (TMPro_EventManager.FONT_PROPERTY_EVENT != null)
			{
				TMPro_EventManager.FONT_PROPERTY_EVENT(isChanged, font);
			}
		}

		public static void ON_SPRITE_ASSET_PROPERTY_CHANGED(bool isChanged, Object obj)
		{
			if (TMPro_EventManager.SPRITE_ASSET_PROPERTY_EVENT != null)
			{
				TMPro_EventManager.SPRITE_ASSET_PROPERTY_EVENT(isChanged, obj);
			}
		}

		public static void ON_TEXTMESHPRO_PROPERTY_CHANGED(bool isChanged, TextMeshPro obj)
		{
			if (TMPro_EventManager.TEXTMESHPRO_PROPERTY_EVENT != null)
			{
				TMPro_EventManager.TEXTMESHPRO_PROPERTY_EVENT(isChanged, obj);
			}
		}

		public static void ON_DRAG_AND_DROP_MATERIAL_CHANGED(GameObject sender, Material currentMaterial, Material newMaterial)
		{
			if (TMPro_EventManager.DRAG_AND_DROP_MATERIAL_EVENT != null)
			{
				TMPro_EventManager.DRAG_AND_DROP_MATERIAL_EVENT(sender, currentMaterial, newMaterial);
			}
		}

		public static void ON_COMPUTE_DT_EVENT(object Sender, Compute_DT_EventArgs e)
		{
			if (TMPro_EventManager.COMPUTE_DT_EVENT != null)
			{
				TMPro_EventManager.COMPUTE_DT_EVENT(Sender, e);
			}
		}
	}
}
