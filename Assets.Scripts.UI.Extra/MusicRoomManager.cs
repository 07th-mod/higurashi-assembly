using Assets.Scripts.Core;
using Assets.Scripts.Core.AssetManagement;
using Assets.Scripts.Core.Audio;
using Assets.Scripts.Core.Buriko;
using Assets.Scripts.Core.State;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Extra
{
	internal class MusicRoomManager : MonoBehaviour
	{
		public CanvasGroup CanvasGroup;

		public bool IsActive;

		public GameObject ButtonPrefab;

		public GameObject MusicListPanel;

		public GameObject PlayButton;

		public GameObject StopButton;

		public TextMeshProUGUI ModeButtonText;

		public List<Button> ButtonList;

		public TextMeshProUGUI TitleHeading;

		public TextMeshProUGUI TitleText;

		public TextMeshProUGUI Heading1Title;

		public TextMeshProUGUI Heading2Title;

		public TextMeshProUGUI Heading1Text;

		public TextMeshProUGUI Heading2Text;

		public TextMeshProUGUI TrackTimeText;

		public Slider VolumeSlider;

		public float MusicRoomVolume = 0.8f;

		private static List<MusicRoomEntry> Entries;

		private static int currentTrack = 2;

		private bool isEnglish;

		private static bool isPlaying = false;

		private static bool playMovie = false;

		private static bool resumePlaylist = false;

		private static bool shuffleOn = false;

		private static List<int> recentTracks = new List<int>();

		private static List<int> shuffleList;

		private static int shufflePosition;

		private static PlayMode playMode = PlayMode.Repeat;

		public void Awake()
		{
			CanvasGroup.alpha = 0f;
			VolumeSlider.value = MusicRoomVolume;
		}

		public void Show()
		{
			isEnglish = GameSystem.Instance.UseEnglishText;
			LoadMusicData();
			ClearTrackInfo();
			UpdatePlayModeButtonText();
			BuildShuffleList();
			isPlaying = false;
			GameSystem.Instance.RegisterAction(delegate
			{
				LTDescr lTDescr = LeanTween.value(base.gameObject, SetFade, 0f, 1f, 0.8f);
				lTDescr.onComplete = (Action)Delegate.Combine(lTDescr.onComplete, (Action)delegate
				{
					FinishShow();
				});
			});
			GameSystem.Instance.ExecuteActions();
		}

		private void BuildShuffleList()
		{
			if (shuffleList == null)
			{
				shuffleList = Enumerable.Range(1, Entries.Count - 1).ToList();
				shuffleList.Shuffle();
				shufflePosition = 0;
			}
		}

		public void LoadMusicData()
		{
			try
			{
				if (Entries == null)
				{
					Entries = JsonConvert.DeserializeObject<List<MusicRoomEntry>>(AssetManager.Instance.LoadTextDataString("musicroom.txt"));
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				UnityEngine.Object.Destroy(ButtonPrefab);
				return;
			}
			string str = isEnglish ? "..." : "～";
			for (int i = 0; i < Entries.Count; i++)
			{
				MusicRoomEntry musicRoomEntry = Entries[i];
				GameObject gameObject = UnityEngine.Object.Instantiate(ButtonPrefab);
				gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 260f);
				TextMeshProUGUI component = gameObject.GetComponent<TextMeshProUGUI>();
				string text = isEnglish ? musicRoomEntry.Name.En : musicRoomEntry.Name.Jp;
				if (musicRoomEntry.ShortName != null)
				{
					text = (isEnglish ? musicRoomEntry.ShortName.En : musicRoomEntry.ShortName.Jp);
				}
				if (text.Length <= 0)
				{
					continue;
				}
				gameObject.transform.SetParent(MusicListPanel.transform, worldPositionStays: false);
				component.text = text;
				TMP_TextInfo textInfo = component.GetTextInfo(text);
				float x = textInfo.characterInfo[0].topLeft.x;
				if (textInfo.characterInfo[textInfo.characterCount - 1].topRight.x - x > 260f)
				{
					int num = 0;
					for (num = 0; num < textInfo.characterCount - 1 && !(textInfo.characterInfo[num + 1].topRight.x - x > 250f); num++)
					{
					}
					string text4 = component.text = (component.text = text.Substring(0, num) + str);
				}
				Button component2 = gameObject.GetComponent<Button>();
				int targetTrack = i;
				component2.onClick.AddListener(delegate
				{
					ClickTrackButton(targetTrack);
				});
				ButtonList.Add(component2);
			}
			UnityEngine.Object.Destroy(ButtonPrefab);
		}

		public void ClearTrackInfo()
		{
			TitleText.text = "";
			TitleHeading.text = "";
			Heading1Text.text = "";
			Heading2Text.text = "";
			Heading1Title.text = "";
			Heading2Title.text = "";
		}

		private void UpdatePlayModeButtonText()
		{
			if (playMode == PlayMode.Repeat)
			{
				ModeButtonText.text = (isEnglish ? "Repeat" : "1曲リピ\u30fcト");
			}
			if (playMode == PlayMode.Shuffle)
			{
				ModeButtonText.text = (isEnglish ? "Shuffle" : "気まぐれ演奏");
			}
			if (playMode == PlayMode.Continuous)
			{
				ModeButtonText.text = (isEnglish ? "Play All" : "全曲演奏");
			}
		}

		public void ChangePlayMode()
		{
			int num = (int)playMode;
			num++;
			if (num >= Enum.GetNames(typeof(PlayMode)).Length)
			{
				num = 0;
			}
			playMode = (PlayMode)num;
			if (Application.isEditor)
			{
				Debug.Log("ChangePlayMode: " + playMode);
			}
			switch (playMode)
			{
			case PlayMode.Repeat:
				GameSystem.Instance.AudioController.EnableBgmLoop(2);
				break;
			case PlayMode.Shuffle:
				GameSystem.Instance.AudioController.DisableBgmLoop(2);
				break;
			case PlayMode.Continuous:
				GameSystem.Instance.AudioController.DisableBgmLoop(2);
				break;
			}
			shuffleOn = (playMode == PlayMode.Shuffle);
			UpdatePlayModeButtonText();
		}

		private void PlayNextShuffleTrack()
		{
			GameSystem.Instance.RegisterAction(delegate
			{
				int trackId = shuffleList[shufflePosition];
				ClickTrackButton(trackId);
				shufflePosition++;
				if (shufflePosition >= shuffleList.Count)
				{
					shuffleList = null;
					BuildShuffleList();
				}
			});
			GameSystem.Instance.ExecuteActions();
		}

		private void PlayNextSequenceTrack()
		{
			GameSystem.Instance.RegisterAction(delegate
			{
				currentTrack++;
				if (currentTrack == Entries.Count)
				{
					currentTrack = 1;
				}
				ClickTrackButton(currentTrack);
			});
			GameSystem.Instance.ExecuteActions();
		}

		private void TrackFinishCallback()
		{
			if (isPlaying)
			{
				if (shuffleOn)
				{
					PlayNextShuffleTrack();
				}
				if (playMode == PlayMode.Continuous)
				{
					PlayNextSequenceTrack();
				}
			}
		}

		public void ChangeVolumeSlider(float value)
		{
			MusicRoomVolume = value;
			GameSystem.Instance.AudioController.ChangeVolumeOfBGM(2, value, 0f);
		}

		private string LanguageString(TwoLanguageString str)
		{
			if (!isEnglish)
			{
				return str.Jp;
			}
			return str.En;
		}

		public void ClickTrackButton(int trackId)
		{
			if (!IsActive)
			{
				return;
			}
			foreach (Button button in ButtonList)
			{
				button.interactable = true;
			}
			MusicRoomEntry musicRoomEntry = Entries[trackId];
			if (musicRoomEntry.JumpScript != null)
			{
				Hide(delegate
				{
					IGameState stateObject = GameSystem.Instance.GetStateObject();
					if (stateObject != null && stateObject.GetStateType() == GameState.MusicRoom)
					{
						playMovie = true;
						isPlaying = false;
						BurikoScriptSystem.Instance.CallBlock("MusicRoomMovie");
						BurikoMemory.Instance.SetFlag("OmakeState", 3);
						stateObject.RequestLeaveImmediate();
					}
				});
				return;
			}
			ClearTrackInfo();
			TitleHeading.text = (isEnglish ? "[Title]" : "【Title】");
			if (musicRoomEntry.Details1Heading != null)
			{
				Heading1Title.text = (isEnglish ? ("[" + musicRoomEntry.Details1Heading.En + "]") : ("【" + musicRoomEntry.Details1Heading.Jp + "】"));
				Heading1Text.text = LanguageString(musicRoomEntry.Details1);
			}
			if (musicRoomEntry.Details2Heading != null)
			{
				Heading2Title.text = (isEnglish ? ("[" + musicRoomEntry.Details2Heading.En + "]") : ("【" + musicRoomEntry.Details2Heading.Jp + "】"));
				Heading2Text.text = LanguageString(musicRoomEntry.Details2);
			}
			TitleText.text = LanguageString(musicRoomEntry.Name);
			StopButton.SetActive(value: true);
			PlayButton.SetActive(value: false);
			if (Application.isEditor)
			{
				Debug.Log("Play " + musicRoomEntry.Filename);
			}
			GameSystem.Instance.AudioController.PlayAudio(musicRoomEntry.Filename, Assets.Scripts.Core.Audio.AudioType.BGM, 2, MusicRoomVolume);
			GameSystem.Instance.AudioController.AddBgmFinishCallback(2, TrackFinishCallback);
			if (playMode != 0)
			{
				GameSystem.Instance.AudioController.DisableBgmLoop(2);
			}
			recentTracks.Add(trackId);
			if (recentTracks.Count > 10)
			{
				recentTracks.RemoveAt(0);
			}
			Debug.Log(string.Join(", ", recentTracks));
			currentTrack = trackId;
			ButtonList[currentTrack].interactable = false;
			isPlaying = true;
		}

		public void StopTrack()
		{
			isPlaying = false;
			GameSystem.Instance.AudioController.StopBGM(2);
			StopButton.SetActive(value: false);
			PlayButton.SetActive(value: true);
			if (shuffleOn)
			{
				ClearTrackInfo();
			}
			foreach (Button button in ButtonList)
			{
				button.interactable = true;
			}
		}

		public void PlayTrack()
		{
			if (shuffleOn)
			{
				PlayNextShuffleTrack();
			}
			else
			{
				ClickTrackButton(currentTrack);
			}
		}

		public void SetFade(float fade)
		{
			CanvasGroup.alpha = fade;
		}

		public void FinishShow()
		{
			IsActive = true;
		}

		public void Hide(Action onFinish)
		{
			LTDescr lTDescr = LeanTween.value(base.gameObject, SetFade, 1f, 0f, 0.6f);
			lTDescr.onComplete = onFinish;
			lTDescr.onComplete = (Action)Delegate.Combine(lTDescr.onComplete, (Action)delegate
			{
				if (Application.isEditor)
				{
					Debug.Log("Finish!");
				}
				UnityEngine.Object.Destroy(base.gameObject);
			});
			GameSystem.Instance.SceneController.DrawScene("black", 0.4f);
			if (!playMovie)
			{
				BurikoMemory.Instance.SetFlag("OmakeState", 0);
			}
			GameSystem.Instance.ExecuteActions();
			GameSystem.Instance.AudioController.FadeOutBGM(2, 600, waitForFade: false);
		}

		public void CloseButtonPress()
		{
			IGameState stateObject = GameSystem.Instance.GetStateObject();
			if (stateObject != null && stateObject.GetStateType() == GameState.MusicRoom)
			{
				playMovie = false;
				GameSystem.Instance.AudioController.PlaySystemSound("wa_038.ogg");
				stateObject.RequestLeave();
			}
		}

		private void Update()
		{
			base.transform.localScale = GameSystem.Instance.MainUIController.transform.localScale;
			if (!isPlaying)
			{
				TrackTimeText.gameObject.SetActive(value: false);
				return;
			}
			TrackTimeText.gameObject.SetActive(value: true);
			TrackTimeText.text = GameSystem.Instance.AudioController.GetBGMPlayTimeAsString(2);
		}
	}
}
