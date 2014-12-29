/// <summary>
///	XmlDB class
/// 
/// 2010/06/25
/// Copyright (c) 2010 - Sebastien PASSIER, All rights reserved
/// </summary>

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace SPL
{
	class XmlDB
	{
		#region Consts
		private const string DB_XML_FILENAME = "ServicePointDB.xml";
		private const string DB_XSD_FILENAME = "ServicePointDB.xsd";
		#endregion

		#region Variables
		private DataSet m_DataSet = null;
		#endregion

		#region Methods
		public DataTable GetTableFromXmlDB(string TableName)
		{
			if (m_DataSet != null)
				m_DataSet.Dispose();
			m_DataSet = new DataSet();
			
			// Load the XML schema, not very usefull at the moment...
			string xsdPath = ApplicationCommon.GetEnvPath(ApplicationCommon.APPDIRECTORY.Settings) + DB_XSD_FILENAME;
			if (File.Exists(xsdPath))
			{
				m_DataSet.ReadXmlSchema(xsdPath);
				Log.WriteLine("....Success to load XML schema");
			}
			else
				Log.WriteLine("....WARNING: fail to find {0}", xsdPath);
			
			string SharedPath = ApplicationCommon.GetEnvPath(ApplicationCommon.APPDIRECTORY.Shared) + DB_XML_FILENAME;
			string LocalPath = ApplicationCommon.GetEnvPath(ApplicationCommon.APPDIRECTORY.Data) + DB_XML_FILENAME;
			
			if (LoadData(SharedPath))
			{
				Log.WriteLine("....Success to load Xml DB from {0}", SharedPath);
				// Now backup locally the XmlDB to prevent issue with shared area
				if (SaveData(ApplicationCommon.GetEnvPath(ApplicationCommon.APPDIRECTORY.Data) + DB_XML_FILENAME))
					Log.WriteLine("....Success to backup XML DB");
				else
					Log.WriteLine("....WARNING: fail to copy Xml DB to {0}", LocalPath);
			}
			else
			{
				Log.WriteLine("....Unable to load Xml DB from {0}", SharedPath);
				// Failed, so try to load the local XmlDB
				if (LoadData(LocalPath))
				{
					Log.WriteLine("....Success to load Xml DB from {0}", LocalPath);
				}
				else
				{
					// No DB so exit the application
					Log.WriteLine("....ERROR: unable to load Xml DB from {0}", LocalPath);
					MessageBox.Show("Unable to load Xml DB from " + LocalPath, ApplicationCommon.ExeName + " Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					// TODO : call here a Program.Exit() method (that include application.exit())
					Application.Exit();
				}
			}

			return m_DataSet.Tables[TableName];
		}
		/// <summary>
		/// 
		/// </summary>
		private bool SaveData(string Path)
		{
			bool bResult = true;
			FileStream FStream = null;

			try
			{
				FStream = new FileStream(Path, System.IO.FileMode.Create);

				m_DataSet.WriteXml(FStream);

				FStream.Close();
			}
			catch
			{
				bResult = false;
			}
		
			return bResult;
		}

		/// <summary>
		/// 
		/// </summary>
		private bool LoadData(string Path)
		{
			bool bResult = true;
			FileStream FStream = null;

			try
			{
				FStream = new FileStream(Path, System.IO.FileMode.Open);

				m_DataSet.ReadXml(FStream);

				FStream.Close();
			}
			catch
			{
				bResult = false;
			}

			return bResult;
		}


		#endregion
	}
}
