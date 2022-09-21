using Assets.Scripts.Core;
using Assets.Scripts.Core.Scene;
using System;
using System.IO;
using UnityEngine;

namespace MOD.Scripts.Core.Movie
{
	public class MovieInfo
	{
		public static string GetPathFromNameWithExt(string name, string ext)
		{
			return System.IO.Path.Combine(Application.streamingAssetsPath, "movies/" + name + ext);
		}

		private readonly string Ext;

		public string Name
		{
			get;
			private set;
		}

		public float Volume => Math.Min(1f, 1.55f * GameSystem.Instance.AudioController.BGMVolume);

		public string PathWithExt => "file:///" + GetPathFromNameWithExt(Name, Ext);

		public Layer Layer => GameSystem.Instance.SceneController.MODActiveScene.BackgroundLayer;

		public MovieInfo(string name, string ext)
		{
			Name = name;
			Ext = ext;
		}
	}
}
