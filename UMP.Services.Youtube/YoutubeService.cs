using Services.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UMP.Services.Helpers;
using UnityEngine;

namespace UMP.Services.Youtube
{
	public class YoutubeService : ServiceBase
	{
		private const string PLAYBACK = "videoplayback";

		private string[] _signatures = new string[3]
		{
			"youtu.be/",
			"www.youtube",
			"youtube.com/embed/"
		};

		public static YoutubeService Default => new YoutubeService();

		public override bool ValidUrl(string url)
		{
			string[] signatures = _signatures;
			foreach (string value in signatures)
			{
				if (url.Contains(value))
				{
					return true;
				}
			}
			return false;
		}

		public override IEnumerator GetAllVideos(string url, Action<List<Video>> resultCallback, Action<string> errorCallback = null)
		{
			if (!TryNormalize(url, out url))
			{
				throw new ArgumentException("URL is not a valid Youtube URL!");
			}
			string requestText2 = string.Empty;
			WWW request = new WWW(headers: new Dictionary<string, string>
			{
				{
					"User-Agent",
					string.Empty
				}
			}, url: url, postData: null);
			yield return request;
			if (!string.IsNullOrEmpty(request.error))
			{
				errorCallback($"[YouTubeService.GetAllVideos] url request is failed: {request.error}");
				yield break;
			}
			requestText2 = request.text;
			List<YoutubeVideo> ytVideos = new List<YoutubeVideo>();
			yield return ParseVideos(requestText2, delegate(List<YoutubeVideo> videos)
			{
				IOrderedEnumerable<YoutubeVideo> source = from video in videos
					orderby video.Resolution, video.AudioBitrate
					select video;
				ytVideos = source.ToList();
			}, errorCallback);
			resultCallback?.Invoke(ytVideos.Cast<Video>().ToList());
		}

		private bool TryNormalize(string url, out string normalized)
		{
			normalized = null;
			StringBuilder stringBuilder = new StringBuilder(url);
			url = stringBuilder.Replace("youtu.be/", "youtube.com/watch?v=").Replace("youtube.com/embed/", "youtube.com/watch?v=").Replace("/v/", "/watch?v=")
				.Replace("/watch#", "/watch?")
				.ToString();
			Query query = new Query(url);
			string value = string.Empty;
			if (!query.TryGetValue("v", out value))
			{
				return false;
			}
			normalized = "https://youtube.com/watch?v=" + value;
			return true;
		}

		private IEnumerator ParseVideos(string source, Action<List<YoutubeVideo>> resultCallback, Action<string> errorCallback = null)
		{
			List<YoutubeVideo> videos = new List<YoutubeVideo>();
			string title = string.Empty;
			string jsPlayer = string.Empty;
			try
			{
				title = Regex.Unescape(Json.GetKey("title", source));
				jsPlayer = ParseJsPlayer(source);
				if (string.IsNullOrEmpty(jsPlayer) && resultCallback != null)
				{
					resultCallback(videos);
					yield break;
				}
				string map = Json.GetKey("url_encoded_fmt_stream_map", source);
				IEnumerable<UnscrambledQuery> queries = from query in map.Split(',')
					select Unscramble(query);
				foreach (UnscrambledQuery query3 in queries)
				{
					videos.Add(new YoutubeVideo(title, query3, jsPlayer));
				}
			}
			catch (Exception ex)
			{
				Exception error3 = ex;
				errorCallback?.Invoke(error3.ToString());
			}
			string adaptiveMap = Json.GetKey("adaptive_fmts", source);
			if (adaptiveMap == string.Empty)
			{
				string temp2 = Json.GetKey("dashmpd", source);
				temp2 = HttpUtility.UrlDecode(temp2).Replace("\\/", "/");
				string requestText2 = string.Empty;
				WWW request = new WWW(temp2);
				yield return request;
				try
				{
					if (!string.IsNullOrEmpty(request.error))
					{
						throw new Exception($"[YouTubeService.ParseVideos] dashmpd request is failed: {request.error}");
					}
					requestText2 = request.text;
					string manifest = requestText2.Replace("\\/", "/");
					IEnumerable<string> uris = HttpUtility.GetUrisFromManifest(manifest);
					foreach (string v in uris)
					{
						videos.Add(new YoutubeVideo(title, UnscrambleManifestUrl(v), jsPlayer));
					}
				}
				catch (Exception error2)
				{
					errorCallback?.Invoke(error2.ToString());
				}
			}
			else
			{
				try
				{
					IEnumerable<UnscrambledQuery> queries2 = from query in adaptiveMap.Split(',')
						select Unscramble(query);
					foreach (UnscrambledQuery query2 in queries2)
					{
						videos.Add(new YoutubeVideo(title, query2, jsPlayer));
					}
				}
				catch (Exception ex2)
				{
					Exception error = ex2;
					errorCallback?.Invoke(error.ToString());
				}
			}
			resultCallback?.Invoke(videos);
		}

		private string ParseJsPlayer(string source)
		{
			string text = Json.GetKey("js", source).Replace("\\/", "/");
			if (string.IsNullOrEmpty(text) || text.Trim().Length == 0)
			{
				return string.Empty;
			}
			if (text.StartsWith("/yts"))
			{
				return $"https://www.youtube.com{text}";
			}
			if (!text.StartsWith("http"))
			{
				text = $"https:{text}";
			}
			return text;
		}

		private UnscrambledQuery Unscramble(string queryString)
		{
			queryString = queryString.Replace("\\u0026", "&");
			Query query = new Query(queryString);
			string text = query["url"];
			bool encrypted = false;
			string value = string.Empty;
			if (query.TryGetValue("s", out value))
			{
				encrypted = true;
				text += GetSignatureAndHost(value, query);
			}
			else if (query.TryGetValue("sig", out value))
			{
				text += GetSignatureAndHost(value, query);
			}
			text = HttpUtility.UrlDecode(HttpUtility.UrlDecode(text));
			Query query2 = new Query(text);
			if (!query2.ContainsKey("ratebypass"))
			{
				text += "&ratebypass=yes";
			}
			return new UnscrambledQuery(text, encrypted);
		}

		private string GetSignatureAndHost(string signature, Query query)
		{
			string text = "&signature=" + signature;
			string value = string.Empty;
			if (query.TryGetValue("fallback_host", out value))
			{
				text = text + "&fallback_host=" + value;
			}
			return text;
		}

		private UnscrambledQuery UnscrambleManifestUrl(string manifestUri)
		{
			int num = manifestUri.IndexOf("videoplayback") + "videoplayback".Length;
			string value = manifestUri.Substring(0, num);
			string text = manifestUri.Substring(num, manifestUri.Length - num);
			string[] array = text.Split(new char[1]
			{
				'/'
			}, StringSplitOptions.RemoveEmptyEntries);
			StringBuilder stringBuilder = new StringBuilder(value);
			stringBuilder.Append("?");
			for (int i = 0; i < array.Length; i += 2)
			{
				stringBuilder.Append(array[i]);
				stringBuilder.Append('=');
				stringBuilder.Append(array[i + 1].Replace("%2F", "/"));
				if (i < array.Length - 2)
				{
					stringBuilder.Append('&');
				}
			}
			return new UnscrambledQuery(stringBuilder.ToString(), encrypted: false);
		}
	}
}
