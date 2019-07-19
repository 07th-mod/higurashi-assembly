using System;
using UnityEngine;

[AddComponentMenu("NGUI/Tween/Tween Alpha")]
public class TweenAlpha : UITweener
{
	[Range(0f, 1f)]
	public float from = 1f;

	[Range(0f, 1f)]
	public float to = 1f;

	private bool mCached;

	private UIRect mRect;

	private Material mMat;

	private SpriteRenderer mSr;

	[Obsolete("Use 'value' instead")]
	public float alpha
	{
		get
		{
			return value;
		}
		set
		{
			this.value = value;
		}
	}

	public float value
	{
		get
		{
			if (!mCached)
			{
				Cache();
			}
			if (mRect != null)
			{
				return mRect.alpha;
			}
			if (mSr != null)
			{
				Color color = mSr.color;
				return color.a;
			}
			float result;
			if (mMat != null)
			{
				Color color2 = mMat.color;
				result = color2.a;
			}
			else
			{
				result = 1f;
			}
			return result;
		}
		set
		{
			if (!mCached)
			{
				Cache();
			}
			if (mRect != null)
			{
				mRect.alpha = value;
			}
			else if (mSr != null)
			{
				Color color = mSr.color;
				color.a = value;
				mSr.color = color;
			}
			else if (mMat != null)
			{
				Color color2 = mMat.color;
				color2.a = value;
				mMat.color = color2;
			}
		}
	}

	private void Cache()
	{
		mCached = true;
		mRect = GetComponent<UIRect>();
		mSr = GetComponent<SpriteRenderer>();
		if (mRect == null && mSr == null)
		{
			Renderer component = GetComponent<Renderer>();
			if (component != null)
			{
				mMat = component.material;
			}
			if (mMat == null)
			{
				mRect = GetComponentInChildren<UIRect>();
			}
		}
	}

	protected override void OnUpdate(float factor, bool isFinished)
	{
		value = Mathf.Lerp(from, to, factor);
	}

	public static TweenAlpha Begin(GameObject go, float duration, float alpha)
	{
		TweenAlpha tweenAlpha = UITweener.Begin<TweenAlpha>(go, duration);
		tweenAlpha.from = tweenAlpha.value;
		tweenAlpha.to = alpha;
		if (duration <= 0f)
		{
			tweenAlpha.Sample(1f, isFinished: true);
			tweenAlpha.enabled = false;
		}
		return tweenAlpha;
	}

	public override void SetStartToCurrentValue()
	{
		from = value;
	}

	public override void SetEndToCurrentValue()
	{
		to = value;
	}
}
