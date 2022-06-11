using UnityEngine;

namespace Assets.Scripts.Core.Scene
{
	public class BlackBars : MonoBehaviour
	{
		public bool IsLeft;

		public void UpdatePosition()
		{
			int num = Mathf.RoundToInt(400f + 480f * GameSystem.Instance.AspectRatio / 2f);
			if (IsLeft)
			{
				num *= -1;
			}
			base.transform.localPosition = new Vector3(num, 0f, 0f);
		}
	}
}
