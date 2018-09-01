using UnityEngine;

namespace Assets.Scripts.Core
{
	public class Wait
	{
		private OnFinishWait onFinish;

		public WaitTypes Type;

		private float time;

		public bool IsActive;

		public Wait(float length, WaitTypes type, OnFinishWait onFinishDelegate)
		{
			Type = type;
			time = length;
			onFinish = onFinishDelegate;
		}

		public override string ToString()
		{
			return $"Wait {Type} IsActive: {IsActive} RemainingTime: {time}";
		}

		public float GetTime()
		{
			return time;
		}

		public void Update()
		{
			time -= Time.deltaTime;
			IsActive = true;
		}

		public bool IsRunning()
		{
			if (Type == WaitTypes.WaitForInput)
			{
				return true;
			}
			if (time > 0f)
			{
				return true;
			}
			Finish();
			return false;
		}

		public void Finish()
		{
			if (IsActive && onFinish != null)
			{
				onFinish();
			}
		}
	}
}
