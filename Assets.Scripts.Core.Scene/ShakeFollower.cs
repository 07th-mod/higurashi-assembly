using UnityEngine;

namespace Assets.Scripts.Core.Scene
{
	public class ShakeFollower : MonoBehaviour
	{
		public GameObject Target;

		public Shaker TargetShaker;

		public float Life;

		public float Scale;

		public bool FlipPosition;

		public static void FollowShakeOfObject(GameObject go, GameObject target, float time, float scale, bool flipPosition)
		{
			go.AddComponent<ShakeFollower>().StartFollow(target, time, scale, flipPosition);
		}

		public void StartFollow(GameObject target, float time, float scale, bool flipPosition)
		{
			Target = target;
			Life = time;
			TargetShaker = target.GetComponent<Shaker>();
			FlipPosition = flipPosition;
			Scale = scale;
		}

		public void LateUpdate()
		{
			Life -= Time.deltaTime;
			if (Life < 0f || TargetShaker == null)
			{
				base.transform.localPosition = Vector3.zero;
				Object.Destroy(this);
			}
			else if (!FlipPosition)
			{
				base.transform.localPosition = Target.transform.localPosition * Scale;
			}
			else
			{
				base.transform.localPosition = -Target.transform.localPosition * Scale;
			}
		}
	}
}
