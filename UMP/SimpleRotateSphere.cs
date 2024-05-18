using UnityEngine;

namespace UMP
{
	public class SimpleRotateSphere : MonoBehaviour
	{
		private const int RMB_ID = 0;

		private Transform _cachedTransform;

		private Vector2? _rmbPrevPos;

		private float _x;

		private float _y;

		[SerializeField]
		[Range(1f, 100f)]
		private float _rotationSpeed = 4f;

		private void Awake()
		{
			_cachedTransform = Camera.main.transform;
		}

		private void Update()
		{
			TrackRotation();
		}

		private void TrackRotation()
		{
			if (_rmbPrevPos.HasValue)
			{
				if (Input.GetMouseButton(0))
				{
					Vector2 value = _rmbPrevPos.Value;
					int num = (int)value.x;
					Vector3 mousePosition = Input.mousePosition;
					if (num == (int)mousePosition.x)
					{
						Vector2 value2 = _rmbPrevPos.Value;
						int num2 = (int)value2.y;
						Vector3 mousePosition2 = Input.mousePosition;
						if (num2 == (int)mousePosition2.y)
						{
							return;
						}
					}
					float x = _x;
					Vector2 value3 = _rmbPrevPos.Value;
					float y = value3.y;
					Vector3 mousePosition3 = Input.mousePosition;
					_x = x + (y - mousePosition3.y) * Time.deltaTime * _rotationSpeed;
					float y2 = _y;
					Vector2 value4 = _rmbPrevPos.Value;
					float x2 = value4.x;
					Vector3 mousePosition4 = Input.mousePosition;
					_y = y2 - (x2 - mousePosition4.x) * Time.deltaTime * _rotationSpeed;
					_cachedTransform.rotation = Quaternion.Euler(_x, _y, 0f);
					_rmbPrevPos = Input.mousePosition;
				}
				else
				{
					_rmbPrevPos = null;
				}
			}
			else if (Input.GetMouseButtonDown(0))
			{
				_rmbPrevPos = Input.mousePosition;
			}
		}
	}
}
