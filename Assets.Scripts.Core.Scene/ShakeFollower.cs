using Assets.Scripts.UI;
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


		public static void MODFollowShakeOfObjectMainUIOnly(GameObject go, GameObject target, float time, float scale, bool flipPosition)
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

		// 07th-mod: Updated as vanilla code assumes UI has localposition = Vector3.zero, but in our mod
		// we adjust the localposition of the MainUI to place the UI at the bottom right of the 16:9 screen
		//
		// base.transform.localPosition appears to be the MainUI position, eg (170.0, 0.0, 0.0) when the
		// UI is adjusted for our mod by MainUIController.UpdateGuiPosition()
		public void LateUpdate()
		{
			Life -= Time.deltaTime;
			if (Life < 0f || TargetShaker == null)
			{
				base.transform.localPosition = GameSystem.Instance.MainUIController.MODGetScaledGuiPosition();
				Object.Destroy(this);
			}
			else if (!FlipPosition)
			{
				base.transform.localPosition = GameSystem.Instance.MainUIController.MODGetScaledGuiPosition() + Target.transform.localPosition * Scale;
			}
			else
			{
				base.transform.localPosition = GameSystem.Instance.MainUIController.MODGetScaledGuiPosition() - Target.transform.localPosition * Scale;
			}
		}
	}
}
