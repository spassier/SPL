/// <summary>
///	Common class
/// 
/// 2010/06/16
/// Copyright (c) 2010 - Sebastien PASSIER, All rights reserved
/// </summary>

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;


namespace SPL
{
	sealed public class ApplicationCommon
	{
		#region Enums
		public enum APPDIRECTORY { Root, Data, Pictures, Settings, Shared };
		public enum OSVERSION { WIN7, VISTA, WINXP, WINNT, WIN2003, WIN2000, UNKNOWN };
		#endregion

		#region Constructors
		private ApplicationCommon() { }	// private constructor to prevent creation of class instance
		#endregion

		#region Consts
		public static readonly int WM_SHOWFIRSTINSTANCE = WinAPI.RegisterWindowMessage("WM_SHOWFIRSTINSTANCE|{0}", ApplicationCommon.AssemblyGUID);

		public const string LOG_FILENAME = "Log.txt";
		public const string SETTINGS_FILENAME = "Data.xml";

		const string DATA_DIRECTORY = "Data";
		const string PICTURES_DIRECTORY = "Pictures";
		const string SETTINGS_DIRECTORY = "Settings";
		#endregion

		#region Properties
		/// <summary>
		/// The root path of the application
		/// </summary>
		private static string m_RootPath = String.Empty;
		public static string RootPath
		{
			get
			{
				if (m_RootPath == String.Empty)
					m_RootPath = Directory.GetCurrentDirectory();

				return m_RootPath;
			}
		}

		/// <summary>
		/// The GUID associated with the application
		/// </summary>
		public static string AssemblyGUID
		{
			get
			{
				object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(System.Runtime.InteropServices.GuidAttribute), false);

				if (attributes.Length == 0)
					return String.Empty;
				else
					return ((System.Runtime.InteropServices.GuidAttribute)attributes[0]).Value;
			}
		}

		/// <summary>
		/// The .EXE name of the application
		/// </summary>
		/// <returns></returns>
		private static string m_ExeName = String.Empty;
		public static string ExeName
		{
			get
			{
				if (m_ExeName == String.Empty)
				{
					AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName();
					m_ExeName = assemblyName.Name;
				}
				
				return m_ExeName;
			}
		}

		/// <summary>
		/// The FileVersion attibute of the application
		/// </summary>
		/// <returns></returns>
		private static string m_ExeVersion = String.Empty;
		public static string ExeVersion
		{
			get
			{
				if (m_ExeVersion == String.Empty)
					m_ExeVersion = Assembly.GetEntryAssembly().GetName().Version.ToString();

				return m_ExeVersion;
			}
		}
		#endregion

		#region Methods
		/// <summary>
		/// Make environmment path 
		/// </summary>
		/// <param name="csDirectory"></param>
		/// <returns></returns>
		public static string GetEnvPath(APPDIRECTORY eDirectory)
		{
			string sEnvPath;

			switch (eDirectory)
			{
				case APPDIRECTORY.Data:
					sEnvPath = RootPath + @"\" + DATA_DIRECTORY + @"\";
					break;
				case APPDIRECTORY.Pictures:
					sEnvPath = RootPath + @"\" + PICTURES_DIRECTORY + @"\";
					break;
				case APPDIRECTORY.Settings:
					sEnvPath = RootPath + @"\" + SETTINGS_DIRECTORY + @"\";
					break;
				case APPDIRECTORY.Shared:
					sEnvPath = Properties.Settings.Default.SHARED_DIRECTORY + @"\";
					break;
				case APPDIRECTORY.Root:
				default:
					sEnvPath = RootPath + @"\";
					break;
			}

			return sEnvPath;
		}

		/// <summary>
		/// Retrieve the Operating System version on which the application is running
		/// Note : Win32s & Win32Windows are obsolete
		/// </summary>
		/// <returns></returns>
		public static OSVERSION GetOsVersion()
		{
			OSVERSION osVersion = OSVERSION.UNKNOWN;

			OperatingSystem osInfo = Environment.OSVersion;

			if (osInfo.Platform == PlatformID.Win32NT)
			{
				if (osInfo.Version.Major == 4)
					osVersion = OSVERSION.WINNT;
				else if (osInfo.Version.Major == 5 && osInfo.Version.Minor == 0)
					osVersion = OSVERSION.WIN2000;
				else if (osInfo.Version.Major == 5 && osInfo.Version.Minor == 1)
					osVersion = OSVERSION.WINXP;
				else if (osInfo.Version.Major == 5 && osInfo.Version.Minor == 2)
					osVersion = OSVERSION.WIN2003;
				else if (osInfo.Version.Major == 6 && osInfo.Version.Minor == 0)
					osVersion = OSVERSION.VISTA;
				else if (osInfo.Version.Major == 6 && osInfo.Version.Minor == 1)
					osVersion = OSVERSION.WIN7;
			}

			return osVersion;
		}
		#endregion
	}
}
