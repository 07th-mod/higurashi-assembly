using UnityEngine;

public class DestroySelf : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		Object.Destroy(base.gameObject);
	}
}
