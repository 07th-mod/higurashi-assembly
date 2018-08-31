using System;
using UnityEngine;

[AddComponentMenu("NGUI/UI/NGUI Unity2D Sprite")]
[ExecuteInEditMode]
public class UI2DSprite : UIBasicSprite
{
	[HideInInspector]
	[SerializeField]
	private Sprite mSprite;

	[HideInInspector]
	[SerializeField]
	private Material mMat;

	[HideInInspector]
	[SerializeField]
	private Shader mShader;

	[HideInInspector]
	[SerializeField]
	private Vector4 mBorder = Vector4.zero;

	[HideInInspector]
	[SerializeField]
	private bool mFixedAspect;

	[HideInInspector]
	[SerializeField]
	private float mPixelSize = 1f;

	public Sprite nextSprite;

	[NonSerialized]
	private int mPMA = -1;

	public Sprite sprite2D
	{
		get
		{
			return mSprite;
		}
		set
		{
			if (mSprite != value)
			{
				RemoveFromPanel();
				mSprite = value;
				nextSprite = null;
				CreatePanel();
			}
		}
	}

	public override Material material
	{
		get
		{
			return mMat;
		}
		set
		{
			if (mMat != value)
			{
				RemoveFromPanel();
				mMat = value;
				mPMA = -1;
				MarkAsChanged();
			}
		}
	}

	public override Shader shader
	{
		get
		{
			if (mMat != null)
			{
				return mMat.shader;
			}
			if (mShader == null)
			{
				mShader = Shader.Find("Unlit/Transparent Colored");
			}
			return mShader;
		}
		set
		{
			if (mShader != value)
			{
				RemoveFromPanel();
				mShader = value;
				if (mMat == null)
				{
					mPMA = -1;
					MarkAsChanged();
				}
			}
		}
	}

	public override Texture mainTexture
	{
		get
		{
			if (mSprite != null)
			{
				return mSprite.texture;
			}
			if (mMat != null)
			{
				return mMat.mainTexture;
			}
			return null;
		}
	}

	public override bool premultipliedAlpha
	{
		get
		{
			if (mPMA == -1)
			{
				Shader shader = this.shader;
				mPMA = ((shader != null && shader.name.Contains("Premultiplied")) ? 1 : 0);
			}
			return mPMA == 1;
		}
	}

	public override float pixelSize => mPixelSize;

	public override Vector4 drawingDimensions
	{
		get
		{
			Vector2 pivotOffset = base.pivotOffset;
			float num = (0f - pivotOffset.x) * (float)mWidth;
			float num2 = (0f - pivotOffset.y) * (float)mHeight;
			float num3 = num + (float)mWidth;
			float num4 = num2 + (float)mHeight;
			if (mSprite != null && mType != Type.Tiled)
			{
				int num5 = Mathf.RoundToInt(mSprite.rect.width);
				int num6 = Mathf.RoundToInt(mSprite.rect.height);
				Vector2 textureRectOffset = mSprite.textureRectOffset;
				int num7 = Mathf.RoundToInt(textureRectOffset.x);
				Vector2 textureRectOffset2 = mSprite.textureRectOffset;
				int num8 = Mathf.RoundToInt(textureRectOffset2.y);
				float num9 = mSprite.rect.width - mSprite.textureRect.width;
				Vector2 textureRectOffset3 = mSprite.textureRectOffset;
				int num10 = Mathf.RoundToInt(num9 - textureRectOffset3.x);
				float num11 = mSprite.rect.height - mSprite.textureRect.height;
				Vector2 textureRectOffset4 = mSprite.textureRectOffset;
				int num12 = Mathf.RoundToInt(num11 - textureRectOffset4.y);
				float num13 = 1f;
				float num14 = 1f;
				if (num5 > 0 && num6 > 0 && (mType == Type.Simple || mType == Type.Filled))
				{
					if ((num5 & 1) != 0)
					{
						num10++;
					}
					if ((num6 & 1) != 0)
					{
						num12++;
					}
					num13 = 1f / (float)num5 * (float)mWidth;
					num14 = 1f / (float)num6 * (float)mHeight;
				}
				if (mFlip == Flip.Horizontally || mFlip == Flip.Both)
				{
					num += (float)num10 * num13;
					num3 -= (float)num7 * num13;
				}
				else
				{
					num += (float)num7 * num13;
					num3 -= (float)num10 * num13;
				}
				if (mFlip == Flip.Vertically || mFlip == Flip.Both)
				{
					num2 += (float)num12 * num14;
					num4 -= (float)num8 * num14;
				}
				else
				{
					num2 += (float)num8 * num14;
					num4 -= (float)num12 * num14;
				}
			}
			float num15;
			float num16;
			if (mFixedAspect)
			{
				num15 = 0f;
				num16 = 0f;
			}
			else
			{
				Vector4 vector = border * pixelSize;
				num15 = vector.x + vector.z;
				num16 = vector.y + vector.w;
			}
			float x = Mathf.Lerp(num, num3 - num15, mDrawRegion.x);
			float y = Mathf.Lerp(num2, num4 - num16, mDrawRegion.y);
			float z = Mathf.Lerp(num + num15, num3, mDrawRegion.z);
			float w = Mathf.Lerp(num2 + num16, num4, mDrawRegion.w);
			return new Vector4(x, y, z, w);
		}
	}

