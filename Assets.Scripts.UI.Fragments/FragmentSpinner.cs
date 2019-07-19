using UnityEngine;

namespace Assets.Scripts.UI.Fragments
{
	internal class FragmentSpinner : MonoBehaviour
	{
		public Material Mat;

		public float MinSpin = -360f;

		public float MaxSpin = 360f;

		public float MinTime = 4f;

		public float MaxTime = 12f;

		public float MinDistance = 60f;

		private float curSpinX;

		private float curSpinY;

		private float curSpinZ;

		private LTDescr spinXDesc;

		private LTDescr spinYDesc;

		private LTDescr spinZDesc;

		private void Awake()
		{
			curSpinX = Random.Range(MinSpin, MaxSpin);
			curSpinY = Random.Range(MinSpin, MaxSpin);
			curSpinZ = Random.Range(MinSpin, MaxSpin);
			Vector3 eulerAngles = new Vector3(curSpinX, curSpinY, curSpinZ);
			base.transform.eulerAngles = eulerAngles;
			NewSpinX();
			NewSpinY();
			NewSpinZ();
		}

		private void NewSpinX(bool midspin = false)
		{
			float num = Random.Range(MinSpin, MaxSpin);
			float time = Random.Range(MinTime, MaxTime);
			float num2 = curSpinX;
			if (num < num2 && num2 - num < MinDistance)
			{
				num -= MinDistance;
			}
			if (num > num2 && num - num2 < MinDistance)
			{
				num += MinDistance;
			}
			spinXDesc = LeanTween.value(base.gameObject, delegate(float f)
			{
				curSpinX = f;
			}, num2, num, time);
			spinXDesc.onComplete = delegate
			{
				NewSpinX();
			};
			spinXDesc.setEase(LeanTweenType.easeInOutSine);
			if (midspin && Random.Range(0, 2) == 1)
			{
				spinXDesc.setEase(LeanTweenType.easeOutSine);
			}
		}

		private void NewSpinY(bool midspin = false)
		{
			float num = Random.Range(MinSpin, MaxSpin);
			float time = Random.Range(MinTime, MaxTime);
			float num2 = curSpinY;
			if (num < num2 && num2 - num < MinDistance)
			{
				num -= MinDistance;
			}
			if (num > num2 && num - num2 < MinDistance)
			{
				num += MinDistance;
			}
			spinYDesc = LeanTween.value(base.gameObject, delegate(float f)
			{
				curSpinY = f;
			}, num2, num, time);
			spinYDesc.onComplete = delegate
			{
				NewSpinY();
			};
			spinYDesc.setEase(LeanTweenType.easeInOutSine);
			if (midspin && Random.Range(0, 2) == 1)
			{
				spinYDesc.setEase(LeanTweenType.easeOutSine);
			}
		}

		private void NewSpinZ(bool midspin = false)
		{
			float num = Random.Range(MinSpin, MaxSpin);
			float time = Random.Range(MinTime, MaxTime);
			float num2 = curSpinZ;
			if (num < num2 && num2 - num < MinDistance)
			{
				num -= MinDistance;
			}
			if (num > num2 && num - num2 < MinDistance)
			{
				num += MinDistance;
			}
			spinZDesc = LeanTween.value(base.gameObject, delegate(float f)
			{
				curSpinZ = f;
			}, num2, num, time);
			spinZDesc.onComplete = delegate
			{
				NewSpinZ();
			};
			spinZDesc.setEase(LeanTweenType.easeInOutSine);
			if (midspin && Random.Range(0, 2) == 1)
			{
				spinZDesc.setEase(LeanTweenType.easeOutSine);
			}
		}

		private void Update()
		{
			Vector3 eulerAngles = new Vector3(curSpinX, curSpinY, curSpinZ);
			base.transform.eulerAngles = eulerAngles;
		}
	}
}
