using UnityEngine;

public class UI2DSpriteAnimation : MonoBehaviour
{
	[SerializeField]
	protected int framerate = 20;

	public bool ignoreTimeScale = true;

	public bool loop = true;

	public Sprite[] frames;

	private SpriteRenderer mUnitySprite;

	private UI2DSprite mNguiSprite;

	private int mIndex;

	private float mUpdate;

	public bool isPlaying => base.enabled;

	public int framesPerSecond
	{
		get
		{
			return framerate;
		}
		set
		{
			framerate = value;
		}
	}

	public void Play()
	{
		if (frames == null || frames.Length <= 0)
		{
			return;
		}
		if (!base.enabled && !loop)
		{
			int num = (framerate <= 0) ? (mIndex - 1) : (mIndex + 1);
			if (num < 0 || num >= frames.Length)
			{
				mIndex = ((framerate < 0) ? (frames.Length - 1) : 0);
			}
		}
		base.enabled = true;
		UpdateSprite();
	}

	public void Pause()
	{
		base.enabled = false;
	}

	public void ResetToBeginning()
	{
		mIndex = ((framerate < 0) ? (frames.Length - 1) : 0);
		UpdateSprite();
	}

	private void Start()
	{
		Play();
	}

	private void Update()
	{
		if (frames == null || frames.Length == 0)
		{
			base.enabled = false;
		}
		else
		{
			if (framerate == 0)
			{
				return;
			}
			float num = (!ignoreTimeScale) ? Time.time : RealTime.time;
			if (mUpdate < num)
			{
				mUpdate = num;
				int num2 = (framerate <= 0) ? (mIndex - 1) : (mIndex + 1);
				if (!loop && (num2 < 0 || num2 >= frames.Length))
				{
					base.enabled = false;
					return;
				}
				mIndex = NGUIMath.RepeatIndex(num2, frames.Length);
				UpdateSprite();
			}
		}
	}

	private void UpdateSprite()
	{
		if (mUnitySprite == null && mNguiSprite == null)
		{
			mUnitySprite = GetComponent<SpriteRenderer>();
			mNguiSprite = GetComponent<UI2DSprite>();
			if (mUnitySprite == null && mNguiSprite == null)
			{
				base.enabled = false;
				return;
			}
		}
		float num = (!ignoreTimeScale) ? Time.time : RealTime.time;
		if (framerate != 0)
		{
			mUpdate = num + Mathf.Abs(1f / (float)framerate);
		}
		if (mUnitySprite != null)
		{
			mUnitySprite.sprite = frames[mIndex];
		}
		else if (mNguiSprite != null)
		{
			mNguiSprite.nextSprite = frames[mIndex];
		}
	}
}
