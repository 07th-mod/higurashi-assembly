using UnityEngine;

namespace Assets.Scripts.Core.Scene
{
	public class Shaker : MonoBehaviour
	{
		private Vector3 srcPos = Vector3.zero;

		private Vector3 lastPos;

		private Vector3 destination;

		private bool isActive = true;

		private float intensity;

		private int shaketype;

		private int attenuation;

		private int remainingcount;

		private float timeperswing;

		private float timetoswitch;

		private bool shakedir;

		private bool infinite;

		public static void ShakeObject(GameObject target, float speed, int level, int attenuation, int vector, int loopcount, bool isblocking)
		{
			Shaker component = target.GetComponent<Shaker>();
			if (component != null)
			{
				component.StopShake();
			}
			GameSystem.Instance.RegisterAction(delegate
			{
				Shaker shaker = target.AddComponent<Shaker>();
				shaker.StartShake(speed, level, attenuation, vector, loopcount, isblocking);
			});
			if (isblocking)
			{
				GameSystem.Instance.ExecuteActions();
			}
		}

		public void StartShake(float speed, int level, int atten, int vector, int loopcount, bool isblocking)
		{
			isActive = true;
			infinite = false;
			srcPos = base.gameObject.transform.localPosition;
			lastPos = srcPos;
			intensity = (float)level;
			attenuation = atten;
			shaketype = vector;
			shakedir = false;
			remainingcount = loopcount;
			timeperswing = speed;
			timetoswitch = timeperswing / 2f;
			if (timeperswing < 0.01f)
			{
				timeperswing = 0.01f;
			}
			float num = timeperswing * (float)loopcount + (float)loopcount * 0.005f + timetoswitch;
			num += (float)loopcount * 0.005f;
			if (loopcount == 0)
			{
				num = 2.14748365E+09f;
				infinite = true;
			}
			UpdateShake();
			if (isblocking && loopcount != 0)
			{
				GameSystem.Instance.AddWait(new Wait(num, WaitTypes.WaitForMove, StopShake));
			}
			if (isblocking)
			{
				GameSystem.Instance.ExecuteActions();
			}
		}

		private void UpdateShake()
		{
			SetShakeTarget();
		}

		private void SetShakeTarget()
		{
			int num = 1;
			if (shakedir)
			{
				num = -1;
			}
			float num2 = intensity;
			lastPos = destination;
			switch (shaketype)
			{
			case 0:
				destination = srcPos + new Vector3(intensity * (float)num, 0f, 0f);
				break;
			case 1:
				destination = srcPos + new Vector3(intensity * (float)num, intensity * (float)(-num), 0f);
				break;
			case 2:
				destination = srcPos + new Vector3(0f, intensity * (float)num, 0f);
				break;
			case 3:
				destination = srcPos + new Vector3(intensity * (float)(-num), intensity * (float)num, 0f);
				break;
			case 4:
				destination = srcPos + new Vector3(Random.Range(0f - num2, num2), Random.Range(0f - num2, num2), 0f);
				break;
			default:
				Debug.LogError("Error: Undefined shake type " + shaketype);
				break;
			}
			shakedir = !shakedir;
			intensity *= 1f - (float)attenuation / 100f;
		}

		public void StopShake()
		{
			isActive = false;
		}

		private void Update()
		{
			if (isActive)
			{
				timetoswitch -= Time.deltaTime;
				float num = 1f - timetoswitch / timeperswing;
				float num2 = (float)Sine.EaseInOut((double)num, (double)lastPos.x, (double)(destination.x - lastPos.x), 1.0);
				float num3 = (float)Sine.EaseInOut((double)num, (double)lastPos.y, (double)(destination.y - lastPos.y), 1.0);
				Transform transform = base.transform;
				float x = num2;
				float y = num3;
				Vector3 localPosition = base.transform.localPosition;
				transform.localPosition = new Vector3(x, y, localPosition.z);
				if (!(timetoswitch > 0f))
				{
					base.transform.localPosition = destination;
					if (remainingcount > 0 || infinite)
					{
						timetoswitch = timeperswing;
						UpdateShake();
						remainingcount--;
					}
					else
					{
						isActive = false;
						base.transform.localPosition = srcPos;
					}
				}
			}
		}

		private void LateUpdate()
		{
			if (!isActive)
			{
				Object.Destroy(this);
			}
		}

		private void OnDestroy()
		{
			isActive = false;
			base.transform.localPosition = srcPos;
		}
	}
}
