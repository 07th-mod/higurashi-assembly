using UnityEngine;

namespace RenderHeads.Media.AVProVideo
{
	[AddComponentMenu("AVPro Video/Update Stereo Material", 400)]
	public class UpdateStereoMaterial : MonoBehaviour
	{
		public Camera _camera;

		public MeshRenderer _renderer;

		public Material _material;

		private int _cameraPositionId;

		private int _viewMatrixId;

		private void Awake()
		{
			_cameraPositionId = Shader.PropertyToID("_cameraPosition");
			_viewMatrixId = Shader.PropertyToID("_ViewMatrix");
			if (_camera == null)
			{
				Debug.LogWarning("[AVProVideo] No camera set for UpdateStereoMaterial component. If you are rendering in stereo then it is recommended to set this.");
			}
		}

		private void LateUpdate()
		{
			Camera camera = _camera;
			if (camera == null)
			{
				camera = Camera.main;
			}
			if (_renderer == null && _material == null)
			{
				_renderer = base.gameObject.GetComponent<MeshRenderer>();
			}
			if (camera != null)
			{
				if (_renderer != null)
				{
					_renderer.material.SetVector(_cameraPositionId, camera.transform.position);
					_renderer.material.SetMatrix(_viewMatrixId, camera.worldToCameraMatrix.transpose);
				}
				if (_material != null)
				{
					_material.SetVector(_cameraPositionId, camera.transform.position);
					_material.SetMatrix(_viewMatrixId, camera.worldToCameraMatrix.transpose);
				}
			}
		}
	}
}
