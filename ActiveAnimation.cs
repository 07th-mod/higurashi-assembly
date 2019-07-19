using AnimationOrTween;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Internal/Active Animation")]
public class ActiveAnimation : MonoBehaviour
{
	public static ActiveAnimation current;

	public List<EventDelegate> onFinished = new List<EventDelegate>();

	[HideInInspector]
	public GameObject eventReceiver;

	[HideInInspector]
	public string callWhenFinished;

	private Animation mAnim;

	private Direction mLastDirection;

	private Direction mDisableDirection;

	private bool mNotify;

	private Animator mAnimator;

	private string mClip = string.Empty;

	private float playbackTime => Mathf.Clamp01(mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);

	public bool isPlaying
	{
		get
		{
			if (mAnim == null)
			{
				if (mAnimator != null)
				{
					if (mLastDirection == Direction.Reverse)
					{
						if (playbackTime == 0f)
						{
							return false;
						}
					}
					else if (playbackTime == 1f)
					{
						return false;
					}
					return true;
				}
				return false;
			}
			IEnumerator enumerator = mAnim.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					AnimationState animationState = (AnimationState)enumerator.Current;
					if (mAnim.IsPlaying(animationState.name))
					{
						if (mLastDirection == Direction.Forward)
						{
							if (animationState.time < animationState.length)
							{
								return true;
							}
						}
						else
						{
							if (mLastDirection != Direction.Reverse)
							{
								return true;
							}
							if (animationState.time > 0f)
							{
								return true;
							}
						}
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			return false;
		}
	}

	public void Finish()
	{
		if (mAnim != null)
		{
			IEnumerator enumerator = mAnim.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					AnimationState animationState = (AnimationState)enumerator.Current;
					if (mLastDirection == Direction.Forward)
					{
						animationState.time = animationState.length;
					}
					else if (mLastDirection == Direction.Reverse)
					{
						animationState.time = 0f;
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			mAnim.Sample();
		}
		else if (mAnimator != null)
		{
			mAnimator.Play(mClip, 0, (mLastDirection != Direction.Forward) ? 0f : 1f);
		}
	}

	public void Reset()
	{
		if (mAnim != null)
		{
			IEnumerator enumerator = mAnim.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					AnimationState animationState = (AnimationState)enumerator.Current;
					if (mLastDirection == Direction.Reverse)
					{
						animationState.time = animationState.length;
					}
					else if (mLastDirection == Direction.Forward)
					{
						animationState.time = 0f;
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}
		else if (mAnimator != null)
		{
			mAnimator.Play(mClip, 0, (mLastDirection != Direction.Reverse) ? 0f : 1f);
		}
	}

	private void Start()
	{
		if (eventReceiver != null && EventDelegate.IsValid(onFinished))
		{
			eventReceiver = null;
			callWhenFinished = null;
		}
	}

	private void Update()
	{
		float deltaTime = RealTime.deltaTime;
		if (deltaTime == 0f)
		{
			return;
		}
		if (mAnimator != null)
		{
			mAnimator.Update((mLastDirection != Direction.Reverse) ? deltaTime : (0f - deltaTime));
			if (isPlaying)
			{
				return;
			}
			mAnimator.enabled = false;
			base.enabled = false;
		}
		else
		{
			if (!(mAnim != null))
			{
				base.enabled = false;
				return;
			}
			bool flag = false;
			IEnumerator enumerator = mAnim.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					AnimationState animationState = (AnimationState)enumerator.Current;
					if (mAnim.IsPlaying(animationState.name))
					{
						float num = animationState.speed * deltaTime;
						animationState.time += num;
						if (num < 0f)
						{
							if (animationState.time > 0f)
							{
								flag = true;
							}
							else
							{
								animationState.time = 0f;
							}
						}
						else if (animationState.time < animationState.length)
						{
							flag = true;
						}
						else
						{
							animationState.time = animationState.length;
						}
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			mAnim.Sample();
			if (flag)
			{
				return;
			}
			base.enabled = false;
		}
		if (!mNotify)
		{
			return;
		}
		mNotify = false;
		if (current == null)
		{
			current = this;
			EventDelegate.Execute(onFinished);
			if (eventReceiver != null && !string.IsNullOrEmpty(callWhenFinished))
			{
				eventReceiver.SendMessage(callWhenFinished, SendMessageOptions.DontRequireReceiver);
			}
			current = null;
		}
		if (mDisableDirection != 0 && mLastDirection == mDisableDirection)
		{
			NGUITools.SetActive(base.gameObject, state: false);
		}
	}

	private void Play(string clipName, Direction playDirection)
	{
		if (playDirection == Direction.Toggle)
		{
			playDirection = ((mLastDirection != Direction.Forward) ? Direction.Forward : Direction.Reverse);
		}
		if (mAnim != null)
		{
			base.enabled = true;
			mAnim.enabled = false;
			if (string.IsNullOrEmpty(clipName))
			{
				if (!mAnim.isPlaying)
				{
					mAnim.Play();
				}
			}
			else if (!mAnim.IsPlaying(clipName))
			{
				mAnim.Play(clipName);
			}
			IEnumerator enumerator = mAnim.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					AnimationState animationState = (AnimationState)enumerator.Current;
					if (string.IsNullOrEmpty(clipName) || animationState.name == clipName)
					{
						float num = Mathf.Abs(animationState.speed);
						animationState.speed = num * (float)playDirection;
						if (playDirection == Direction.Reverse && animationState.time == 0f)
						{
							animationState.time = animationState.length;
						}
						else if (playDirection == Direction.Forward && animationState.time == animationState.length)
						{
							animationState.time = 0f;
						}
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			mLastDirection = playDirection;
			mNotify = true;
			mAnim.Sample();
		}
		else if (mAnimator != null)
		{
			if (base.enabled && isPlaying && mClip == clipName)
			{
				mLastDirection = playDirection;
				return;
			}
			base.enabled = true;
			mNotify = true;
			mLastDirection = playDirection;
			mClip = clipName;
			mAnimator.Play(mClip, 0, (playDirection != Direction.Forward) ? 1f : 0f);
		}
	}

	public static ActiveAnimation Play(Animation anim, string clipName, Direction playDirection, EnableCondition enableBeforePlay, DisableCondition disableCondition)
	{
		if (!NGUITools.GetActive(anim.gameObject))
		{
			if (enableBeforePlay != EnableCondition.EnableThenPlay)
			{
				return null;
			}
			NGUITools.SetActive(anim.gameObject, state: true);
			UIPanel[] componentsInChildren = anim.gameObject.GetComponentsInChildren<UIPanel>();
			int i = 0;
			for (int num = componentsInChildren.Length; i < num; i++)
			{
				componentsInChildren[i].Refresh();
			}
		}
		ActiveAnimation activeAnimation = anim.GetComponent<ActiveAnimation>();
		if (activeAnimation == null)
		{
			activeAnimation = anim.gameObject.AddComponent<ActiveAnimation>();
		}
		activeAnimation.mAnim = anim;
		activeAnimation.mDisableDirection = (Direction)disableCondition;
		activeAnimation.onFinished.Clear();
		activeAnimation.Play(clipName, playDirection);
		if (activeAnimation.mAnim != null)
		{
			activeAnimation.mAnim.Sample();
		}
		else if (activeAnimation.mAnimator != null)
		{
			activeAnimation.mAnimator.Update(0f);
		}
		return activeAnimation;
	}

	public static ActiveAnimation Play(Animation anim, string clipName, Direction playDirection)
	{
		return Play(anim, clipName, playDirection, EnableCondition.DoNothing, DisableCondition.DoNotDisable);
	}

	public static ActiveAnimation Play(Animation anim, Direction playDirection)
	{
		return Play(anim, null, playDirection, EnableCondition.DoNothing, DisableCondition.DoNotDisable);
	}

	public static ActiveAnimation Play(Animator anim, string clipName, Direction playDirection, EnableCondition enableBeforePlay, DisableCondition disableCondition)
	{
		if (enableBeforePlay != EnableCondition.IgnoreDisabledState && !NGUITools.GetActive(anim.gameObject))
		{
			if (enableBeforePlay != EnableCondition.EnableThenPlay)
			{
				return null;
			}
			NGUITools.SetActive(anim.gameObject, state: true);
			UIPanel[] componentsInChildren = anim.gameObject.GetComponentsInChildren<UIPanel>();
			int i = 0;
			for (int num = componentsInChildren.Length; i < num; i++)
			{
				componentsInChildren[i].Refresh();
			}
		}
		ActiveAnimation activeAnimation = anim.GetComponent<ActiveAnimation>();
		if (activeAnimation == null)
		{
			activeAnimation = anim.gameObject.AddComponent<ActiveAnimation>();
		}
		activeAnimation.mAnimator = anim;
		activeAnimation.mDisableDirection = (Direction)disableCondition;
		activeAnimation.onFinished.Clear();
		activeAnimation.Play(clipName, playDirection);
		if (activeAnimation.mAnim != null)
		{
			activeAnimation.mAnim.Sample();
		}
		else if (activeAnimation.mAnimator != null)
		{
			activeAnimation.mAnimator.Update(0f);
		}
		return activeAnimation;
	}
}
