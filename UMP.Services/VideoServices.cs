using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UMP.Services.Youtube;
using UnityEngine;

namespace UMP.Services
{
	public class VideoServices
	{
		private MonoBehaviour _monoObject;

		private List<ServiceBase> _services;

		private IEnumerator _getVideosEnum;

		public VideoServices(MonoBehaviour monoObject)
		{
			_monoObject = monoObject;
			_services = new List<ServiceBase>();
			_services.Add(YoutubeService.Default);
		}

		public bool ValidUrl(string url)
		{
			bool result = false;
			foreach (ServiceBase service in _services)
			{
				if (service.ValidUrl(url))
				{
					return true;
				}
			}
			return result;
		}

		public IEnumerator GetVideos(string url, Action<List<Video>> resultCallback, Action<string> errorCallback)
		{
			foreach (ServiceBase service in _services)
			{
				if (service.ValidUrl(url))
				{
					_getVideosEnum = service.GetAllVideos(url, resultCallback, errorCallback);
					yield return _monoObject.StartCoroutine(_getVideosEnum);
				}
			}
		}

		public static Video FindVideo(List<Video> videos, int maxResolution, int maxAudioBitrate = -1)
		{
			Video result = null;
			if (videos != null && videos.Count > 0)
			{
				result = videos[0];
				List<YoutubeVideo> list = new List<YoutubeVideo>();
				try
				{
					list = videos.Cast<YoutubeVideo>().ToList();
				}
				catch (Exception ex)
				{
					ex.ToString();
				}
				if (list.Count > 0)
				{
					list = list.FindAll((YoutubeVideo video) => (maxAudioBitrate < 0) ? (video.Resolution <= maxResolution) : (video.Resolution <= maxResolution && video.AudioBitrate >= 0 && video.AudioBitrate <= maxAudioBitrate));
					IOrderedEnumerable<YoutubeVideo> source = from video in list
						orderby video.Resolution, video.AudioBitrate
						select video;
					result = source.LastOrDefault();
				}
			}
			return result;
		}
	}
}