	public override Vector4 border
	{
		get
		{
			return mBorder;
		}
		set
		{
			if (mBorder != value)
			{
				mBorder = value;
				MarkAsChanged();
			}
		}
	}

	protected override void OnUpdate()
	{
		if (nextSprite != null)
		{
			if (nextSprite != mSprite)
			{
				sprite2D = nextSprite;
			}
			nextSprite = null;
		}
		base.OnUpdate();
		if (mFixedAspect)
		{
			Texture mainTexture = this.mainTexture;
			if (mainTexture != null)
			{
				int num = Mathf.RoundToInt(mSprite.rect.width);
				int num2 = Mathf.RoundToInt(mSprite.rect.height);
				Vector2 textureRectOffset = mSprite.textureRectOffset;
				int num3 = Mathf.RoundToInt(textureRectOffset.x);
				Vector2 textureRectOffset2 = mSprite.textureRectOffset;
				int num4 = Mathf.RoundToInt(textureRectOffset2.y);
				float num5 = mSprite.rect.width - mSprite.textureRect.width;
				Vector2 textureRectOffset3 = mSprite.textureRectOffset;
				int num6 = Mathf.RoundToInt(num5 - textureRectOffset3.x);
				float num7 = mSprite.rect.height - mSprite.textureRect.height;
				Vector2 textureRectOffset4 = mSprite.textureRectOffset;
				int num8 = Mathf.RoundToInt(num7 - textureRectOffset4.y);
				num += num3 + num6;
				num2 += num8 + num4;
				float num9 = (float)mWidth;
				float num10 = (float)mHeight;
				float num11 = num9 / num10;
				float num12 = (float)num / (float)num2;
				if (num12 < num11)
				{
					float num13 = (num9 - num10 * num12) / num9 * 0.5f;
					base.drawRegion = new Vector4(num13, 0f, 1f - num13, 1f);
				}
				else
				{
					float num14 = (num10 - num9 / num12) / num10 * 0.5f;
					base.drawRegion = new Vector4(0f, num14, 1f, 1f - num14);
				}
			}
		}
	}

	public override void MakePixelPerfect()
	{
		base.MakePixelPerfect();
		if (mType != Type.Tiled)
		{
			Texture mainTexture = this.mainTexture;
			if (!(mainTexture == null) && (mType == Type.Simple || mType == Type.Filled || !base.hasBorder) && mainTexture != null)
			{
				Rect rect = mSprite.rect;
				int num = Mathf.RoundToInt(rect.width);
				int num2 = Mathf.RoundToInt(rect.height);
				if ((num & 1) == 1)
				{
					num++;
				}
				if ((num2 & 1) == 1)
				{
					num2++;
				}
				base.width = num;
				base.height = num2;
			}
		}
	}

	public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		Texture mainTexture = this.mainTexture;
		if (!(mainTexture == null))
		{
			Rect rect = (!(mSprite != null)) ? new Rect(0f, 0f, (float)mainTexture.width, (float)mainTexture.height) : mSprite.textureRect;
			Rect inner = rect;
			Vector4 border = this.border;
			inner.xMin += border.x;
			inner.yMin += border.y;
			inner.xMax -= border.z;
			inner.yMax -= border.w;
			float num = 1f / (float)mainTexture.width;
			float num2 = 1f / (float)mainTexture.height;
			rect.xMin *= num;
			rect.xMax *= num;
			rect.yMin *= num2;
			rect.yMax *= num2;
			inner.xMin *= num;
			inner.xMax *= num;
			inner.yMin *= num2;
			inner.yMax *= num2;
			int size = verts.size;
			Fill(verts, uvs, cols, rect, inner);
			if (onPostFill != null)
			{
				onPostFill(this, size, verts, uvs, cols);
			}
		}
	}
}
