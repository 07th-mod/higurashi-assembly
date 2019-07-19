using Assets.Scripts.Core;
using UnityEngine;

namespace MOD.Scripts.Core.Movie
{
	public class TextureMovieRenderer : MonoBehaviour, IMovieRenderer
	{
		private MovieTexture MovieTexture;

		private bool isStarted;

		private WWW www;

		private AudioSource audioSource;

		private MovieInfo movieInfo;

		public void Update()
		{
			if (www != null)
			{
				if (!string.IsNullOrEmpty(www.error))
				{
					Debug.LogError("Error playing video: " + www.error);
					Finished();
				}
				else if (www.isDone)
				{
					if (!isStarted)
					{
						MovieTexture = www.movie;
						movieInfo.Layer.MODMaterial.SetTexture("_Primary", MovieTexture);
						audioSource.clip = MovieTexture.audioClip;
						MovieTexture.Play();
						audioSource.Play();
						isStarted = true;
					}
					else if (!MovieTexture.isPlaying)
					{
						Finished();
					}
				}
			}
		}

		public void OnDestroy()
		{
			Quit();
			if (MovieTexture != null)
			{
				Object.Destroy(MovieTexture);
			}
			www?.Dispose();
		}

		public void Quit()
		{
			if (MovieTexture != null)
			{
				MovieTexture.Stop();
			}
			if (audioSource != null)
			{
				audioSource.Stop();
			}
			base.enabled = false;
		}

		public void Finished()
		{
			Quit();
			GameSystem.Instance.PopStateStack();
		}

		public TextureMovieRenderer()
		{
			base.enabled = false;
		}

		public void Init(MovieInfo movieInfo)
		{
			this.movieInfo = movieInfo;
			www = new WWW(movieInfo.Path + ".ogv");
			audioSource = base.gameObject.AddComponent<AudioSource>();
			audioSource.volume = this.movieInfo.Volume;
			base.enabled = true;
		}
	}
}
