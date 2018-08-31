using System;
using System.Collections;
using UnityEngine;

public class LTDescr
{
	public bool toggle;

	public bool useEstimatedTime;

	public bool useFrames;

	public bool hasInitiliazed;

	public bool hasPhysics;

	public float passed;

	public float delay;

	public float time;

	public float lastVal;

	private uint _id;

	public int loopCount;

	public uint counter;

	public float direction;

	public bool destroyOnComplete;

	public Transform trans;

	public LTRect ltRect;

	public Vector3 from;

	public Vector3 to;

	public Vector3 diff;

	public Vector3 point;

	public Vector3 axis;

	public Vector3 origRotation;

	public LTBezierPath path;

	public LTSpline spline;

	public TweenAction type;

	public LeanTweenType tweenType;

	public AnimationCurve animationCurve;

	public LeanTweenType loopType;

	public Action<float> onUpdateFloat;

	public Action<float, object> onUpdateFloatObject;

	public Action<Vector3> onUpdateVector3;

	public Action<Vector3, object> onUpdateVector3Object;

	public Action onComplete;

	public Action<object> onCompleteObject;

	public object onCompleteParam;

	public object onUpdateParam;

	public Hashtable optional;

	private static uint global_counter;

	public int uniqueId => (int)(_id | (counter << 16));

	public int id => uniqueId;

	public override string ToString()
	{
		return ((!(trans != null)) ? "gameObject:null" : ("gameObject:" + trans.gameObject)) + " toggle:" + toggle + " passed:" + passed + " time:" + time + " delay:" + delay + " from:" + from + " to:" + to + " type:" + type + " useEstimatedTime:" + useEstimatedTime + " id:" + id + " hasInitiliazed:" + hasInitiliazed;
	}

	public LTDescr cancel()
	{
		LeanTween.removeTween((int)_id);
		return this;
	}

	public void reset()
	{
		toggle = true;
		optional = null;
		destroyOnComplete = false;
		passed = (delay = 0f);
		useEstimatedTime = (useFrames = (hasInitiliazed = false));
		animationCurve = null;
		tweenType = LeanTweenType.linear;
		loopType = LeanTweenType.once;
		loopCount = 0;
		direction = (lastVal = 1f);
		onUpdateFloat = null;
		onUpdateVector3 = null;
		onUpdateFloatObject = null;
		onUpdateVector3Object = null;
		onComplete = null;
		onCompleteObject = null;
		onCompleteParam = null;
		point = Vector3.zero;
		global_counter++;
	}

	public LTDescr pause()
	{
		if (direction != 0f)
		{
			lastVal = direction;
			direction = 0f;
		}
		return this;
	}

	public LTDescr resume()
	{
		direction = lastVal;
		return this;
	}

	public LTDescr setAxis(Vector3 axis)
	{
		this.axis = axis;
		return this;
	}

	public LTDescr setDelay(float delay)
	{
		if (useEstimatedTime)
		{
			this.delay = delay;
		}
		else
		{
			this.delay = delay * Time.timeScale;
		}
		return this;
	}

	public LTDescr setEase(LeanTweenType easeType)
	{
		tweenType = easeType;
		return this;
	}

	public LTDescr setEase(AnimationCurve easeCurve)
	{
		animationCurve = easeCurve;
		return this;
	}

	public LTDescr setTo(Vector3 to)
	{
		this.to = to;
		return this;
	}

	public LTDescr setFrom(Vector3 from)
	{
		this.from = from;
		hasInitiliazed = true;
		diff = to - this.from;
		return this;
	}

	public LTDescr setId(uint id)
	{
		_id = id;
		counter = global_counter;
		return this;
	}

	public LTDescr setRepeat(int repeat)
	{
		loopCount = repeat;
		if ((repeat > 1 && loopType == LeanTweenType.once) || (repeat < 0 && loopType == LeanTweenType.once))
		{
			loopType = LeanTweenType.clamp;
		}
		return this;
	}

	public LTDescr setLoopType(LeanTweenType loopType)
	{
		this.loopType = loopType;
		return this;
	}

