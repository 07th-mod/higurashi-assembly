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
using System.Threading;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Core
{
	public class GameSystem : MonoBehaviour
	{
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

		public GameObject LoadingBox;

		public TextMeshPro LoadingText;

		public TextMeshPro HistoryTextMesh;

		private PreparedAction actions;

		private PreparedAction delayedActions;

		public bool IsInitialized;

		public bool IsSkipping;

		public bool IsForceSkip;

		public bool IsAuto;

		public bool IsRunning;

		public bool ForceForceSkip;

		public bool CanSkip = true;

		public bool CanSave = true;

		public bool CanLoad = true;

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

		private bool WaitOnDelayedAction;

		private int coroutinecount;

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

		private const float maxProcTime = 0.01f;

		private float blockInputTime;

		private int SystemInit = 1;

		private Resolution fullscreenResolution;

		private int screenModeSet = -1;

		private Stack<StateEntry> stateStack = new Stack<StateEntry>();

		public bool HasFocus;

		public float AspectRatio;

		private Thread CompileThread;

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
			IsRunning = false;
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
			if ((Screen.width < 640 || Screen.height < 480) && !Screen.fullScreen)
			{
				Screen.SetResolution(640, 480, fullscreen: false);
			}
			Debug.Log("Starting compile thread...");
			CompileThread = new Thread(CompileScripts)
			{
				IsBackground = true
			};
			CompileThread.Start();
			if (Application.platform == RuntimePlatform.WindowsPlayer)
			{
				KeyHook = new KeyHook();
			}
		}

		public void CompileScripts()
		{
			try
			{
				Debug.Log("Compiling!");
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
			}
		}

		public void PostLoading()
		{
			LoadingBox.SetActive(value: false);
		}

		public void UpdateAspectRatio(float newratio)
		{
			AspectRatio = newratio;
			if (!Screen.fullScreen)
			{
				int width = Mathf.RoundToInt((float)Screen.height * AspectRatio);
				Screen.SetResolution(width, Screen.height, fullscreen: false);
			}
			if (!PlayerPrefs.HasKey("width"))
			{
				PlayerPrefs.SetInt("width", Mathf.RoundToInt((float)PlayerPrefs.GetInt("height") * AspectRatio));
			}
			MainUIController.UpdateBlackBars();
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

		public void RegisterDelayedAction(PreparedAction action)
		{
			delayedActions = (PreparedAction)Delegate.Combine(delayedActions, action);
		}

		public void ExecuteDelayedActions()
		{
			StartCoroutine(DelayedActionRunner());
			WaitOnDelayedAction = true;
		}

		private IEnumerator DelayedActionRunner()
		{
			yield return null;
			if (this.delayedActions != null)
			{
				this.delayedActions();
			}
			this.delayedActions = null;
			this.WaitOnDelayedAction = false;
			yield break;
		}

		public void RegisterAction(PreparedAction action)
		{
			actions = (PreparedAction)Delegate.Combine(actions, action);
		}

		public void ExecuteActions()
		{
			PreparedAction act = actions;
			actions = null;
			StartCoroutine(ActionRunner(act));
			CanAdvance = false;
			coroutinecount++;
		}

		private IEnumerator ActionRunner(PreparedAction act)
		{
			Resources.UnloadUnusedAssets();
			yield return null;
			yield return null;
			if (act != null)
			{
				act();
			}
			this.coroutinecount--;
			if (this.coroutinecount == 0)
			{
				this.CanAdvance = true;
			}
			yield break;
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
			yield return new WaitForEndOfFrame();
			yield return new WaitForFixedUpdate();
			Screen.SetResolution(width, height, fullscreen);
			while (Screen.width != width || Screen.height != height)
			{
				yield return null;
			}
			yield break;
		}

		public void GoFullscreen()
		{
			int width = fullscreenResolution.width;
			int height = fullscreenResolution.height;
			Screen.SetResolution(width, height, fullscreen: true);
			Debug.Log(width + " , " + height);
			PlayerPrefs.SetInt("fullscreen_width", width);
			PlayerPrefs.SetInt("fullscreen_height", height);
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

		private bool CheckInitialization()
		{
			if (!IsInitialized)
			{
				Initialize();
				return false;
			}
			if (!IsRunning)
			{
				if (CompileThread == null || CompileThread.IsAlive)
				{
					if (AssetManager.MaxLoading > 0)
					{
						LoadingText.text = "Preparing scripts (" + AssetManager.CurrentLoading + " of " + AssetManager.MaxLoading + ")...";
					}
					return false;
				}
				IsRunning = true;
				PostLoading();
			}
			if (SystemInit > 0)
			{
				return false;
			}
			return true;
		}

		private void Update()
		{
			if (!CheckInitialization())
			{
				return;
			}
			Logger.Update();
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
			if (IsRunning && CanAdvance && !WaitOnDelayedAction)
			{
				UpdateWaits();
				if (delayedActions != null)
				{
					if (!HasExistingWaits())
					{
						ExecuteDelayedActions();
					}
				}
				else
				{
					UpdateCarret();
					try
					{
						if (GameState == GameState.Normal)
						{
							TextController.Update();
							if (!IsSkipping)
							{
								goto IL_0127;
							}
							skipWait -= Time.deltaTime;
							if (!(skipWait <= 0f))
							{
								goto IL_0127;
							}
							if (!HasExistingWaits())
							{
								if (SkipModeDelay)
								{
									skipWait = 0.1f;
								}
								goto IL_0127;
							}
							ClearAllWaits();
						}
						goto end_IL_00b7;
						IL_0127:
						float num = Time.time + 0.01f;
						while (!HasExistingWaits() && !(Time.time > num) && !HasExistingWaits() && delayedActions == null && CanAdvance && !WaitOnDelayedAction)
						{
							ScriptSystem.Advance();
						}
						end_IL_00b7:;
					}
					catch (Exception)
					{
						IsRunning = false;
						throw;
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
			if (!IsRunning)
			{
				return;
			}
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
	}
}
