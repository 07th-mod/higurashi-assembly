using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UMP
{
	public class PlayerManagerLogs
	{
		public class PlayerLog
		{
			public string Message
			{
				get;
				private set;
			}

			public LogLevels Level
			{
				get;
				private set;
			}

			public PlayerLog(LogLevels level, string msg)
			{
				Message = msg;
				Level = level;
			}
		}

		private MonoBehaviour _monoObject;

		private MediaPlayerStandalone _player;

		private Queue<PlayerLog> _playerLogs;

		private IEnumerator _eventListenerEnum;

		private LogLevels _logDetail;

		private string _errorMessage = string.Empty;

		private PlayerLog Message
		{
			get
			{
				string logMessage = _player.LogMessage;
				if (!string.IsNullOrEmpty(logMessage))
				{
					int logLevel = _player.LogLevel;
					LogLevels level = LogLevels.Debug;
					switch (logLevel)
					{
					case 3:
						level = LogLevels.Error;
						break;
					case 4:
						level = LogLevels.Warning;
						break;
					}
					return new PlayerLog(level, logMessage);
				}
				return null;
			}
		}

		public LogLevels LogDetail
		{
			get
			{
				return _logDetail;
			}
			set
			{
				_logDetail = value;
			}
		}

		public string LastError => _errorMessage;

		private event Action<PlayerLog> _logMessageListener;

		public event Action<PlayerLog> LogMessageListener
		{
			add
			{
				this._logMessageListener = (Action<PlayerLog>)Delegate.Combine(this._logMessageListener, value);
			}
			remove
			{
				if (this._logMessageListener != null)
				{
					this._logMessageListener = (Action<PlayerLog>)Delegate.Remove(this._logMessageListener, value);
				}
			}
		}

		internal PlayerManagerLogs(MonoBehaviour monoObject, MediaPlayerStandalone player)
		{
			_monoObject = monoObject;
			_player = player;
			_playerLogs = new Queue<PlayerLog>();
		}

		private IEnumerator LogManager()
		{
			while (true)
			{
				PlayerLog currentMessage = Message;
				if (currentMessage != null)
				{
					_playerLogs.Enqueue(currentMessage);
				}
				if (_playerLogs.Count <= 0)
				{
					yield return null;
				}
				else
				{
					CallLog();
				}
			}
		}

		private void CallLog()
		{
			PlayerLog playerLog = _playerLogs.Dequeue();
			if (playerLog != null && playerLog.Level == _logDetail && this._logMessageListener != null)
			{
				this._logMessageListener(playerLog);
			}
		}

		internal void SetLog(LogLevels detail, string message)
		{
			_playerLogs.Enqueue(new PlayerLog(detail, message));
		}

		public void StartListener()
		{
			_playerLogs.Clear();
			if (_eventListenerEnum != null)
			{
				_monoObject.StopCoroutine(_eventListenerEnum);
			}
			_eventListenerEnum = LogManager();
			_monoObject.StartCoroutine(_eventListenerEnum);
		}

		public void StopListener()
		{
			if (_eventListenerEnum != null)
			{
				_monoObject.StopCoroutine(_eventListenerEnum);
			}
			while (_playerLogs.Count > 0)
			{
				CallLog();
			}
		}

		public void RemoveAllEvents()
		{
			if (this._logMessageListener != null)
			{
				Delegate[] invocationList = this._logMessageListener.GetInvocationList();
				for (int i = 0; i < invocationList.Length; i++)
				{
					Action<PlayerLog> value = (Action<PlayerLog>)invocationList[i];
					this._logMessageListener = (Action<PlayerLog>)Delegate.Remove(this._logMessageListener, value);
				}
			}
		}
	}
}
