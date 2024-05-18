using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UMP
{
	public class PlayerManagerEvents
	{
		internal class PlayerEvent
		{
			private PlayerState _state;

			private object _arg;

			public PlayerState State => _state;

			public object Arg
			{
				get
				{
					return _arg;
				}
				set
				{
					_arg = value;
				}
			}

			public float GetFloatArg => (_arg == null || !(_arg is float)) ? 0f : ((float)_arg);

			public long GetLongArg => (_arg == null || !(_arg is long)) ? 0 : ((long)_arg);

			public string GetStringArg => (_arg == null || !(_arg is string)) ? string.Empty : ((string)_arg);

			public PlayerEvent(PlayerState state, object arg)
			{
				_state = state;
				_arg = arg;
			}
		}

		private MonoBehaviour _monoObject;

		private IPlayer _player;

		private Queue<PlayerEvent> _playerEvents;

		private IEnumerator _eventListenerEnum;

		private PlayerState _replaceState;

		private PlayerEvent _replaceEvent;

		private PlayerEvent Event => new PlayerEvent(_player.State, _player.StateValue);

		private event Action _playerOpeningListener;

		public event Action PlayerOpeningListener
		{
			add
			{
				this._playerOpeningListener = (Action)Delegate.Combine(this._playerOpeningListener, value);
			}
			remove
			{
				if (this._playerOpeningListener != null)
				{
					this._playerOpeningListener = (Action)Delegate.Remove(this._playerOpeningListener, value);
				}
			}
		}

		private event Action<float> _playerBufferingListener;

		public event Action<float> PlayerBufferingListener
		{
			add
			{
				this._playerBufferingListener = (Action<float>)Delegate.Combine(this._playerBufferingListener, value);
			}
			remove
			{
				if (this._playerBufferingListener != null)
				{
					this._playerBufferingListener = (Action<float>)Delegate.Remove(this._playerBufferingListener, value);
				}
			}
		}

		private event Action<Texture2D> _playerImageReadyListener;

		public event Action<Texture2D> PlayerImageReadyListener
		{
			add
			{
				this._playerImageReadyListener = (Action<Texture2D>)Delegate.Combine(this._playerImageReadyListener, value);
			}
			remove
			{
				if (this._playerImageReadyListener != null)
				{
					this._playerImageReadyListener = (Action<Texture2D>)Delegate.Remove(this._playerImageReadyListener, value);
				}
			}
		}

		private event Action<int, int> _playerPreparedListener;

		public event Action<int, int> PlayerPreparedListener
		{
			add
			{
				this._playerPreparedListener = (Action<int, int>)Delegate.Combine(this._playerPreparedListener, value);
			}
			remove
			{
				if (this._playerPreparedListener != null)
				{
					this._playerPreparedListener = (Action<int, int>)Delegate.Remove(this._playerPreparedListener, value);
				}
			}
		}

		private event Action _playerPlayingListener;

		public event Action PlayerPlayingListener
		{
			add
			{
				this._playerPlayingListener = (Action)Delegate.Combine(this._playerPlayingListener, value);
			}
			remove
			{
				if (this._playerPlayingListener != null)
				{
					this._playerPlayingListener = (Action)Delegate.Remove(this._playerPlayingListener, value);
				}
			}
		}

		private event Action _playerPausedListener;

		public event Action PlayerPausedListener
		{
			add
			{
				this._playerPausedListener = (Action)Delegate.Combine(this._playerPausedListener, value);
			}
			remove
			{
				if (this._playerPausedListener != null)
				{
					this._playerPausedListener = (Action)Delegate.Remove(this._playerPausedListener, value);
				}
			}
		}

		private event Action _playerStoppedListener;

		public event Action PlayerStoppedListener
		{
			add
			{
				this._playerStoppedListener = (Action)Delegate.Combine(this._playerStoppedListener, value);
			}
			remove
			{
				if (this._playerStoppedListener != null)
				{
					this._playerStoppedListener = (Action)Delegate.Remove(this._playerStoppedListener, value);
				}
			}
		}

		private event Action _playerEndReachedListener;

		public event Action PlayerEndReachedListener
		{
			add
			{
				this._playerEndReachedListener = (Action)Delegate.Combine(this._playerEndReachedListener, value);
			}
			remove
			{
				if (this._playerEndReachedListener != null)
				{
					this._playerEndReachedListener = (Action)Delegate.Remove(this._playerEndReachedListener, value);
				}
			}
		}

		private event Action _playerEncounteredErrorListener;

		public event Action PlayerEncounteredErrorListener
		{
			add
			{
				this._playerEncounteredErrorListener = (Action)Delegate.Combine(this._playerEncounteredErrorListener, value);
			}
			remove
			{
				if (this._playerEncounteredErrorListener != null)
				{
					this._playerEncounteredErrorListener = (Action)Delegate.Remove(this._playerEncounteredErrorListener, value);
				}
			}
		}

		private event Action<long> _playerTimeChangedListener;

		public event Action<long> PlayerTimeChangedListener
		{
			add
			{
				this._playerTimeChangedListener = (Action<long>)Delegate.Combine(this._playerTimeChangedListener, value);
			}
			remove
			{
				if (this._playerTimeChangedListener != null)
				{
					this._playerTimeChangedListener = (Action<long>)Delegate.Remove(this._playerTimeChangedListener, value);
				}
			}
		}

		private event Action<float> _playerPositionChangedListener;

		public event Action<float> PlayerPositionChangedListener
		{
			add
			{
				this._playerPositionChangedListener = (Action<float>)Delegate.Combine(this._playerPositionChangedListener, value);
			}
			remove
			{
				if (this._playerPositionChangedListener != null)
				{
					this._playerPositionChangedListener = (Action<float>)Delegate.Remove(this._playerPositionChangedListener, value);
				}
			}
		}

		private event Action<string> _playerSnapshotTakenListener;

		public event Action<string> PlayerSnapshotTakenListener
		{
			add
			{
				this._playerSnapshotTakenListener = (Action<string>)Delegate.Combine(this._playerSnapshotTakenListener, value);
			}
			remove
			{
				if (this._playerSnapshotTakenListener != null)
				{
					this._playerSnapshotTakenListener = (Action<string>)Delegate.Remove(this._playerSnapshotTakenListener, value);
				}
			}
		}

		internal PlayerManagerEvents(MonoBehaviour monoObject, IPlayer player)
		{
			_monoObject = monoObject;
			_player = player;
			_playerEvents = new Queue<PlayerEvent>();
		}

		private IEnumerator EventManager()
		{
			while (true)
			{
				PlayerEvent currentEvent = Event;
				if (currentEvent != null && currentEvent.State != 0)
				{
					_playerEvents.Enqueue(currentEvent);
				}
				if (_playerEvents.Count <= 0)
				{
					yield return null;
				}
				else
				{
					CallEvent();
				}
			}
		}

		private void CallEvent()
		{
			PlayerEvent playerEvent = _playerEvents.Dequeue();
			if (_replaceState == playerEvent.State)
			{
				_replaceState = PlayerState.Empty;
				playerEvent = _replaceEvent;
			}
			switch (playerEvent.State)
			{
			case PlayerState.Opening:
				if (this._playerOpeningListener != null)
				{
					this._playerOpeningListener();
				}
				break;
			case PlayerState.Buffering:
				if (this._playerBufferingListener != null)
				{
					this._playerBufferingListener(playerEvent.GetFloatArg);
				}
				break;
			case PlayerState.ImageReady:
				if (this._playerImageReadyListener != null)
				{
					this._playerImageReadyListener((Texture2D)playerEvent.Arg);
				}
				break;
			case PlayerState.Prepared:
				if (this._playerPreparedListener != null)
				{
					Vector2 vector = (Vector2)playerEvent.Arg;
					this._playerPreparedListener((int)vector.x, (int)vector.y);
				}
				break;
			case PlayerState.Playing:
				if (this._playerPlayingListener != null)
				{
					this._playerPlayingListener();
				}
				break;
			case PlayerState.Paused:
				if (this._playerPausedListener != null)
				{
					this._playerPausedListener();
				}
				break;
			case PlayerState.Stopped:
				if (this._playerStoppedListener != null)
				{
					this._playerStoppedListener();
				}
				break;
			case PlayerState.EndReached:
				if (this._playerEndReachedListener != null)
				{
					this._playerEndReachedListener();
				}
				break;
			case PlayerState.EncounteredError:
				if (this._playerEncounteredErrorListener != null)
				{
					this._playerEncounteredErrorListener();
				}
				break;
			case PlayerState.TimeChanged:
				if (this._playerTimeChangedListener != null && _player.IsReady)
				{
					this._playerTimeChangedListener(playerEvent.GetLongArg);
				}
				break;
			case PlayerState.PositionChanged:
				if (this._playerPositionChangedListener != null && _player.IsReady)
				{
					this._playerPositionChangedListener(playerEvent.GetFloatArg);
				}
				break;
			case PlayerState.SnapshotTaken:
				if (this._playerSnapshotTakenListener != null)
				{
					this._playerSnapshotTakenListener(playerEvent.GetStringArg);
				}
				break;
			}
		}

		private bool IsNativeEvents(object events)
		{
			return events is MediaPlayerStandalone || events is MediaPlayerWebGL;
		}

		internal void SetEvent(PlayerState state)
		{
			_playerEvents.Enqueue(new PlayerEvent(state, null));
		}

		internal void SetEvent(PlayerState state, object arg)
		{
			_playerEvents.Enqueue(new PlayerEvent(state, arg));
		}

		internal void ReplaceEvent(PlayerState replaceState, PlayerState newState, object arg)
		{
			_replaceState = replaceState;
			_replaceEvent = new PlayerEvent(newState, arg);
		}

		public void StartListener()
		{
			_playerEvents.Clear();
			if (_eventListenerEnum != null)
			{
				_monoObject.StopCoroutine(_eventListenerEnum);
			}
			_eventListenerEnum = EventManager();
			_monoObject.StartCoroutine(_eventListenerEnum);
		}

		public void StopListener()
		{
			if (_eventListenerEnum != null)
			{
				_monoObject.StopCoroutine(_eventListenerEnum);
			}
			if (!_monoObject.isActiveAndEnabled)
			{
				_playerEvents.Clear();
				return;
			}
			do
			{
				if (_playerEvents.Count > 0)
				{
					CallEvent();
				}
				PlayerEvent @event = Event;
				if (@event != null && @event.State != 0)
				{
					_playerEvents.Enqueue(@event);
				}
			}
			while (_playerEvents.Count > 0);
		}

		public void RemoveAllEvents()
		{
			if (this._playerOpeningListener != null)
			{
				Delegate[] invocationList = this._playerOpeningListener.GetInvocationList();
				for (int i = 0; i < invocationList.Length; i++)
				{
					Action action = (Action)invocationList[i];
					if (!IsNativeEvents(action.Target))
					{
						this._playerOpeningListener = (Action)Delegate.Remove(this._playerOpeningListener, action);
					}
				}
			}
			if (this._playerBufferingListener != null)
			{
				Delegate[] invocationList2 = this._playerBufferingListener.GetInvocationList();
				for (int j = 0; j < invocationList2.Length; j++)
				{
					Action<float> action2 = (Action<float>)invocationList2[j];
					if (!IsNativeEvents(action2.Target))
					{
						this._playerBufferingListener = (Action<float>)Delegate.Remove(this._playerBufferingListener, action2);
					}
				}
			}
			if (this._playerImageReadyListener != null)
			{
				Delegate[] invocationList3 = this._playerImageReadyListener.GetInvocationList();
				for (int k = 0; k < invocationList3.Length; k++)
				{
					Action<Texture2D> action3 = (Action<Texture2D>)invocationList3[k];
					if (!IsNativeEvents(action3.Target))
					{
						this._playerImageReadyListener = (Action<Texture2D>)Delegate.Remove(this._playerImageReadyListener, action3);
					}
				}
			}
			if (this._playerPreparedListener != null)
			{
				Delegate[] invocationList4 = this._playerPreparedListener.GetInvocationList();
				for (int l = 0; l < invocationList4.Length; l++)
				{
					Action<int, int> action4 = (Action<int, int>)invocationList4[l];
					if (!IsNativeEvents(action4.Target))
					{
						this._playerPreparedListener = (Action<int, int>)Delegate.Remove(this._playerPreparedListener, action4);
					}
				}
			}
			if (this._playerPlayingListener != null)
			{
				Delegate[] invocationList5 = this._playerPlayingListener.GetInvocationList();
				for (int m = 0; m < invocationList5.Length; m++)
				{
					Action action5 = (Action)invocationList5[m];
					if (!IsNativeEvents(action5.Target))
					{
						this._playerPlayingListener = (Action)Delegate.Remove(this._playerPlayingListener, action5);
					}
				}
			}
			if (this._playerPausedListener != null)
			{
				Delegate[] invocationList6 = this._playerPausedListener.GetInvocationList();
				for (int n = 0; n < invocationList6.Length; n++)
				{
					Action action6 = (Action)invocationList6[n];
					if (!IsNativeEvents(action6.Target))
					{
						this._playerPausedListener = (Action)Delegate.Remove(this._playerPausedListener, action6);
					}
				}
			}
			if (this._playerStoppedListener != null)
			{
				Delegate[] invocationList7 = this._playerStoppedListener.GetInvocationList();
				for (int num = 0; num < invocationList7.Length; num++)
				{
					Action action7 = (Action)invocationList7[num];
					if (!IsNativeEvents(action7.Target))
					{
						this._playerStoppedListener = (Action)Delegate.Remove(this._playerStoppedListener, action7);
					}
				}
			}
			if (this._playerEndReachedListener != null)
			{
				Delegate[] invocationList8 = this._playerEndReachedListener.GetInvocationList();
				for (int num2 = 0; num2 < invocationList8.Length; num2++)
				{
					Action action8 = (Action)invocationList8[num2];
					if (!IsNativeEvents(action8.Target))
					{
						this._playerEndReachedListener = (Action)Delegate.Remove(this._playerEndReachedListener, action8);
					}
				}
			}
			if (this._playerEncounteredErrorListener != null)
			{
				Delegate[] invocationList9 = this._playerEncounteredErrorListener.GetInvocationList();
				for (int num3 = 0; num3 < invocationList9.Length; num3++)
				{
					Action action9 = (Action)invocationList9[num3];
					if (!IsNativeEvents(action9.Target))
					{
						this._playerEncounteredErrorListener = (Action)Delegate.Remove(this._playerEncounteredErrorListener, action9);
					}
				}
			}
			if (this._playerTimeChangedListener != null)
			{
				Delegate[] invocationList10 = this._playerTimeChangedListener.GetInvocationList();
				for (int num4 = 0; num4 < invocationList10.Length; num4++)
				{
					Action<long> action10 = (Action<long>)invocationList10[num4];
					if (!IsNativeEvents(action10.Target))
					{
						this._playerTimeChangedListener = (Action<long>)Delegate.Remove(this._playerTimeChangedListener, action10);
					}
				}
			}
			if (this._playerPositionChangedListener != null)
			{
				Delegate[] invocationList11 = this._playerPositionChangedListener.GetInvocationList();
				for (int num5 = 0; num5 < invocationList11.Length; num5++)
				{
					Action<float> action11 = (Action<float>)invocationList11[num5];
					if (!IsNativeEvents(action11.Target))
					{
						this._playerPositionChangedListener = (Action<float>)Delegate.Remove(this._playerPositionChangedListener, action11);
					}
				}
			}
			if (this._playerSnapshotTakenListener == null)
			{
				return;
			}
			Delegate[] invocationList12 = this._playerSnapshotTakenListener.GetInvocationList();
			for (int num6 = 0; num6 < invocationList12.Length; num6++)
			{
				Action<string> action12 = (Action<string>)invocationList12[num6];
				if (!IsNativeEvents(action12.Target))
				{
					this._playerSnapshotTakenListener = (Action<string>)Delegate.Remove(this._playerSnapshotTakenListener, action12);
				}
			}
		}

		public void CopyPlayerEvents(PlayerManagerEvents events)
		{
			RemoveAllEvents();
			Delegate[] invocationList = events._playerOpeningListener.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				Action action = (Action)invocationList[i];
				if (!IsNativeEvents(action.Target))
				{
					PlayerOpeningListener += action;
				}
			}
			Delegate[] invocationList2 = events._playerBufferingListener.GetInvocationList();
			for (int j = 0; j < invocationList2.Length; j++)
			{
				Action<float> action2 = (Action<float>)invocationList2[j];
				if (!IsNativeEvents(action2.Target))
				{
					PlayerBufferingListener += action2;
				}
			}
			Delegate[] invocationList3 = events._playerImageReadyListener.GetInvocationList();
			for (int k = 0; k < invocationList3.Length; k++)
			{
				Action<Texture2D> action3 = (Action<Texture2D>)invocationList3[k];
				if (!IsNativeEvents(action3.Target))
				{
					PlayerImageReadyListener += action3;
				}
			}
			Delegate[] invocationList4 = events._playerPreparedListener.GetInvocationList();
			for (int l = 0; l < invocationList4.Length; l++)
			{
				Action<int, int> action4 = (Action<int, int>)invocationList4[l];
				if (!IsNativeEvents(action4.Target))
				{
					PlayerPreparedListener += action4;
				}
			}
			Delegate[] invocationList5 = events._playerPlayingListener.GetInvocationList();
			for (int m = 0; m < invocationList5.Length; m++)
			{
				Action action5 = (Action)invocationList5[m];
				if (!IsNativeEvents(action5.Target))
				{
					PlayerPlayingListener += action5;
				}
			}
			Delegate[] invocationList6 = events._playerPausedListener.GetInvocationList();
			for (int n = 0; n < invocationList6.Length; n++)
			{
				Action action6 = (Action)invocationList6[n];
				if (!IsNativeEvents(action6.Target))
				{
					PlayerPausedListener += action6;
				}
			}
			Delegate[] invocationList7 = events._playerStoppedListener.GetInvocationList();
			for (int num = 0; num < invocationList7.Length; num++)
			{
				Action action7 = (Action)invocationList7[num];
				if (!IsNativeEvents(action7.Target))
				{
					PlayerStoppedListener += action7;
				}
			}
			Delegate[] invocationList8 = events._playerEndReachedListener.GetInvocationList();
			for (int num2 = 0; num2 < invocationList8.Length; num2++)
			{
				Action action8 = (Action)invocationList8[num2];
				if (!IsNativeEvents(action8.Target))
				{
					PlayerEndReachedListener += action8;
				}
			}
			Delegate[] invocationList9 = events._playerEncounteredErrorListener.GetInvocationList();
			for (int num3 = 0; num3 < invocationList9.Length; num3++)
			{
				Action action9 = (Action)invocationList9[num3];
				if (!IsNativeEvents(action9.Target))
				{
					PlayerEncounteredErrorListener += action9;
				}
			}
			Delegate[] invocationList10 = events._playerTimeChangedListener.GetInvocationList();
			for (int num4 = 0; num4 < invocationList10.Length; num4++)
			{
				Action<long> action10 = (Action<long>)invocationList10[num4];
				if (!IsNativeEvents(action10.Target))
				{
					PlayerTimeChangedListener += action10;
				}
			}
			Delegate[] invocationList11 = events._playerPositionChangedListener.GetInvocationList();
			for (int num5 = 0; num5 < invocationList11.Length; num5++)
			{
				Action<float> action11 = (Action<float>)invocationList11[num5];
				if (!IsNativeEvents(action11.Target))
				{
					PlayerPositionChangedListener += action11;
				}
			}
			Delegate[] invocationList12 = events._playerSnapshotTakenListener.GetInvocationList();
			for (int num6 = 0; num6 < invocationList12.Length; num6++)
			{
				Action<string> action12 = (Action<string>)invocationList12[num6];
				if (!IsNativeEvents(action12.Target))
				{
					PlayerSnapshotTakenListener += action12;
				}
			}
		}
	}
}
