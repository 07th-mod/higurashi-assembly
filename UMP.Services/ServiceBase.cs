using System;
using System.Collections;
using System.Collections.Generic;

namespace UMP.Services
{
	public abstract class ServiceBase
	{
		public abstract bool ValidUrl(string url);

		public abstract IEnumerator GetAllVideos(string url, Action<List<Video>> resultCallback, Action<string> errorCallback = null);
	}
}
