using Services.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UMP.Services.Helpers;
using UnityEngine;

namespace UMP.Services.Youtube
{
	public class YoutubeVideo : Video
	{
		private class Decryptor
		{
			private enum FunctionType
			{
				Reverse,
				Slice,
				Swap
			}

			private static readonly Regex ParametersRegex = new Regex("\\(\\w+,(\\d+)\\)");

			private readonly Dictionary<string, FunctionType> _functionTypes = new Dictionary<string, FunctionType>();

			private readonly StringBuilder _stringBuilder = new StringBuilder();

			public bool IsComplete => _functionTypes.Count == Enum.GetValues(typeof(FunctionType)).Length;

			public void AddFunction(string js, string function)
			{
				string arg = Regex.Escape(function);
				FunctionType? functionType = null;
				if (Regex.IsMatch(js, $"{arg}:\\bfunction\\b\\(\\w+\\)"))
				{
					functionType = FunctionType.Reverse;
				}
				else if (Regex.IsMatch(js, $"{arg}:\\bfunction\\b\\([a],b\\).(\\breturn\\b)?.?\\w+\\."))
				{
					functionType = FunctionType.Slice;
				}
				else if (Regex.IsMatch(js, $"{arg}:\\bfunction\\b\\(\\w+\\,\\w\\).\\bvar\\b.\\bc=a\\b"))
				{
					functionType = FunctionType.Swap;
				}
				if (functionType.HasValue)
				{
					_functionTypes[function] = functionType.Value;
				}
			}

			public string ExecuteFunction(string signature, string line, string function)
			{
				FunctionType value = FunctionType.Reverse;
				if (!_functionTypes.TryGetValue(function, out value))
				{
					return signature;
				}
				switch (value)
				{
				case FunctionType.Reverse:
					return Reverse(signature);
				case FunctionType.Slice:
				case FunctionType.Swap:
				{
					int index = int.Parse(ParametersRegex.Match(line).Groups[1].Value, NumberStyles.AllowThousands, NumberFormatInfo.InvariantInfo);
					return (value != FunctionType.Slice) ? Swap(signature, index) : Slice(signature, index);
				}
				default:
					throw new ArgumentOutOfRangeException($"[YouTubeVideo.Decryptor] {value}");
				}
			}

			private string Reverse(string signature)
			{
				_stringBuilder.Remove(0, _stringBuilder.Length);
				for (int num = signature.Length - 1; num >= 0; num--)
				{
					_stringBuilder.Append(signature[num]);
				}
				return _stringBuilder.ToString();
			}

			private string Slice(string signature, int index)
			{
				return signature.Substring(index);
			}

			private string Swap(string signature, int index)
			{
				_stringBuilder.Remove(0, _stringBuilder.Length);
				_stringBuilder.Append(signature);
				_stringBuilder[0] = signature[index];
				_stringBuilder[index] = signature[0];
				return _stringBuilder.ToString();
			}
		}

		private static readonly Regex DefaultDecryptionFunctionRegex = new Regex("\\bc\\s*&&\\s*d\\.set\\([^,]+\\s*,\\s*\\([^)]*\\)\\s*\\(\\s*([a-zA-Z0-9$]+)\\(");

		private static readonly Regex FunctionRegex = new Regex("\\w+(?:.|\\[)(\\\"?\\w+(?:\\\")?)\\]?\\(");

		private string _uri;

		private bool _encrypted;

		private readonly string _title;

		private readonly string _jsPlayer;

		private readonly int _formatCode;

		public bool Is3D
		{
			get
			{
				switch (FormatCode)
				{
				case 82:
				case 83:
				case 84:
				case 85:
				case 100:
				case 101:
				case 102:
					return true;
				default:
					return false;
				}
			}
		}

		public bool IsAdaptive => AdaptiveType != AdaptiveFormat.None;

		public AdaptiveFormat AdaptiveType
		{
			get
			{
				switch (FormatCode)
				{
				case 133:
				case 134:
				case 135:
				case 136:
				case 137:
				case 138:
				case 160:
				case 242:
				case 243:
				case 244:
				case 247:
				case 248:
				case 264:
				case 271:
				case 272:
				case 278:
				case 313:
					return AdaptiveFormat.Video;
				case 139:
				case 140:
				case 141:
				case 171:
				case 172:
				case 249:
				case 250:
				case 251:
					return AdaptiveFormat.Audio;
				default:
					return AdaptiveFormat.None;
				}
			}
		}

		public int AudioBitrate
		{
			get
			{
				switch (FormatCode)
				{
				case 5:
				case 6:
				case 250:
					return 64;
				case 17:
					return 24;
				case 18:
				case 82:
				case 83:
					return 96;
				case 22:
				case 37:
				case 38:
				case 45:
				case 46:
				case 101:
				case 102:
				case 172:
					return 192;
				case 34:
				case 35:
				case 43:
				case 44:
				case 100:
				case 140:
				case 171:
					return 128;
				case 36:
					return 38;
				case 84:
				case 85:
					return 152;
				case 251:
					return 160;
				case 139:
				case 249:
					return 48;
				case 141:
					return 256;
				default:
					return -1;
				}
			}
		}

