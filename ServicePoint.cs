/// <summary>
///	ServicePoint class
/// 
/// 2010/06/22
/// Copyright (c) 2010 - Sebastien PASSIER, All rights reserved
/// </summary>

using System;

namespace SPL
{
	public class ServicePoint
	{
		#region Variables
		public string Name;
		public string Address1;
		public string Address2;
		public string ZipCode;
		public string City;
		public string Phone;
		public string Fax;
		public string Email;
		public string Contact;
		public string Department;
		public float HourlyRate;
		public float KilometricRate;
		public float PackageDeal;
		public float Lunch;
		
		public OnSite Intervention;
		#endregion

		#region Constructor
		public ServicePoint()
		{
			this.Name = String.Empty;
			this.Address1 = String.Empty;
			this.Address2 = String.Empty;
			this.ZipCode = String.Empty;
			this.City = String.Empty;
			this.Phone = String.Empty;
			this.Fax = String.Empty;
			this.Email = String.Empty;
			this.Contact = String.Empty;
			this.Department = String.Empty;
			this.HourlyRate = 0.0F;
			this.KilometricRate = 0.0F;
			this.PackageDeal = 0.0F;
			this.Lunch = 0.0F;
			
			this.Intervention = null;
		}
		#endregion
	}
}
