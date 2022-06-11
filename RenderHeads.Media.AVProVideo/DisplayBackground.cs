using UnityEngine;

namespace RenderHeads.Media.AVProVideo
{
	[AddComponentMenu("AVPro Video/Display Background", 200)]
	[ExecuteInEditMode]
	public class DisplayBackground : MonoBehaviour
	{
		public IMediaProducer _source;

		public Texture2D _texture;

		public Material _material;

		private void OnRenderObject()
		{
			if (!(_material == null) && !(_texture == null))
			{
				Vector4 vector = new Vector4(0f, 0f, 1f, 1f);
				_material.SetPass(0);
				GL.PushMatrix();
				GL.LoadOrtho();
				GL.Begin(7);
				GL.TexCoord2(vector.x, vector.y);
				GL.Vertex3(0f, 0f, 0.1f);
				GL.TexCoord2(vector.z, vector.y);
				GL.Vertex3(1f, 0f, 0.1f);
				GL.TexCoord2(vector.z, vector.w);
				GL.Vertex3(1f, 1f, 0.1f);
				GL.TexCoord2(vector.x, vector.w);
				GL.Vertex3(0f, 1f, 0.1f);
				GL.End();
				GL.PopMatrix();
			}
		}
	}
}
