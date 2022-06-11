using System;
using UnityEngine;

public class DemoTimeOfDay : MonoBehaviour
{
	public Light lightSource;

	public float minAngle = -15f;

	public float cycleDuration = 10f;

	private float maxAngle = 50f;

	private float yAngle = 60f;

	private float maxIntensity = 1f;

	private void Start()
	{
		if (!lightSource)
		{
			base.enabled = false;
		}
		maxAngle = base.transform.eulerAngles.x;
		yAngle = base.transform.eulerAngles.y;
		maxIntensity = lightSource.intensity;
	}

	private void Update()
	{
		float num = Mathf.Cos(Time.time / cycleDuration * (float)Math.PI * 2f) * 0.5f + 0.5f;
		lightSource.intensity = num * maxIntensity;
		float x = minAngle + num * (maxAngle - minAngle);
		base.transform.eulerAngles = new Vector3(x, yAngle, 0f);
		DynamicGI.UpdateEnvironment();
	}
}
