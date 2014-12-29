/// <summary>
///	LocatorService class
/// 
/// 2010/07/19
/// Copyright (c) 2010 - Sebastien PASSIER, All rights reserved
/// </summary>

using System;
using System.Collections.Generic;
using System.Text;
using SPL.net.mappoint.staging;


namespace SPL
{
	public sealed class LocatorService
	{
		#region Consts
		private const string DATASOURCENAME = "MapPoint.EU";
		private const string CULTURE_NAME = "fr";
		#endregion

		#region Variables
		private FindServiceSoap m_FindServiceSoap = null;
		private UserInfoFindHeader m_UserInfoFindHeader = null;
		private RouteServiceSoap m_RouteServiceSoap = null;
		private UserInfoRouteHeader m_UserInfoRouteHeader = null;
		#endregion

		#region Constructor
		public LocatorService()
		{ 
			// Initialize country name & default distance unit used in the global FindServiceSoap object
			m_UserInfoFindHeader = new UserInfoFindHeader();
			m_UserInfoFindHeader.Culture = new CultureInfo();
			m_UserInfoFindHeader.Culture.Name = CULTURE_NAME;
			m_UserInfoFindHeader.DefaultDistanceUnit = DistanceUnit.Kilometer;

			// Initialize the global FindServiceSoap used in address and LatLong search
			m_FindServiceSoap = new FindServiceSoap();
			m_FindServiceSoap.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.MapPoint_UserID, Properties.Settings.Default.MapPoint_Password);
			m_FindServiceSoap.UserInfoFindHeaderValue = m_UserInfoFindHeader;

			// Initialize country name & default distance unit used in the global RouteServiceSoap object
			m_UserInfoRouteHeader = new UserInfoRouteHeader();
			m_UserInfoRouteHeader.Culture = new CultureInfo();
			m_UserInfoRouteHeader.Culture.Name = CULTURE_NAME;
			m_UserInfoRouteHeader.DefaultDistanceUnit = DistanceUnit.Kilometer;

			// Initialize the global RouteServiceSoap used to compute distance between two cities
			m_RouteServiceSoap = new RouteServiceSoap();
			m_RouteServiceSoap.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.MapPoint_UserID, Properties.Settings.Default.MapPoint_Password);
			m_RouteServiceSoap.UserInfoRouteHeaderValue = m_UserInfoRouteHeader;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Find a geo-location (latitude/longitude) of a city from its Post Code
		/// </summary>
		/// <param name="postCode"></param>
		/// <returns></returns>
		public LatLong FindLatLongFromPostCode(string postCode)
		{
			LatLong latLong = null;

			// Initialize the search input & options
			FindAddressSpecification findAddressSpecification = new FindAddressSpecification();
			findAddressSpecification.DataSourceName = DATASOURCENAME;															// In which DB the search will occur
			findAddressSpecification.Options = new FindOptions();
			findAddressSpecification.Options.ResultMask = FindResultMask.LatLongFlag;							// Limite option to a LatLong search
			findAddressSpecification.InputAddress = new Address();
			findAddressSpecification.InputAddress.CountryRegion = m_FindServiceSoap.UserInfoFindHeaderValue.Culture.Name;
			findAddressSpecification.InputAddress.PostalCode = postCode;													// Post Code is the unique input

			// Execute the search request 
			FindResults findResults;
			try
			{
				findResults = m_FindServiceSoap.FindAddress(findAddressSpecification);
				
				// Get the LatLong object
				if (findResults.NumberFound > 0)
				{
					FindResult findResult;
					findResult = findResults.Results[0];	// Don't care about other hits
					latLong = findResult.FoundLocation.LatLong;
				}
			}
			catch(Exception ex)
			{
				latLong = null;
				Log.WriteLine("...WARNING: {0}", ex.Message);
			}

			return latLong;
		}

