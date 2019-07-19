using UnityEngine;

public class Orbit : MonoBehaviour
{
	public float degreesPerSecond = 10f;

	private void Update()
	{
		base.transform.RotateAround(Vector3.zero, Vector3.up, degreesPerSecond * Time.deltaTime);
	}
}
