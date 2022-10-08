using MOD.Scripts.Core;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Assets.Scripts.Core
{
	public class KeyHook
	{
		private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

		private const int WH_KEYBOARD_LL = 13;

		private const int WM_KEYDOWN = 256;

		private const int WM_KEYWITHALT = 260;

		private LowLevelKeyboardProc _proc = HookCallback;

		private static IntPtr _hookID = IntPtr.Zero;

		public KeyHook()
		{
			_hookID = SetHook(_proc);
			UnityEngine.Debug.Log("Enabled key hook");
		}

		private static IntPtr SetHook(LowLevelKeyboardProc proc)
		{
			using (Process process = Process.GetCurrentProcess())
			{
				using (ProcessModule processModule = process.MainModule)
				{
					return SetWindowsHookEx(13, proc, GetModuleHandle(processModule.ModuleName), 0u);
					IL_0027:
					IntPtr result;
					return result;
				}
			}
		}

		private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
		{
			if (nCode >= 0 && wParam == (IntPtr)260)
			{
				int num = Marshal.ReadInt32(lParam);
				if (num == 13 && GameSystem.Instance.HasFocus)
				{
					MODWindowManager.FullscreenToggle(showToast: true);
					return (IntPtr)1;
				}
			}
			return CallNextHookEx(_hookID, nCode, wParam, lParam);
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool UnhookWindowsHookEx(IntPtr hhk);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr GetModuleHandle(string lpModuleName);

		public void Unhook()
		{
			UnhookWindowsHookEx(_hookID);
		}
	}
}
