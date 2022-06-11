using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Sprite Animation")]
[ExecuteInEditMode]
[RequireComponent(typeof(UISprite))]
public class UISpriteAnimation : MonoBehaviour
{
	[HideInInspector]
	[SerializeField]
	protected int mFPS = 30;

	[HideInInspector]
	[SerializeField]
	protected string mPrefix = "";

	[HideInInspector]
	[SerializeField]
	protected bool mLoop = true;

	[HideInInspector]
	[SerializeField]
	protected bool mSnap = true;

	protected UISprite mSprite;

	protected float mDelta;

	protected int mIndex;

	protected bool mActive = true;

	protected List<string> mSpriteNames = new List<string>();

	public int frames => mSpriteNames.Count;

	public int framesPerSecond
	{
		get
		{
			return mFPS;
		}
		set
		{
			mFPS = value;
		}
	}

	public string namePrefix
	{
		get
		{
			return mPrefix;
		}
		set
		{
			if (mPrefix != value)
			{
				mPrefix = value;
				RebuildSpriteList();
			}
		}
	}

	public bool loop
	{
		get
		{
			return mLoop;
		}
		set
		{
			mLoop = value;
		}
	}

	public bool isPlaying => mActive;

	protected virtual void Start()
	{
		RebuildSpriteList();
	}

	protected virtual void Update()
	{
		if (!mActive || mSpriteNames.Count <= 1 || !Application.isPlaying || mFPS <= 0)
		{
			return;
		}
		mDelta += RealTime.deltaTime;
		float num = 1f / (float)mFPS;
		if (!(num < mDelta))
		{
			return;
		}
		mDelta = ((num > 0f) ? (mDelta - num) : 0f);
		if (++mIndex >= mSpriteNames.Count)
		{
			mIndex = 0;
			mActive = mLoop;
		}
		if (mActive)
		{
			mSprite.spriteName = mSpriteNames[mIndex];
			if (mSnap)
			{
				mSprite.MakePixelPerfect();
			}
		}
	}

	public void RebuildSpriteList()
	{
		if (mSprite == null)
		{
			mSprite = GetComponent<UISprite>();
		}
		mSpriteNames.Clear();
		if (!(mSprite != null) || !(mSprite.atlas != null))
		{
			return;
		}
		List<UISpriteData> spriteList = mSprite.atlas.spriteList;
		int i = 0;
		for (int count = spriteList.Count; i < count; i++)
		{
			UISpriteData uISpriteData = spriteList[i];
			if (string.IsNullOrEmpty(mPrefix) || uISpriteData.name.StartsWith(mPrefix))
			{
				mSpriteNames.Add(uISpriteData.name);
			}
		}
		mSpriteNames.Sort();
	}

	public void Play()
	{
		mActive = true;
	}

	public void Pause()
	{
		mActive = false;
	}

	public void ResetToBeginning()
	{
		mActive = true;
		mIndex = 0;
		if (mSprite != null && mSpriteNames.Count > 0)
		{
			mSprite.spriteName = mSpriteNames[mIndex];
			if (mSnap)
			{
				mSprite.MakePixelPerfect();
			}
		}
	}
}
