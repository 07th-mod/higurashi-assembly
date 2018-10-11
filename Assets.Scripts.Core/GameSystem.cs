using Assets.Scripts.Core.AssetManagement;
using Assets.Scripts.Core.Audio;
using Assets.Scripts.Core.Buriko;
using Assets.Scripts.Core.History;
using Assets.Scripts.Core.Interfaces;
using Assets.Scripts.Core.Scene;
using Assets.Scripts.Core.State;
using Assets.Scripts.Core.SteamWorks;
using Assets.Scripts.Core.TextWindow;
using Assets.Scripts.UI;
using Assets.Scripts.UI.CGGallery;
using Assets.Scripts.UI.Choice;
using Assets.Scripts.UI.Config;
using Assets.Scripts.UI.Prompt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Core
{
	public class GameSystem : MonoBehaviour
	{
		private const float maxProcTime = 0.01f;

		private static GameSystem _instance;

		public AssetManager AssetManager;

		public AudioController AudioController;

		public SceneController SceneController;

		public MainUIController MainUIController;

		public TextController TextController;

		public TextHistory TextHistory;

		public KeyHook KeyHook;

		public GameObject UIControl;

		public SteamController SteamController;

		public LoggingLevel ConsoleLoggingLevel = LoggingLevel.All;

		public LoggingLevel FileLoggingLevel = LoggingLevel.WarningsAndErrors;

		public ChoiceController ChoiceController;

		public IScriptInterpreter ScriptSystem;

		public string ScriptInterpreterName;

		public GameObject MenuPrefab;

		public GameObject ScenePrefab;

		public GameObject HistoryPrefab;

		public GameObject TitlePrefab;

		public GameObject PromptPrefab;

		public GameObject ConfigPrefab;

		public GameObject SaveLoadPrefab;

		public GameObject ChapterJumpPrefab;

		public GameObject GalleryPrefab;

		public GameObject ChapterScreenPrefab;

		public GameObject ExtraScreenPrefab;

		public GameObject TipsPrefab;

		public GameObject ChapterPreviewPrefab;

		private PreparedAction actions;

		public bool IsInitialized;

		public bool IsSkipping;

		public bool IsForceSkip;

		public bool IsAuto;

		public bool IsRunning;

		public bool ForceForceSkip;

		public bool CanSkip = true;

		public bool CanSave = true;

		public bool CanInput = true;

		public bool CanReturn;

		public bool UsePrompts = true;

		public bool SkipUnreadMessages;

		public bool SkipModeDelay = true;

		public bool ClickDuringAuto;

		public bool RightClickMenu = true;

		public bool StopVoiceOnClick;

		public bool UseSystemSounds = true;

		public bool UseEnglishText = true;

		public bool DisableUI;

		private float skipWait = 0.1f;

		private bool CanAdvance = true;

		public bool CanExit;

		private GameState gameState;

		public bool MessageBoxVisible;

		public float MessageWindowOpacity = 0.5f;

		private bool ReopenMessageBox;

		public readonly List<Wait> WaitList = new List<Wait>();

		private MenuUIController menuUIController;

		private HistoryWindow historyWindow;

		private PromptController promptController;

		private ConfigManager configManager;

		private GalleryManager galleryManager;

		private MGHelper.InputHandler inputHandler;

		private IGameState curStateObj;

		private float blockInputTime;

		private int SystemInit = 1;

		private Resolution fullscreenResolution;

		private int screenModeSet = -1;

		private Stack<StateEntry> stateStack = new Stack<StateEntry>();

		public bool HasFocus;

		public float AspectRatio;

		public bool IsFullscreen { get; private set; }

		public float ConfigMenuFontSize = 0;

		public static GameSystem Instance => _instance ?? (_instance = GameObject.Find("_GameSystem").GetComponent<GameSystem>());

		public GameState GameState
		{
			get
			{
				return gameState;
			}
			set
			{
				Debug.Log("Changing game state to " + value);
				gameState = value;
			}
		}

		private void Initialize()
		{
			Logger.Log("GameSystem: Starting GameSystem");
			IsInitialized = true;
			AssetManager = new AssetManager();
			AudioController = new AudioController();
			TextController = new TextController();
			TextHistory = new TextHistory();
			try
			{
				AssetManager.CompileIfNeeded();
				Logger.Log("GameSystem: Starting ScriptInterpreter");
				Type type = Type.GetType(ScriptInterpreterName);
				if (type == null)
				{
					throw new Exception("Cannot find class " + ScriptInterpreterName + " through reflection!");
				}
				ScriptSystem = (Activator.CreateInstance(type) as IScriptInterpreter);
				if (ScriptSystem == null)
				{
					throw new Exception("Failed to instantiate ScriptSystem!");
				}
				ScriptSystem.Initialize(this);
			}
			catch (Exception arg)
			{
				Logger.LogError($"Unable to load Script Interpreter of type {ScriptInterpreterName}!\r\n{arg}");
				throw;
				IL_00d0:;
			}
			IsRunning = true;
			GameState = GameState.Normal;
			curStateObj = new StateNormal();
			IGameState obj = curStateObj;
			inputHandler = obj.InputHandler;
			MessageBoxVisible = false;
			if (!PlayerPrefs.HasKey("width"))
			{
				PlayerPrefs.SetInt("width", 640);
			}
			if (!PlayerPrefs.HasKey("height"))
			{
				PlayerPrefs.SetInt("height", 480);
			}
			if (PlayerPrefs.GetInt("width") < 640)
			{
				PlayerPrefs.SetInt("width", 640);
			}
			if (PlayerPrefs.GetInt("height") < 480)
			{
				PlayerPrefs.SetInt("height", 480);
			}
			IsFullscreen = PlayerPrefs.GetInt("is_fullscreen", 0) == 1;
			fullscreenResolution.width = 0;
			fullscreenResolution.height = 0;
			fullscreenResolution = GetFullscreenResolution();

			if (IsFullscreen)
			{
				Screen.SetResolution(fullscreenResolution.width, fullscreenResolution.height, fullscreen: true);
			}
			else if (PlayerPrefs.HasKey("height") && PlayerPrefs.HasKey("width"))
			{
				int width = PlayerPrefs.GetInt("width");
				int height = PlayerPrefs.GetInt("height");
				Debug.Log("Requesting window size " + width + "x" + height + " based on config file");
				Screen.SetResolution(width, height, fullscreen: false);
			}
			if ((Screen.width < 640 || Screen.height < 480) && !IsFullscreen)
			{
				Screen.SetResolution(640, 480, fullscreen: false);
			}
			if (Application.platform == RuntimePlatform.WindowsPlayer)
			{
				KeyHook = new KeyHook();
			}

		}

		public void UpdateAspectRatio(float newratio)
		{
			AspectRatio = newratio;
			if (!IsFullscreen)
			{
				int width = Mathf.RoundToInt((float)Screen.height * AspectRatio);
				Screen.SetResolution(width, Screen.height, fullscreen: false);
			}
			PlayerPrefs.SetInt("width", Mathf.RoundToInt(PlayerPrefs.GetInt("height") * AspectRatio));
			MainUIController.UpdateBlackBars();
			SceneController.UpdateScreenSize();
		}

		public void CheckinSystem()
		{
			SystemInit--;
		}

		public void ForceReturnNormalState()
		{
			stateStack.Clear();
			curStateObj = new StateNormal();
			IGameState obj = curStateObj;
			inputHandler = obj.InputHandler;
			GameState = GameState.Normal;
		}

		public void PopStateStack()
		{
			if (stateStack.Count == 0)
			{
				Debug.Log("PopStateStack has no state to remove.");
				curStateObj = new StateNormal();
				IGameState obj = curStateObj;
				inputHandler = obj.InputHandler;
				GameState = GameState.Normal;
			}
			else
			{
				if (curStateObj != null)
				{
					Debug.Log("PopStateStack - Calling OnLeaveState");
					curStateObj.OnLeaveState();
					curStateObj = null;
				}
				StateEntry stateEntry = stateStack.Pop();
				inputHandler = stateEntry.InputHandler;
				GameState = stateEntry.State;
				curStateObj = stateEntry.StateObject;
				if (curStateObj != null)
				{
					curStateObj.OnRestoreState();
				}
				Debug.Log("StateStack now has " + stateStack.Count + " entries.");
			}
		}

		private void PushStateStack(MGHelper.InputHandler newHandler, GameState newState)
		{
			if (curStateObj != null)
			{
				stateStack.Push(new StateEntry(curStateObj, GameState));
			}
			else
			{
				stateStack.Push(new StateEntry(inputHandler, GameState));
			}
			inputHandler = newHandler;
			GameState = newState;
			curStateObj = null;
		}

		public void PushStateObject(IGameState stateObject)
		{
			if (curStateObj != null)
			{
				stateStack.Push(new StateEntry(curStateObj, GameState));
			}
			else
			{
				stateStack.Push(new StateEntry(inputHandler, GameState));
			}
			curStateObj = stateObject;
			IGameState obj = curStateObj;
			inputHandler = obj.InputHandler;
			GameState = curStateObj.GetStateType();
		}

		public IGameState GetStateObject()
		{
			return curStateObj;
		}

		public void ClearActions()
		{
			actions = null;
		}

		public void RegisterAction(PreparedAction action)
		{
			actions = (PreparedAction)Delegate.Combine(actions, action);
		}

		public void ExecuteActions()
		{
			StartCoroutine(ActionRunner());
			CanAdvance = false;
		}

		private IEnumerator ActionRunner()
		{
			Resources.UnloadUnusedAssets();
			yield return (object)null;
			yield return (object)null;
			if (actions != null)
			{
				actions();
			}
			actions = null;
			CanAdvance = true;
		}

		public void HideUIControls()
		{
			DisableUI = true;
			UIControl.SetActive(value: false);
		}

		public void ShowUIControls()
		{
			DisableUI = false;
			UIControl.SetActive(value: true);
		}

		public void CloseChoiceIfExists()
		{
			if (ChoiceController != null)
			{
				ChoiceController.Destroy();
				ChoiceController = null;
			}
		}

		public void LeaveChoices()
		{
			PopStateStack();
			AddWait(new Wait(0.5f, WaitTypes.WaitForTime, delegate
			{
				ChoiceController.Destroy();
				ChoiceController = null;
			}));
			ExecuteActions();
		}

		public void DisplayChoices(List<string> options, int count)
		{
			if (ChoiceController != null)
			{
				ChoiceController.Destroy();
				ChoiceController = null;
			}
			PushStateStack(ChoiceHandleInput, GameState.ChoiceScreen);
			ChoiceController = new ChoiceController();
			ChoiceController.Create(options, count);
		}

		private bool ChoiceHandleInput()
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				SwitchToViewMode();
				return false;
			}
			if (Input.GetMouseButtonDown(1))
			{
				if (IsSkipping && !IsForceSkip)
				{
					IsSkipping = false;
					return false;
				}
				if (IsAuto)
				{
					IsAuto = false;
					return false;
				}
			}
			if (Input.GetMouseButtonDown(1) && MessageBoxVisible)
			{
				SwitchToRightClickMenu();
				return false;
			}
			return false;
		}

		private bool HiddenMessageWindowHandleInput()
		{
			if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape))
			{
				PopStateStack();
				UpdateWaits();
				MessageBoxVisible = true;
				MainUIController.FadeIn(0.2f);
				SceneController.RevealFace(0.2f);
				ExecuteActions();
				blockInputTime = 0.25f;
			}
			return false;
		}

		public void LeaveMenu(MenuUIController.MenuCloseDelegate onClose, bool doPop)
		{
			menuUIController.LeaveMenu(delegate
			{
				if (doPop)
				{
					PopStateStack();
				}
				UpdateWaits();
				ExecuteActions();
				if (onClose != null)
				{
					onClose();
				}
			}, showMessage: false);
		}

		public void SwitchToHiddenWindow2()
		{
			MainUIController.FadeOut(0.3f, isBlocking: false);
			SceneController.HideFace(0.3f);
			ExecuteActions();
			PushStateStack(HiddenMessageWindowHandleInput, GameState.HiddenMessageWindow);
		}

		public void SwitchToHiddenWindow()
		{
			menuUIController.LeaveMenu(delegate
			{
				PopStateStack();
				UpdateWaits();
				ExecuteActions();
				PushStateStack(HiddenMessageWindowHandleInput, GameState.HiddenMessageWindow);
			}, showMessage: false);
		}

		public void LeaveGalleryScreen(GalleryManager.GalleryCloseCallback callback)
		{
			if (!(galleryManager == null))
			{
				inputHandler = null;
				galleryManager.Close(delegate
				{
					PopStateStack();
					ExecuteActions();
					galleryManager = null;
					if (callback != null)
					{
						callback();
					}
				});
			}
		}

		public void LeaveConfigScreen(ConfigManager.LeaveConfigDelegate callback)
		{
			inputHandler = null;
			configManager.Leave(delegate
			{
				PopStateStack();
				ExecuteActions();
				if (callback != null)
				{
					callback();
				}
			});
		}

		public void RevealMessageBox()
		{
			MessageBoxVisible = true;
			MainUIController.FadeIn(0.5f);
			SceneController.RevealFace(0.5f);
			ExecuteActions();
		}

		private bool MenuHandleInput()
		{
			if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
			{
				menuUIController.LeaveMenu(delegate
				{
					PopStateStack();
					UpdateWaits();
					MessageBoxVisible = true;
					MainUIController.ShowMessageBox();
					ExecuteActions();
				}, showMessage: true);
			}
			return false;
		}

		private void UpdateCarret()
		{
			if (IsSkipping || IsAuto || GameState != GameState.Normal)
			{
				MainUIController.HideCarret();
			}
			else if (WaitList.Exists((Wait a) => a.Type == WaitTypes.WaitForInput) && !WaitList.Exists((Wait a) => a.Type == WaitTypes.WaitForText) && !TextController.IsTyping() && MessageBoxVisible)
			{
				MainUIController.ShowCarret();
			}
			else
			{
				MainUIController.HideCarret();
			}
		}

		public void SwitchToConfig(int configtype, bool showMessageWindow)
		{
			ReopenMessageBox = showMessageWindow;
			RegisterAction(delegate
			{
				PushStateStack(ConfigHandleInput, GameState.ConfigScreen);
				GameObject gameObject = UnityEngine.Object.Instantiate(ConfigPrefab);
				configManager = gameObject.GetComponent<ConfigManager>();
				if (configManager == null)
				{
					throw new Exception("Failed to instantiate configManager!");
				}
				configManager.Open(configtype, showMessageWindow);
			});
			ExecuteActions();
		}

		public void SwitchToGallery()
		{
			if (galleryManager != null)
			{
				Debug.Log(galleryManager);
				PushStateStack(GalleryHandleInput, GameState.CGGallery);
				ExecuteActions();
			}
			else
			{
				RegisterAction(delegate
				{
					PushStateStack(GalleryHandleInput, GameState.CGGallery);
					GameObject gameObject = UnityEngine.Object.Instantiate(GalleryPrefab);
					galleryManager = gameObject.GetComponent<GalleryManager>();
					if (galleryManager == null)
					{
						throw new Exception("Failed to instantiate GalleryManager!");
					}
					galleryManager.Open();
				});
				ExecuteActions();
			}
		}

		public void SwitchToViewMode()
		{
			PushStateStack(HiddenMessageWindowHandleInput, GameState.HiddenMessageWindow);
			MainUIController.FadeOut(0.2f, isBlocking: false);
			SceneController.HideFace(0.2f);
			ExecuteActions();
			blockInputTime = 0.25f;
		}

		public void SwitchToHistoryScreen()
		{
			PushStateObject(new StateHistory());
		}

		public void SwitchToRightClickMenu()
		{
			PushStateStack(MenuHandleInput, GameState.RightClickMenu);
			GameObject gameObject = UnityEngine.Object.Instantiate(MenuPrefab);
			menuUIController = gameObject.GetComponent<MenuUIController>();
			if (menuUIController == null)
			{
				throw new Exception("Failed to instantiate MenuUIController!");
			}
			MainUIController.FadeOut(0.3f, isBlocking: false);
			SceneController.HideFace(0.3f);
			ExecuteActions();
		}

		private bool GalleryHandleInput()
		{
			if (Input.GetMouseButtonDown(1))
			{
				PopStateStack();
				BurikoMemory.Instance.SetFlag("LOCALWORK_NO_RESULT", 0);
				return false;
			}
			return false;
		}

		private bool ConfigHandleInput()
		{
			if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
			{
				LeaveConfigScreen(null);
			}
			return false;
		}

		public void ClearAllWaits()
		{
			WaitList.ForEach(delegate(Wait a)
			{
				a.Finish();
			});
			WaitList.RemoveAll((Wait a) => a.IsActive);
		}

		public bool HasWaitOfType(WaitTypes type)
		{
			return WaitList.Exists((Wait f) => f.Type == type);
		}

		public void ClearTextWait(bool finish)
		{
			List<Wait> list = (from a in WaitList
			where a.Type == WaitTypes.WaitForText
			select a).ToList();
			foreach (Wait item in list)
			{
				if (finish)
				{
					item.Finish();
				}
				WaitList.Remove(item);
			}
			TextController.FinishTyping();
		}

		public void ClearVoiceWaits()
		{
			WaitList.RemoveAll((Wait a) => a.Type == WaitTypes.WaitForVoice);
		}

		public void ClearInputWaits()
		{
			if (IsAuto)
			{
			}
		}

		public void ClearWait()
		{
			if (WaitList.Exists((Wait a) => a.Type == WaitTypes.WaitForText) && !IsSkipping)
			{
				ClearTextWait(finish: true);
			}
			else
			{
				WaitList.ForEach(delegate(Wait a)
				{
					a.Finish();
				});
				WaitList.RemoveAll((Wait a) => a.IsActive);
				if (StopVoiceOnClick)
				{
					AudioController.StopAllVoice();
				}
			}
		}

		public void AddWait(Wait newWait)
		{
			WaitList.Add(newWait);
		}

		private bool HasExistingWaits()
		{
			bool result = WaitList.Count != 0;
			WaitList.RemoveAll((Wait a) => !a.IsRunning());
			if (IsAuto)
			{
				WaitList.RemoveAll((Wait a) => a.Type == WaitTypes.WaitForInput);
			}
			return result;
		}

		public void UpdateWaits()
		{
			WaitList.ForEach(delegate(Wait a)
			{
				a.Update();
			});
		}

		public IEnumerator FrameWaitForFullscreen(int width, int height, bool fullscreen)
		{
			yield return (object)new WaitForEndOfFrame();
			yield return (object)new WaitForFixedUpdate();
			IsFullscreen = fullscreen;
			PlayerPrefs.SetInt("is_fullscreen", fullscreen ? 1 : 0);
			Screen.SetResolution(width, height, fullscreen);
			while (Screen.width != width || Screen.height != height)
			{
				yield return (object)null;
			}
		}

		public void GoFullscreen()
		{
			IsFullscreen = true;
			PlayerPrefs.SetInt("is_fullscreen", 1);
			Resolution resolution = GetFullscreenResolution();
			Screen.SetResolution(resolution.width, resolution.height, fullscreen: true);
			Debug.Log(resolution.width + " , " + resolution.height);
			PlayerPrefs.SetInt("fullscreen_width", resolution.width);
			PlayerPrefs.SetInt("fullscreen_height", resolution.height);
		}

		public void DeFullscreen(int width, int height)
		{
			IsFullscreen = false;
			PlayerPrefs.SetInt("is_fullscreen", 0);
			Screen.SetResolution(width, height, fullscreen: false);
		}

		private void OnApplicationFocus(bool focusStatus)
		{
			HasFocus = focusStatus;
		}

		private void LateUpdate()
		{
			if (screenModeSet == -1)
			{
				screenModeSet = 0;
				fullscreenResolution = Screen.currentResolution;
				if (PlayerPrefs.HasKey("fullscreen_width") && PlayerPrefs.HasKey("fullscreen_height") && Screen.fullScreen)
				{
					fullscreenResolution.width = PlayerPrefs.GetInt("fullscreen_width");
					fullscreenResolution.height = PlayerPrefs.GetInt("fullscreen_height");
				}
				Debug.Log("Fullscreen Resolution: " + fullscreenResolution.width + ", " + fullscreenResolution.height);
			}
		}

		private void Update()
		{
			if (SystemInit <= 0)
			{
				Logger.Update();
				if (!IsInitialized)
				{
					Initialize();
				}
				else
				{
					if (blockInputTime <= 0f)
					{
						if ((CanInput || GameState != GameState.Normal) && (inputHandler == null || !inputHandler()))
						{
							return;
						}
					}
					else
					{
						blockInputTime -= Time.deltaTime;
					}
					if (IsRunning && CanAdvance)
					{
						UpdateWaits();
						UpdateCarret();
						try
						{
							if (GameState == GameState.Normal)
							{
								TextController.Update();
								if (!IsSkipping)
								{
									goto IL_0112;
								}
								skipWait -= Time.deltaTime;
								if (!(skipWait <= 0f))
								{
									goto IL_0112;
								}
								if (!HasExistingWaits())
								{
									if (SkipModeDelay)
									{
										skipWait = 0.1f;
									}
									goto IL_0112;
								}
								ClearAllWaits();
							}
							goto end_IL_00a2;
							IL_0112:
							float num = Time.time + 0.01f;
							while (!HasExistingWaits() && !(Time.time > num) && !HasExistingWaits() && CanAdvance)
							{
								ScriptSystem.Advance();
							}
							end_IL_00a2:;
						}
						catch (Exception)
						{
							IsRunning = false;
							throw;
							IL_0178:;
						}
					}
				}
			}
		}

		private void OnDestroy()
		{
			if (Application.platform == RuntimePlatform.WindowsPlayer)
			{
				KeyHook.Unhook();
			}
		}

		private void OnApplicationQuit()
		{
			BurikoMemory.Instance.SaveGlobals();
			if (GameState != GameState.DialogPrompt && !CanExit)
			{
				Application.CancelQuit();
				if (GameState == GameState.ConfigScreen)
				{
					if (ReopenMessageBox)
					{
						RevealMessageBox();
					}
					RegisterAction(delegate
					{
						LeaveConfigScreen(delegate
						{
							PushStateObject(new StateDialogPrompt(PromptType.DialogExit, delegate
							{
								CanExit = true;
								Application.Quit();
							}, null));
						});
					});
					ExecuteActions();
				}
				else
				{
					RegisterAction(delegate
					{
						PushStateObject(new StateDialogPrompt(PromptType.DialogExit, delegate
						{
							CanExit = true;
							Application.Quit();
						}, null));
					});
					ExecuteActions();
				}
			}
			else
			{
				SteamController.Close();
			}
		}

		public Resolution GetFullscreenResolution()
		{
			Resolution resolution = new Resolution();
			// Try to guess resolution from Screen.currentResolution
			if (!Screen.fullScreen || Application.platform == RuntimePlatform.OSXPlayer)
			{
				resolution.width = this.fullscreenResolution.width = Screen.currentResolution.width;
				resolution.height = this.fullscreenResolution.height = Screen.currentResolution.height;
			}
			else if (this.fullscreenResolution.width > 0 && this.fullscreenResolution.height > 0)
			{
				resolution.width = this.fullscreenResolution.width;
				resolution.height = this.fullscreenResolution.height;
			}
			else
			{
				resolution.width = Screen.currentResolution.width;
				resolution.height = Screen.currentResolution.height;
			}

			// Above can be glitchy on Linux, so also check the maximum resolution of a single monitor
			// If it's bigger than that, then switch over
			// Note that this (from what I can tell) gives you the biggest resolution of any of your monitors,
			// not just the one the game is running under, so it could *also* be wrong, which is why we check both methods
			if (Screen.resolutions.Length > 0)
			{
				Resolution tmp = Screen.resolutions[Screen.resolutions.Length - 1];
				if (tmp.width <= resolution.width && tmp.height <= resolution.height) {
					resolution = tmp;
				}
			}

			if (PlayerPrefs.HasKey("fullscreen_width_override"))
			{
				resolution.width = PlayerPrefs.GetInt("fullscreen_width_override");
			}
			if (PlayerPrefs.HasKey("fullscreen_height_override"))
			{
				resolution.height = PlayerPrefs.GetInt("fullscreen_height_override");
			}
			Debug.Log("Using resolution " + resolution.width + "x" + resolution.height + " as the fullscreen resolution.");
			return resolution;
		}

		/// <summary>
		/// Gets the amount you should offset gui elements to center them properly based on the current aspect ratio.
		/// Add this number to GUI elements' positions to center them, subtract it from window positions.
		/// </summary>
		public float GetGUIOffset() {
			float differenceFrom43 = (4f / 3f) - AspectRatio;
			return differenceFrom43 * 384f;
		}

		~GameSystem()
		{
			// Fixes an issue where Unity would write garbage values to its saved state on Linux
			// If we do this while the game is running, Unity will overwrite the values
			// So do it in the finalizer, which will run as the game quits and the GameSystem is deallocated
			if (PlayerPrefs.HasKey("width") && PlayerPrefs.HasKey("height"))
			{
				int width = PlayerPrefs.GetInt("width");
				int height = PlayerPrefs.GetInt("height");
				PlayerPrefs.SetInt("Screenmanager Resolution Width", width);
				PlayerPrefs.SetInt("Screenmanager Resolution Height", height);
				PlayerPrefs.SetInt("is_fullscreen", IsFullscreen ? 1 : 0);
				PlayerPrefs.SetInt("Screenmanager Is Fullscreen mode", 0);
			}
		}
	}
}