		public int Resolution
		{
			get
			{
				switch (FormatCode)
				{
				case 5:
				case 36:
				case 83:
				case 133:
				case 242:
					return 240;
				case 6:
					return 270;
				case 17:
				case 160:
				case 278:
					return 144;
				case 18:
				case 34:
				case 43:
				case 82:
				case 100:
				case 101:
				case 134:
				case 243:
					return 360;
				case 22:
				case 45:
				case 84:
				case 102:
				case 136:
				case 247:
					return 720;
				case 35:
				case 44:
				case 135:
				case 244:
					return 480;
				case 37:
				case 46:
				case 137:
				case 248:
					return 1080;
				case 38:
					return 3072;
				case 85:
					return 520;
				case 138:
				case 272:
				case 313:
					return 2160;
				case 264:
				case 271:
					return 1440;
				default:
					return -1;
				}
			}
		}

		public override VideoFormat VideoFormat
		{
			get
			{
				switch (FormatCode)
				{
				case 5:
				case 6:
				case 34:
				case 35:
					return VideoFormat.Flv;
				case 13:
				case 17:
				case 36:
					return VideoFormat.Mobile;
				case 18:
				case 22:
				case 37:
				case 38:
				case 82:
				case 83:
				case 84:
				case 85:
				case 133:
				case 134:
				case 135:
				case 136:
				case 137:
				case 138:
				case 139:
				case 140:
				case 141:
				case 160:
				case 264:
					return VideoFormat.Mp4;
				case 43:
				case 44:
				case 45:
				case 46:
				case 100:
				case 101:
				case 102:
				case 171:
				case 172:
				case 242:
				case 243:
				case 244:
				case 247:
				case 248:
				case 249:
				case 250:
				case 251:
				case 271:
				case 272:
				case 278:
				case 313:
					return VideoFormat.WebM;
				default:
					return VideoFormat.Unknown;
				}
			}
		}

		public override AudioFormat AudioFormat
		{
			get
			{
				switch (FormatCode)
				{
				case 5:
				case 6:
					return AudioFormat.Mp3;
				case 13:
				case 17:
				case 18:
				case 22:
				case 34:
				case 35:
				case 36:
				case 37:
				case 38:
				case 82:
				case 83:
				case 84:
				case 85:
				case 139:
				case 140:
				case 141:
					return AudioFormat.Aac;
				case 43:
				case 44:
				case 45:
				case 46:
				case 100:
				case 101:
				case 102:
				case 171:
				case 172:
					return AudioFormat.Vorbis;
				case 249:
				case 250:
				case 251:
					return AudioFormat.Opus;
				default:
					return AudioFormat.Unknown;
				}
			}
		}

		public override string Title => _title;

		public override string Url => _uri;

		public int FormatCode => _formatCode;

		public bool IsEncrypted => _encrypted;

		internal YoutubeVideo(string title, UnscrambledQuery query, string jsPlayer)
		{
			_title = title;
			_uri = query.Uri;
			_jsPlayer = jsPlayer;
			_encrypted = query.IsEncrypted;
			_formatCode = int.Parse(new Query(_uri)["itag"]);
		}

		public IEnumerator Decrypt(string decryptFunction = null, Action<string> errorCallback = null)
		{
			if (_encrypted)
			{
				Query query = new Query(_uri);
				string signature = string.Empty;
				if (query.TryGetValue("signature", out signature))
				{
					string requestText2 = string.Empty;
					WWW request = new WWW(_jsPlayer);
					yield return request;
					try
					{
						if (!string.IsNullOrEmpty(request.error))
						{
							throw new Exception($"[YouTubeVideo.Decrypt] jsPlayer request is failed: {request.error}");
						}
						requestText2 = request.text;
						query["signature"] = DecryptSignature(requestText2, signature, decryptFunction);
						_uri = query.ToString();
						_encrypted = false;
					}
					catch (Exception error)
					{
						errorCallback?.Invoke(error.ToString());
					}
				}
			}
		}

		private string DecryptSignature(string js, string signature, string decryptFunction)
		{
			string[] decryptionFunctionLines = GetDecryptionFunctionLines(js, decryptFunction);
			Decryptor decryptor = new Decryptor();
			string[] array = decryptionFunctionLines;
			foreach (string input in array)
			{
				if (decryptor.IsComplete)
				{
					break;
				}
				Match match = FunctionRegex.Match(input);
				if (match.Success)
				{
					decryptor.AddFunction(js, match.Groups[1].Value);
				}
			}
			string[] array2 = decryptionFunctionLines;
			foreach (string text in array2)
			{
				Match match2 = FunctionRegex.Match(text);
				if (match2.Success)
				{
					signature = decryptor.ExecuteFunction(signature, text, match2.Groups[1].Value);
				}
			}
			return signature;
		}

		private string[] GetDecryptionFunctionLines(string js, string decryptFunction)
		{
			string decryptionFunction = GetDecryptionFunction(js, decryptFunction);
			Match match = Regex.Match(js, $"(?!h\\.){Regex.Escape(decryptionFunction)}=function\\(\\w+\\)\\{{(.*?)\\}}", RegexOptions.Singleline);
			if (!match.Success)
			{
				throw new Exception("[YouTubeVideo.Decrypt] GetDecryptionFunctionLines failed");
			}
			return match.Groups[1].Value.Split(';');
		}

		private string GetDecryptionFunction(string js, string decryptFunction)
		{
			Regex regex = (!string.IsNullOrEmpty(decryptFunction)) ? new Regex(decryptFunction) : DefaultDecryptionFunctionRegex;
			Match match = regex.Match(js);
			if (!match.Success)
			{
				throw new Exception("[YouTubeVideo.Decrypt] GetDecryptionFunction failed");
			}
			return match.Groups[1].Value;
		}

		public override string ToString()
		{
			return $"{base.ToString()}, Resolution: {Resolution}, AudioBitrate: {AudioBitrate}, Is3D: {Is3D}";
		}
	}
}
