/// <summary>
///	SqlDB class
/// 
/// The main idea is to prevent the application operating from server disconnection by having a local copy of the DB in a XML file.
/// DataSet class has usefull functions to read or write XML file format and because the application only read small DB, it has been decided to work with decoupled data.
/// 
/// 2010/06/22
/// Copyright (c) 2010 - Sebastien PASSIER, All rights reserved
/// </summary>

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace SPL
{
	class SqlDB
	{
		#region Consts
		public const string DB_TABLE_NAME = "PointService";
		public const string DB_COLUMN_NAME = "Name";
		public const string DB_COLUMN_ADDRESS1 = "Address1";
		public const string DB_COLUMN_ADDRESS2 = "Address2";
		public const string DB_COLUMN_ZIPCODE = "ZipCode";
		public const string DB_COLUMN_CITY = "City";
		public const string DB_COLUMN_PHONE = "Phone";
		public const string DB_COLUMN_FAX = "Fax";
		public const string DB_COLUMN_EMAIL = "eMail";
		public const string DB_COLUMN_CONTACT = "Contact";
		public const string DB_COLUMN_DEPTNUMBER = "DeptNumber";

		private const string DB_DUMP_FILENAME = "ServicePointDB.xml";
		#endregion

		#region Variables
		private MySqlConnection m_sqlConnection = null;
		private DataSet m_DataSet = null;
		#endregion

		#region Methods
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool GetConnection()
		{
			bool bResult = true;

			// To prevent side effect of useless multicalls of this method...
			if (m_sqlConnection != null)
				return bResult;

			// Buold the connection string
			StringBuilder sqlConnectionString = new StringBuilder();
			sqlConnectionString.AppendFormat("server={0};database={1};username={2};password={3}", Properties.Settings.Default.SQL_SERVER,
																																														Properties.Settings.Default.SQL_DATABASE,
																																														Properties.Settings.Default.SQL_USERNAME,
																																														Properties.Settings.Default.SQL_PASSWORD);
			
			m_sqlConnection = new MySqlConnection(sqlConnectionString.ToString());

			// Just open the connnection now or handle an exception
			try
			{
				m_sqlConnection.Open();
			}
			catch (MySqlException ex)
			{
				bResult = false;

				m_sqlConnection.Close();
				m_sqlConnection = null;

				switch (ex.Number)
				{
					case 0:
						MessageBox.Show("Cannot connect to server" + Environment.NewLine + "Please contact administrator", ApplicationCommon.ExeName + " Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						Log.WriteLine("....Failure (0): {0}", ex.Message);
						break;
					case 1045:
						MessageBox.Show("Invalid username or password" + Environment.NewLine + "Please contact administrator", ApplicationCommon.ExeName + " Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						Log.WriteLine("....Failure (1045): {0}", ex.Message);
						break;
					default:
						MessageBox.Show("Connection to server failed" + Environment.NewLine + "Please contact administrator", ApplicationCommon.ExeName + " Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						Log.WriteLine("....Failure ({0}): {1}", ex.Number.ToString(), ex.Message);
						break;
				}
			}
			
			return bResult;
		}

		/// <summary>
		/// 
		/// </summary>
		public void CloseConnection()
		{
			if (m_sqlConnection != null)
			{
				m_sqlConnection.Close();
				m_sqlConnection.Dispose();
				m_sqlConnection = null;
			}
		}

		public DataSet GetTable()
		{
			if (m_sqlConnection != null)
			{
				// Build the sql command, we just select all columns in the default table
				StringBuilder sqlCommand = new StringBuilder();
				sqlCommand.AppendFormat("SELECT {0},{1},{2},{3},{4},{5},{6},{7},{8},{9} FROM {10}", DB_COLUMN_NAME,
																																														DB_COLUMN_ADDRESS1,
																																														DB_COLUMN_ADDRESS2,
																																														DB_COLUMN_ZIPCODE,
																																														DB_COLUMN_CITY,
																																														DB_COLUMN_PHONE,
																																														DB_COLUMN_FAX,
																																														DB_COLUMN_EMAIL,
																																														DB_COLUMN_CONTACT,
																																														DB_COLUMN_DEPTNUMBER,
																																														DB_TABLE_NAME);

				// Working with decoupled data
				MySqlDataAdapter sqlDataAdapter = new MySqlDataAdapter(sqlCommand.ToString(), m_sqlConnection);
				MySqlCommandBuilder sqlCommandBuilder = new MySqlCommandBuilder(sqlDataAdapter);

				if (m_DataSet != null)
					m_DataSet.Dispose();
				m_DataSet = new DataSet();

				sqlDataAdapter.Fill(m_DataSet, DB_TABLE_NAME);

				// Now we have the datas, dump them into an XML file to prevent application operating from server disconnection or or from others issues
				this.SaveData();
			}
			else
			{
				// load a previous copy stored locally in case of no connection or issue with the SQL server
				this.LoadData();
			}

			return m_DataSet;
		}

		/// <summary>
		/// 
		/// </summary>
		private void SaveData()
		{
			FileStream fStream = null;

			try
			{
				fStream = new FileStream(ApplicationCommon.GetEnvPath(ApplicationCommon.APPDIRECTORY.Settings) + DB_DUMP_FILENAME, System.IO.FileMode.Create);
			}
			catch
			{
				MessageBox.Show("Unable to save DB data in " + ApplicationCommon.GetEnvPath(ApplicationCommon.APPDIRECTORY.Settings) + DB_DUMP_FILENAME, ApplicationCommon.ExeName + " Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Application.Exit();
			}

			// Dump the DataSet into XML file
			m_DataSet.WriteXml(fStream);
			
			fStream.Flush();
			fStream.Close();

		}

		/// <summary>
		/// 
		/// </summary>
		private void LoadData()
		{
			FileStream fStream = null;
			
			try
			{
				fStream = new FileStream(ApplicationCommon.GetEnvPath(ApplicationCommon.APPDIRECTORY.Settings) + DB_DUMP_FILENAME, System.IO.FileMode.Open);
			}
			catch
			{
				MessageBox.Show("Unable to load DB data from " + ApplicationCommon.GetEnvPath(ApplicationCommon.APPDIRECTORY.Settings) + DB_DUMP_FILENAME, ApplicationCommon.ExeName + " Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Application.Exit();
			}

			// Load the DataSet from XML file
			m_DataSet.ReadXml(fStream);
			
			fStream.Flush();
			fStream.Close();
		}
		#endregion
	}
}
