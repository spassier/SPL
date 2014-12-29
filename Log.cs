/// <summary>
///	Log class
/// 
/// 2010/06/16
/// Copyright (c) 2010 - Sebastien PASSIER, All rights reserved
/// </summary>

using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace SPL
{
	public static class Log
	{
		#region Properties
		/// <summary>
		/// Indicates whetever the Log is enabled
		/// </summary>
		private static bool m_Enabled = true;
		public static bool Enabled
		{
			get { return m_Enabled; }
			set { m_Enabled = value; }
		}

		/// <summary>
		/// Indicates if the date/hour must be added in the Log
		/// </summary>
		private static bool m_Verbose = true;
		public static bool Verbose
		{
			get { return m_Verbose; }
			set { m_Verbose = value; }
		}
		#endregion

		#region Variables
		private static bool m_bInitialized = false;
		#endregion

		#region Methods
		/// <summary>
		/// Initialized the Log file using a unique listener
		/// So this method should be call one time before any other Log methods
		/// </summary>
		/// <param name="Path"></param>
		/// <returns></returns>
		public static bool Initialize(string Path)
		{
			bool bResult = true;

			if (m_bInitialized)
				return bResult;

			if (Properties.Settings.Default.Clear_Log)
			{
				if (File.Exists(Path))
				  File.Delete(Path);
			}

			Trace.Listeners.Add(new TextWriterTraceListener(Path));
			Trace.AutoFlush = true;

			m_bInitialized = true;

			return bResult;
		}
		
		/// <summary>
		/// Write a formatted string into the Log file
		/// </summary>
		/// <param name="StringFormat"></param>
		/// <param name="Args"></param>
		public static void WriteLine(string StringFormat, params string[] Args)
		{
			if (Enabled)
			{
				StringBuilder sStringFormatted = new StringBuilder(String.Format(StringFormat, Args));
				
				if (Verbose)
				{
					DateTime dtNow = DateTime.Now;
					StringBuilder sDateFormatted = new StringBuilder(String.Empty);
					sDateFormatted.AppendFormat("|{0:D2}:{1:D2}:{2:D2}| ", dtNow.Hour, dtNow.Minute, dtNow.Second);
					sStringFormatted.Insert(0, sDateFormatted);
				}

				Trace.WriteLine(sStringFormatted);
			}
		}

		#endregion
	}
}
