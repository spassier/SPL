/// <summary>
///	OnSite class
/// 
/// 2010/09/30
/// Copyright (c) 2010 - Sebastien PASSIER, All rights reserved
/// </summary>
/// 
//using System;
//using System.Collections.Generic;
//using System.Text;

namespace SPL
{
	public class OnSite
	{
		#region Properties
		private float m_Cost;
		public float Cost
		{
			get { return m_Cost; }
			private set { m_Cost = value; }
		}
		private float m_Distance;
		public float Distance
		{
			get { return m_Distance; }
			private set { m_Distance = value; }
		}
		#endregion
		
		#region Constructor
		public OnSite()
		{
			this.Cost = 0.0F;
			this.Distance = 0.0F;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Add a distance to the onsite's route
		/// </summary>
		/// <param name="kilometricValue"></param>
		public void AddDistance(float kilometricValue)
		{
			this.Distance += kilometricValue;
		}

		/// <summary>
		/// Substract a distance to the onsite's route
		/// </summary>
		/// <param name="kilometricValue"></param>
		public void SubDistance(float kilometricValue)
		{
			this.Distance -= kilometricValue;
		}

		/// <summary>
		/// Calculate the cost of the onsite.
		/// </summary>
		/// <param name="KilometricRate"></param>
		/// <returns></returns>
		public float CalculateCost(float KilometricRate)
		{
			float result = this.Distance * KilometricRate;
			this.Cost = result;

			return result;
		}

		public float CalculateCost(float KilometricRate, float PackageDeal)
		{
			float result = 0.0F;
			
			if (this.Distance > 0.0F)
			{
				if (KilometricRate > 0.0F)
					result = this.Distance * KilometricRate;
				else if (PackageDeal > 0.0F)
					result = PackageDeal;
			}

			this.Cost = result;

			return result;
		}

		#endregion

	}
}
