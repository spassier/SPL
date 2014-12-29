/// <summary> 
///	SPL : Service Point Locator
///	
/// See Historic.txt file for details about updates & fixes
/// 
/// 2010/06/16
/// Copyright (c) 2010 - Sebastien PASSIER, All rights reserved
/// </summary> 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

using SPL.net.mappoint.staging;

namespace SPL
{
	static class Program
	{
		#region Globals Objets
		public static CoreLayer ApplicationCore = new CoreLayer();
		public static ServicePointDB ServicePointManager = null;
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			// Using a mutex to prevent multiples instances
			// --------------------------------------------
			bool bFirstInstance = true;
			using (Mutex mutex = new Mutex(true, ApplicationCommon.AssemblyGUID, out bFirstInstance))
			{
				// Check if application is already running
				if (!bFirstInstance)
				{
					// Send message to the first application instance to show itself (TopMost or raise from iconic)
					WinAPI.PostMessage((IntPtr)WinAPI.HWND_BROADCAST, ApplicationCommon.WM_SHOWFIRSTINSTANCE, IntPtr.Zero, IntPtr.Zero);
					return;
				}

				// Start logger
				// ------------
				Log.Initialize(ApplicationCommon.GetEnvPath(ApplicationCommon.APPDIRECTORY.Root) + ApplicationCommon.LOG_FILENAME);
				Log.Verbose = false;
				Log.WriteLine("----------------------------------------------------------------------------------------------------");
				Log.WriteLine("  Starting {0} - Version : {1}", ApplicationCommon.ExeName, ApplicationCommon.ExeVersion);
				Log.WriteLine("----------------------------------------------------------------------------------------------------");
				Log.Verbose = true;

				// Check Operating System version
				// ------------------------------
				Log.WriteLine("> Checking Operating System version");
				if (ApplicationCommon.GetOsVersion() == ApplicationCommon.OSVERSION.WIN7)
					Log.WriteLine("....Found Windows 7");
				else if (ApplicationCommon.GetOsVersion() == ApplicationCommon.OSVERSION.VISTA)
					Log.WriteLine("....Found Windows Vista");
				else if (ApplicationCommon.GetOsVersion() == ApplicationCommon.OSVERSION.WINXP)
					Log.WriteLine("....Found Windows XP");
				else
				{
					Log.WriteLine("....STOP: this operating system is not supported by this application");
					MessageBox.Show("This operating system is not supported by this application", ApplicationCommon.ExeName + " Stop", MessageBoxButtons.OK, MessageBoxIcon.Stop);
					// TODO : implemente a global exit method
					return;
				}

				// Check if application is running on the Guest Account
				// ----------------------------------------------------
				if (System.Security.Principal.WindowsIdentity.GetCurrent().IsGuest)
				{
					Log.WriteLine("> STOP: guest account is not supported by this application");
					MessageBox.Show("Guest account is not supported by this application", ApplicationCommon.ExeName + " Stop", MessageBoxButtons.OK, MessageBoxIcon.Stop);
					// TODO : implemente a global exit method
					return;
				}

				// Check screen resolution requirements
				// ------------------------------------
				Log.WriteLine("> Checking required screen resolution");
				Log.WriteLine("....Found {0}x{1} resolution", Screen.PrimaryScreen.Bounds.Width.ToString(), Screen.PrimaryScreen.Bounds.Height.ToString());
				if (Screen.PrimaryScreen.Bounds.Width <= Program.ApplicationCore.MinScreenWidth || Screen.PrimaryScreen.Bounds.Height <= Program.ApplicationCore.MinScreenHeight)
				{
					Log.WriteLine("....STOP: current screen resolution is not supported by this application");
					MessageBox.Show("Current screen resolution is not supported by this application", ApplicationCommon.ExeName + " Stop", MessageBoxButtons.OK, MessageBoxIcon.Stop);
					// TODO : implemente a global exit method
					return;
				}

				// Check if required files exists
				// ------------------------------
				string MissingFile = String.Empty;
				Log.WriteLine("> Checking required files existence");
				if (!File.Exists(ApplicationCommon.GetEnvPath(ApplicationCommon.APPDIRECTORY.Settings) + ApplicationCommon.SETTINGS_FILENAME))
					MissingFile = ApplicationCommon.SETTINGS_FILENAME;
				
				if (String.IsNullOrEmpty(MissingFile))
					Log.WriteLine("....Success");
				else
				{
					Log.WriteLine("....ERROR: fail to find {0}", MissingFile);
					MessageBox.Show("Missing required file : " + MissingFile, ApplicationCommon.ExeName + " Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					// TODO : implemente a global exit method
					return;
				}

				// Load XML DB
				// -----------
				Log.WriteLine("> Loading Service Point data base");
				ServicePointManager = new ServicePointDB();
				ServicePointManager.Load();

				// Load application settings
				// -------------------------
				Log.WriteLine("> Loading application settings");
				if (ApplicationCore.LoadSettings(ApplicationCommon.GetEnvPath(ApplicationCommon.APPDIRECTORY.Settings) + ApplicationCommon.SETTINGS_FILENAME))
					Log.WriteLine("....Success");
				else
				{
					// TODO : to something better with log message...
					Log.WriteLine("....ERROR");
					// TODO : implemente a global exit method
					return;
				}

				// Load application UI
				// -------------------
				Log.WriteLine("> Loading application UI");
				if (ApplicationCore.LoadUI())
					Log.WriteLine("....Success");
				else
				{
					// TODO : to something better with log message...
					Log.WriteLine("....ERROR");
					// TODO : implemente a global exit method
					return;
				}

				// Build the Department Graph used in postal code search
				// -----------------------------------------------------
				Log.WriteLine("> Building Department Graph");
				ApplicationCore.BuildDepartmentGraph();

				// Start the application
				// ---------------------
				Log.WriteLine("> Running the application");
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new MainForm());
			}
		}
	}
}
