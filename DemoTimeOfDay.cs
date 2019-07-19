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
		if (!(bool)lightSource)
		{
			base.enabled = false;
		}
		Vector3 eulerAngles = base.transform.eulerAngles;
		maxAngle = eulerAngles.x;
		Vector3 eulerAngles2 = base.transform.eulerAngles;
		yAngle = eulerAngles2.y;
		maxIntensity = lightSource.intensity;
	}

	private void Update()
	{
		float num = Time.time / cycleDuration;
		float num2 = Mathf.Cos(num * (float)Math.PI * 2f) * 0.5f + 0.5f;
		lightSource.intensity = num2 * maxIntensity;
		float x = minAngle + num2 * (maxAngle - minAngle);
		base.transform.eulerAngles = new Vector3(x, yAngle, 0f);
		DynamicGI.UpdateEnvironment();
	}
}
