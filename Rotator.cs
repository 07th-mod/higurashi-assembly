using UnityEngine;

public class Rotator : MonoBehaviour
{
	public float StartPos;

	public float EndPos = 360f;

	public float Duration = 1f;

	private float starttime;

	public bool EaseType;

	public void Reverse()
	{
		float startPos = StartPos;
		StartPos = EndPos;
		StartPos = startPos;
	}

	public void Restart()
	{
		starttime = Time.time;
	}

	private void Start()
	{
		starttime = Time.time;
	}

	private void Update()
	{
		if (Time.time - starttime <= Duration)
		{
			float num = 0f;
			num = (EaseType ? ((float)Quad.EaseOut(Time.time - starttime, 0.0, 1.0, Duration) * (EndPos - StartPos) + StartPos) : ((float)Quad.EaseInOut(Time.time - starttime, 0.0, 1.0, Duration) * (EndPos - StartPos) + StartPos));
			base.transform.localEulerAngles = new Vector3(0f, 0f, num);
		}
		else
		{
			base.transform.localEulerAngles = new Vector3(0f, 0f, EndPos);
		}
	}
}
