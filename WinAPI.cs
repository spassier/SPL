/// <summary>
///	WinAPI class
/// 
/// 2010/06/16
/// Copyright Sebastien PASSIER 2010
/// </summary>

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace SPL
{
	static class WinAPI
	{
		#region Consts
		public const int HWND_BROADCAST = 0xFFFF;
		public const int SW_SHOWNORMAL = 1;

		public const int WM_SIZING = 0x214;
		public const int WMSZ_LEFT = 1;
		public const int WMSZ_RIGHT = 2;
		public const int WMSZ_TOP = 3;
		public const int WMSZ_TOPLEFT = 4;
		public const int WMSZ_TOPRIGHT = 5;
		public const int WMSZ_BOTTOM = 6;
		public const int WMSZ_BOTTOMLEFT = 7;
		public const int WMSZ_BOTTOMRIGHT = 8;
		#endregion

		#region Structs
		// RECT type used as parameter with some WM_xxx messages 
		public struct RECT
		{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}
		#endregion

		#region Methods
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool PostMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		public static extern int RegisterWindowMessage(string message);

		public static int RegisterWindowMessage(string csFormat, params object[] Args)
		{
			string csMessage = String.Format(csFormat, Args);
			return RegisterWindowMessage(csMessage);
		}

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
		#endregion
	}
}
