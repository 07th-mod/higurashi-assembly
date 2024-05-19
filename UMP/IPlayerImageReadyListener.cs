using UnityEngine;

namespace UMP
{
	public interface IPlayerImageReadyListener
	{
		void OnPlayerImageReady(Texture2D videoTexture);
	}
}
