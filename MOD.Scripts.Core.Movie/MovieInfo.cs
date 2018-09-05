using Assets.Scripts.Core;
using Assets.Scripts.Core.Scene;
using System;
using System.IO;
using UnityEngine;

namespace MOD.Scripts.Core.Movie
{
	public class MovieInfo
	{
		public string Name
		{
			get;
			private set;
		}

		public float Volume => Math.Min(1f, 1.55f * GameSystem.Instance.AudioController.BGMVolume);

		public string Path => "file:///" + System.IO.Path.Combine(Application.streamingAssetsPath, "movies/" + Name);

		public Layer Layer => GameSystem.Instance.SceneController.MODActiveScene.BackgroundLayer;

		public MovieInfo(string name)
		{
			Name = name;
		}
	}
}