	public LTDescr setUseEstimatedTime(bool useEstimatedTime)
	{
		this.useEstimatedTime = useEstimatedTime;
		return this;
	}

	public LTDescr setUseFrames(bool useFrames)
	{
		this.useFrames = useFrames;
		return this;
	}

	public LTDescr setLoopCount(int loopCount)
	{
		this.loopCount = loopCount;
		return this;
	}

	public LTDescr setLoopOnce()
	{
		loopType = LeanTweenType.once;
		return this;
	}

	public LTDescr setLoopClamp()
	{
		loopType = LeanTweenType.clamp;
		if (loopCount == 0)
		{
			loopCount = -1;
		}
		return this;
	}

	public LTDescr setLoopPingPong()
	{
		loopType = LeanTweenType.pingPong;
		if (loopCount == 0)
		{
			loopCount = -1;
		}
		return this;
	}

	public LTDescr setOnComplete(Action onComplete)
	{
		this.onComplete = onComplete;
		return this;
	}

	public LTDescr setOnComplete(Action<object> onComplete)
	{
		onCompleteObject = onComplete;
		return this;
	}

	public LTDescr setOnComplete(Action<object> onComplete, object onCompleteParam)
	{
		onCompleteObject = onComplete;
		if (onCompleteParam != null)
		{
			this.onCompleteParam = onCompleteParam;
		}
		return this;
	}

	public LTDescr setOnCompleteParam(object onCompleteParam)
	{
		this.onCompleteParam = onCompleteParam;
		return this;
	}

	public LTDescr setOnUpdate(Action<float> onUpdate)
	{
		onUpdateFloat = onUpdate;
		return this;
	}

	public LTDescr setOnUpdateObject(Action<float, object> onUpdate)
	{
		onUpdateFloatObject = onUpdate;
		return this;
	}

	public LTDescr setOnUpdateVector3(Action<Vector3> onUpdate)
	{
		onUpdateVector3 = onUpdate;
		return this;
	}

	public LTDescr setOnUpdate(Action<float, object> onUpdate, object onUpdateParam = null)
	{
		onUpdateFloatObject = onUpdate;
		if (onUpdateParam != null)
		{
			this.onUpdateParam = onUpdateParam;
		}
		return this;
	}

	public LTDescr setOnUpdate(Action<Vector3, object> onUpdate, object onUpdateParam = null)
	{
		onUpdateVector3Object = onUpdate;
		if (onUpdateParam != null)
		{
			this.onUpdateParam = onUpdateParam;
		}
		return this;
	}

	public LTDescr setOnUpdate(Action<Vector3> onUpdate, object onUpdateParam = null)
	{
		onUpdateVector3 = onUpdate;
		if (onUpdateParam != null)
		{
			this.onUpdateParam = onUpdateParam;
		}
		return this;
	}

	public LTDescr setOnUpdateParam(object onUpdateParam)
	{
		this.onUpdateParam = onUpdateParam;
		return this;
	}

	public LTDescr setOrientToPath(bool doesOrient)
	{
		if (type == TweenAction.MOVE_CURVED || type == TweenAction.MOVE_CURVED_LOCAL)
		{
			if (path == null)
			{
				path = new LTBezierPath();
			}
			path.orientToPath = doesOrient;
		}
		else
		{
			spline.orientToPath = doesOrient;
		}
		return this;
	}

	public LTDescr setRect(LTRect rect)
	{
		ltRect = rect;
		return this;
	}

	public LTDescr setRect(Rect rect)
	{
		ltRect = new LTRect(rect);
		return this;
	}

	public LTDescr setPath(LTBezierPath path)
	{
		this.path = path;
		return this;
	}

	public LTDescr setPoint(Vector3 point)
	{
		this.point = point;
		return this;
	}

	public LTDescr setDestroyOnComplete(bool doesDestroy)
	{
		destroyOnComplete = doesDestroy;
		return this;
	}

	public LTDescr setAudio(object audio)
	{
		onCompleteParam = audio;
		return this;
	}
}
