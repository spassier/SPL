/// <summary>
///	ServicePointDB class
/// 
/// 2010/06/22
/// Copyright (c) 2010 - Sebastien PASSIER, All rights reserved
/// </summary>

using System;
using System.Collections.Generic;
using System.Data;


namespace SPL
{
	public class ServicePointDB
	{
		#region Consts
		public const string DB_TABLE_NAME = "ServicePoint";
		public const string DB_COLUMN_NAME = "Name";
		public const string DB_COLUMN_ADDRESS1 = "Address1";
		public const string DB_COLUMN_ADDRESS2 = "Address2";
		public const string DB_COLUMN_ZIPCODE = "ZipCode";
		public const string DB_COLUMN_CITY = "City";
		public const string DB_COLUMN_PHONE = "Phone";
		public const string DB_COLUMN_FAX = "Fax";
		public const string DB_COLUMN_EMAIL = "Email";
		public const string DB_COLUMN_CONTACT = "Contact";
		public const string DB_COLUMN_DEPARTMENT = "Department";
		public const string DB_COLUMN_HOURLYRATE = "HourlyRate";
		public const string DB_COLUMN_KILOMETRICRATE = "KilometricRate";
		public const string DB_COLUMN_PACKAGEDEAL = "PackageDeal";
		public const string DB_COLUMN_LUNCH = "Lunch";
		#endregion

		#region Variables
		private List<ServicePoint> m_ServicePointList = null;
		#endregion
		
		#region Constructor
		public ServicePointDB()
		{
			m_ServicePointList = new List<ServicePoint>();
		}
		#endregion

		#region Methods
		/// <summary>
		/// 
		/// </summary>
		public void Load()
		{
			XmlDB xmlReader = new XmlDB();

			DataTable Table = xmlReader.GetTableFromXmlDB(DB_TABLE_NAME);

			// Each row of the Table is a ServicePoint of course
			foreach (DataRow Row in Table.Rows)
			{
				ServicePoint NewServicePoint = new ServicePoint();
				foreach (DataColumn Column in Table.Columns)
				{
					// Fill-in ServicePoint
					NewServicePoint.Name = Row[DB_COLUMN_NAME].ToString();
					NewServicePoint.Address1 = Row[DB_COLUMN_ADDRESS1].ToString();
					NewServicePoint.Address2 = Row[DB_COLUMN_ADDRESS2].ToString();
					NewServicePoint.ZipCode = Row[DB_COLUMN_ZIPCODE].ToString();
					NewServicePoint.City = Row[DB_COLUMN_CITY].ToString();
					NewServicePoint.Phone = Row[DB_COLUMN_PHONE].ToString();
					NewServicePoint.Fax = Row[DB_COLUMN_FAX].ToString();
					NewServicePoint.Email = Row[DB_COLUMN_EMAIL].ToString();
					NewServicePoint.Contact = Row[DB_COLUMN_CONTACT].ToString();
					NewServicePoint.Department = Row[DB_COLUMN_DEPARTMENT].ToString();
					NewServicePoint.HourlyRate = (float)Convert.ToDouble(Row[DB_COLUMN_HOURLYRATE].ToString());
					NewServicePoint.KilometricRate = (float)Convert.ToDouble(Row[DB_COLUMN_KILOMETRICRATE].ToString());
					NewServicePoint.PackageDeal = (float)Convert.ToDouble(Row[DB_COLUMN_PACKAGEDEAL].ToString());
					NewServicePoint.Lunch = (float)Convert.ToDouble(Row[DB_COLUMN_LUNCH].ToString());
				}
				// Add new ServicePoint in the global list
				m_ServicePointList.Add(NewServicePoint);
			}

		}

		/// <summary>
		/// Check if a ServicePoint object has the department number value parameter
		/// </summary>
		/// <param name="DepartmentNumber"></param>
		/// <returns></returns>
		public bool IsPopulate(string DepartmentNumber)
		{
			bool Result = false;

			Result = m_ServicePointList.Exists(
				delegate(ServicePoint SP)
				{
					if (SP.Department == DepartmentNumber)
						return true;
					else
						return false;
				}
			);

			return Result;
		}

		/// <summary>
		/// Retrieve all the ServicePoint object localized in the department number parameter
		/// </summary>
		/// <param name="DepartmentNumber"></param>
		/// <returns></returns>
		public List<ServicePoint> FindAllServicePoint(string DepartmentNumber)
		{
			List<ServicePoint> ServicePointList = new List<ServicePoint>();

			ServicePointList = m_ServicePointList.FindAll(
				delegate(ServicePoint SP)
				{
					if (SP.Department == DepartmentNumber)
						return true;
					else
						return false;
				}
			);

			return ServicePointList;
		}

		#endregion
	}
}