		/// <summary>
		/// Find a geo-location (latitude/longitude) of a city from its name
		/// </summary>
		/// <param name="city"></param>
		/// <returns></returns>
		public LatLong FindLatLongFromCity(string city)
		{
			LatLong latLong = null;

			// Initialize the search input & options
			FindAddressSpecification findAddressSpecification = new FindAddressSpecification();
			findAddressSpecification.DataSourceName = DATASOURCENAME;															// In which DB the search will occur
			findAddressSpecification.Options = new FindOptions();
			findAddressSpecification.Options.ResultMask = FindResultMask.LatLongFlag;							// Limite option to a LatLong search
			findAddressSpecification.InputAddress = new Address();
			findAddressSpecification.InputAddress.CountryRegion = m_FindServiceSoap.UserInfoFindHeaderValue.Culture.Name;
			findAddressSpecification.InputAddress.PrimaryCity = city;															// City is the unique input

			// Execute the search request 
			FindResults findResults;
			try
			{
				findResults = m_FindServiceSoap.FindAddress(findAddressSpecification);

				// Get the LatLong object
				if (findResults.NumberFound > 0)
				{
					FindResult findResult;
					findResult = findResults.Results[0];	// Don't care about other hits
					latLong = findResult.FoundLocation.LatLong;
				}
			}
			catch (Exception ex)
			{
				latLong = null;
				Log.WriteLine("...WARNING: {0}", ex.Message);
			}

			return latLong;
		}

		/// <summary>
		/// Compute the distance between two geo-locations (latitude/longitude) using the shortest route
		/// </summary>
		/// <param name="origin"></param>
		/// <param name="destination"></param>
		/// <returns></returns>
		public double FindShortestRoute(LatLong origin, LatLong destination)
		{
			double result = 0.0F;
			
			LatLong[] latLongList = new LatLong[2];
			latLongList[0] = new LatLong();
			latLongList[0].Latitude = origin.Latitude;
			latLongList[0].Longitude = origin.Longitude;
			latLongList[1] = new LatLong();
			latLongList[1].Latitude = destination.Latitude;
			latLongList[1].Longitude = destination.Longitude;

			// Execute the search request
			Route route;
			try
			{

				route = m_RouteServiceSoap.CalculateSimpleRoute(latLongList, DATASOURCENAME, SegmentPreference.Shortest);

				// Get the computed distance
				result = route.Itinerary.Distance;
			}
			catch (Exception ex)
			{
				result = 0.0F;
				Log.WriteLine("...WARNING: {0}", ex.Message);
			}

			return result;
		}

		public bool FindShortestRoute(LatLong origin, LatLong destination, out double distance, out long drivingTime)
		{
			bool result = true;

			LatLong[] latLongList = new LatLong[2];
			latLongList[0] = new LatLong();
			latLongList[0].Latitude = origin.Latitude;
			latLongList[0].Longitude = origin.Longitude;
			latLongList[1] = new LatLong();
			latLongList[1].Latitude = destination.Latitude;
			latLongList[1].Longitude = destination.Longitude;

			// Execute the search request
			Route route;
			// TODO: add a try catch in case of no connection
			route = m_RouteServiceSoap.CalculateSimpleRoute(latLongList, DATASOURCENAME, SegmentPreference.Shortest);

			// Get the computed distance & driving time
			distance = route.Itinerary.Distance;
			drivingTime = route.Itinerary.DrivingTime;
			
			return result;
		}

		public void FindItineraryFromPostCode(string postCode)
		{
			// 
			FindServiceSoap findService = new FindServiceSoap();
			findService.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.MapPoint_UserID, Properties.Settings.Default.MapPoint_Password);
			findService.UserInfoFindHeaderValue = m_UserInfoFindHeader;
			//
			FindAddressSpecification findAddressSpecification = new FindAddressSpecification();
			findAddressSpecification.DataSourceName = DATASOURCENAME;
			//
			FindOptions findOptions = new FindOptions();
			findOptions.ResultMask = FindResultMask.LatLongFlag;
			findAddressSpecification.Options = findOptions;
			//
			Address Address = new Address();
			Address.PostalCode = postCode;
			Address.CountryRegion = findService.UserInfoFindHeaderValue.Culture.Name;

			findAddressSpecification.InputAddress = Address;

			FindResults foundResults;
			foundResults = findService.FindAddress(findAddressSpecification);
			foreach (FindResult fr in foundResults.Results)
			{
				Log.WriteLine(fr.FoundLocation.LatLong.Latitude.ToString());
				Log.WriteLine(fr.FoundLocation.LatLong.Longitude.ToString());
			}

			//FindSpecification findSpecificarion = new FindSpecification();
			//findSpecificarion.DataSourceName = DATASOURCENAME;
			//findSpecificarion.InputPlace = "BLOIS";

			//FindResults foundResults;
			//foundResults = findService.Find(findSpecificarion);
			//foreach (FindResult fr in foundResults.Results)
			//{
			//  Log.WriteLine(fr.FoundLocation.Entity.DisplayName);
			//}

		}
		#endregion
	}
}
