/// <summary>
///	CoreLayer class
/// 
/// 2010/06/17
/// Copyright (c) 2010 - Sebastien PASSIER, All rights reserved
/// </summary>

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

using SPL.net.mappoint.staging;


namespace SPL
{
	public enum InfoMessageType { Normal, Warning, Error };

	public class CoreLayer
	{
		#region Const
		public const string UI_TEXT_NORMAL = "/n";
		public const string UI_TEXT_WARNING = "/w";
		public const string UI_TEXT_ERROR = "/e";

		private const int UI_MIN_SCREEN_WIDTH = 800;
		private const int UI_MIN_SCREEN_HEIGHT = 600;
		private const string UI_MAP_FILENAME = "FranceMap.png";
		private const int UI_MIN_MAP_HUE = 211;
		private const string UI_FLAG_FILENAME = "Flag.png";
		private const string UI_LEAFLET_GLYPH_FILENAME = "Info.png";
		private const string UI_LOCATOR_GLYPH_FILENAME = "Finder.png";

		private const string XML_ELEMENT_DEPARTMENT = "Department";
		private const string XML_ELEMENT_NAME = "Name";
		private const string XML_ELEMENT_NUMBER = "Number";
		private const string XML_ELEMENT_COLORID = "ColorID";
		private const string XML_ELEMENT_FLAGLOCATION = "FlagLocation";
		private const string XML_ELEMENT_X = "X";
		private const string XML_ELEMENT_Y = "Y";
		private const string XML_ELEMENT_NODECONNECTIONS = "NodeConnections";
		private const string XML_ELEMENT_NODECONNECTION = "NodeConnection";
		private const string XML_ELEMENT_NODEID = "NodeID";
		private const string XML_ELEMENT_WEIGHT = "Weight";

		private const string CORSE_DU_SUD = "2A";
		private const string HAUTE_CORSE = "2B";
		private const int CORSE_DU_SUD_ID = 201;	// 201xx & 200xx are postal codes reserved for CORSE DU SUD (200xx Ajaccio)
		private const int HAUTE_CORSE_ID = 202;		// 202xx & 206xx are postal codes reserved for HAUTE CORSE (206xx Bastia)

		private const int MIN_SERVICEPOINT_FOUND = 3;
		private const uint MIN_SEARCH_THRESHOLD = 1;
		private const uint MAX_SEARCH_THRESHOLD = 3;
		#endregion

		#region Variables
		private Dictionary<string, Department> DepartmentList = null;
		private Graph DepartmentGraph = null;
		#endregion

		#region Properties
		private int m_minScreenWidth = UI_MIN_SCREEN_WIDTH;
		public int MinScreenWidth
		{
			get { return m_minScreenWidth;  }
		}
		private int m_minScreenHeight = UI_MIN_SCREEN_HEIGHT;
		public int MinScreenHeight
		{
			get { return m_minScreenHeight; }
		}

		private Bitmap m_bmpResourceMap = null;
		public Bitmap bmpResourceMap
		{
			get { return m_bmpResourceMap; }
			private set { m_bmpResourceMap = value; }
		}
		private Bitmap m_bmpCanvasMap = null;
		public Bitmap bmpCanvasMap
		{
			get { return m_bmpCanvasMap; }
			private set { m_bmpCanvasMap = value; }
		}
		private Bitmap m_bmpFlag = null;
		public Bitmap bmpFlag
		{
			get { return m_bmpFlag; }
			private set { m_bmpFlag = value; }
		}
		private Bitmap m_bmpLeafletGlyph = null;
		public Bitmap bmpLeafletGlyph
		{
			get { return m_bmpLeafletGlyph; }
			private set { m_bmpLeafletGlyph = value; }
		}
		private Bitmap m_bmpLocatorGlyph = null;
		public Bitmap bmpLocatorGlyph
		{
			get { return m_bmpLocatorGlyph; }
			private set { m_bmpLocatorGlyph = value; }
		}

		private int m_MinServicePointFound = MIN_SERVICEPOINT_FOUND;
		public int MinServicePointFound
		{
			get { return m_MinServicePointFound; }
			private set { m_MinServicePointFound = value; }
		}
		#endregion

		#region Delegates & Events
		public delegate void SendTextEventHandler(string text);
		public event SendTextEventHandler SendTextEvent;
		#endregion

		#region Constructors
		public CoreLayer()
		{
			DepartmentList = new Dictionary<string, Department>();
			DepartmentGraph = new Graph();
		}
		#endregion

		#region Methods
		/// <summary>
		/// 
		/// </summary>
		/// <param name="messageText"></param>
		/// <param name="messageType"></param>
		public void ShowInfoMessage(string messageText, InfoMessageType messageType = InfoMessageType.Normal)
		{
			// TODO: handle case speficiations...
			switch (messageType)
			{
				case InfoMessageType.Normal:
					messageText += UI_TEXT_NORMAL;
					this.SendText(messageText);
					break;
				case InfoMessageType.Warning:
					messageText += UI_TEXT_WARNING;
					this.SendText(messageText);
					break;
				case InfoMessageType.Error:
					messageText += UI_TEXT_ERROR;
					this.SendText(messageText);
					break;
				default:
					break;
			}
		}
		
		/// <summary>
		/// Retrieves a GraphicsPath object corresponding to this color
		/// </summary>
		/// <param name="ColorID"></param>
		/// <returns></returns>
		public GraphicsPath GetGraphicsPathFromColor(ref Color ColorID)
		{
			string Key = StringColorFromColor(ref ColorID);

			Department department;
			if (DepartmentList.TryGetValue(Key, out department))
				return department.GraphicObject;
			else
				return null;
		}

		/// <summary>
		/// Determines if a Department object exists with this color
		/// </summary>
		/// <param name="ColorID"></param>
		/// <returns></returns>
		public bool IsDepartmentColor(ref Color ColorID)
		{
			string Key = StringColorFromColor(ref ColorID);

			if (DepartmentList.ContainsKey(Key))
				return true;
			else
				return false;
		}

		/// <summary>
		/// Get the Departement title (using this template: "department_name (department_number)")
		/// </summary>
		/// <param name="ColorID"></param>
		/// <returns></returns>
		public string GetDepartmentTitle(Color ColorID)
		{
			// FIXME: should be check if the ColorID is a department color ?
			string Key = StringColorFromColor(ref ColorID);

			return DepartmentList[Key].Name + " (" + DepartmentList[Key].Number + ")";
		}

		/// <summary>
		/// Retrieve and format a series of string for each ServicePoint object found in a department
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		public List<StringCollection> GetAllServicePointText(Color color)
		{
			List<StringCollection> textList = new List<StringCollection>();

			// Retrieve all ServicePoint objects localized in the current department number
			string colorID = StringColorFromColor(ref color);
			List<ServicePoint> servicePointList = Program.ServicePointManager.FindAllServicePoint(DepartmentList[colorID].Number);

			// For each ServicePoint object found, get their formatted text and add it to the list
			foreach (ServicePoint servicePoint in servicePointList)
			{
				textList.Add(GetServicePointText(servicePoint));
			}

			return textList;
		}

		/// <summary>
		/// Extract postal code and city name from a string
		/// </summary>
		/// <param name="searchText"></param>
		public void ParseSearch(string searchText, out string postalCode, out string city)
		{
			// Find a postal code
			// ------------------
			int count = 0;
			int startindex = -1;
			for (int i = 0; i < searchText.Length; i++)//each (Char character in searchText)
			{
				if (Char.IsDigit(searchText[i]))
				{
					if (startindex == -1)										// Case: start to count digit
					{
						startindex = i;	// Backup the index of the start
						count = 1;			// Inc digit counter
					}
					else																		// Case: counting has beun so inc counter 
					{
						count++;
					}
				}
				else if (startindex != -1)								// Case: not a digit but counting has been started
				{
					if (count == 5)													// Case: found exactly 5 continous digits => exit the loop
						break;
					else																		// Case: found less or more than 5 continous digit
					{
						startindex = -1; // Reset starting index and counter only if 5 digits has not been found
						count = 0;
					}
				}
			}

			if (count == 5)
				postalCode = searchText.Substring(startindex, count);
			else
				postalCode = string.Empty;

			// Find a city name
			// ----------------
			if (postalCode != String.Empty)
				city = searchText.Remove(startindex, count);
			else
				city = searchText;

			city = city.ToLower();	// Don't care about upper case
			city = city.Trim();			// Clean a little the string

			count = 0;
			foreach (Char character in city)
			{
				switch (character)
				{
					case ' ':
					case 'é':
					case '\'':
					case '-':
					case 'è':
					case 'ç':
					case 'à':
					case 'û':
					case 'a':
					case 'b':
					case 'c':
					case 'd':
					case 'e':
					case 'f':
					case 'g':
					case 'h':
					case 'i':
					case 'j':
					case 'k':
					case 'l':
					case 'm':
					case 'n':
					case 'o':
					case 'p':
					case 'q':
					case 'r':
					case 's':
					case 't':
					case 'u':
					case 'v':
					case 'w':
					case 'x':
					case 'y':
					case 'z':
						count++;
						break;
					default:
						break;
				}
			}

			if (count != city.Length)
				city = String.Empty;
		}

		/// <summary>
		/// Get a formatted series of strings from a ServicePoint object
		/// </summary>
		/// <param name="servicePoint"></param>
		/// <returns></returns>
		private StringCollection GetServicePointText(ServicePoint servicePoint)
		{
			StringCollection text = new StringCollection();

			// Format Name string
			text.Add(servicePoint.Name);

			// Format Address string
			StringBuilder Address = new StringBuilder();
			if (servicePoint.Address1 != String.Empty)
				Address.AppendLine(servicePoint.Address1);
			if (servicePoint.Address2 != String.Empty)
				Address.AppendLine(servicePoint.Address2);
			if (servicePoint.ZipCode != String.Empty)
				Address.AppendFormat("{0} {1}", servicePoint.ZipCode, servicePoint.City);
			text.Add(Address.ToString());

			// Format contact string
			StringBuilder Contact = new StringBuilder();
			Contact.Append("Contact: ");
			Contact.AppendLine(servicePoint.Contact);
			Contact.Append("Tèl: ");
			Contact.AppendLine(servicePoint.Phone);
			Contact.Append("Fax: ");
			Contact.AppendLine(servicePoint.Fax);
			text.Add(Contact.ToString());

			// Format Email string
			text.Add(servicePoint.Email);

			// Format Rate string
			StringBuilder Rates = new StringBuilder();
			if (servicePoint.HourlyRate != 0.0F)
				Rates.AppendFormat("Taux horaire: {0:C}\r\n", servicePoint.HourlyRate);
			if (servicePoint.KilometricRate != 0.0F)
				Rates.AppendFormat("Taux kilométrique: {0:C}/km\r\n", servicePoint.KilometricRate);
			if (servicePoint.PackageDeal != 0.0F)
				Rates.AppendFormat("Forfait: {0:C}\r\n", servicePoint.PackageDeal);
			if (servicePoint.Lunch != 0.0F)
				Rates.AppendFormat("Frais de vie: {0:C}", servicePoint.Lunch);
			text.Add(Rates.ToString());

			// Format Result string
			string Result = String.Empty;
			text.Add(Result);

			return text;
		}

		/// <summary>
		/// Check whetever the postal code entry is well formatted (5 digital characters) and if the 2 first digits correspond to a department.
		/// </summary>
		/// <param name="postalCode"></param>
		/// <returns></returns>
		public bool IsValidePostalCode(string postalCode)
		{
			bool result = true;

			// Handle the format
			if (postalCode.Length == 5)
			{
				foreach (Char Item in postalCode)
				{
					if (!char.IsDigit(Item))
					{
						result = false;
						break;
					}
				}

				// Now check if it is a department number + corse special cases
				if (result)
				{
					string departmentNumber = postalCode.Substring(0, 2);
					switch (departmentNumber)
					{
						case "00":
						case "96":
						case "97":
						case "98":
						case "99":
							result = false;
							break;
						case "20":
							{
								string subCode = postalCode.Substring(3, 1); // Third value must be 0, 1, 2 or 6
								switch (subCode)
								{
									case "0":
									case "1":
									case "2":
									case "6":
										result = true;
										break;
									default:
										result = false;
										break;
								}
								break;
							}
						default:
							result = true;
							break;
					}
				}
			}
			else
			{
				result = false;
			}

			// Update the status message of the locator control
			if (result)
				this.SendText("");
			else
				this.SendText("Code postal invalide");

			return result;
		}

		/// <summary>
		/// Retrieve and format a series of string at least for 3 ServicePoint objects supplying to the search threshold and criteria
		/// </summary>
		/// <param name="postalCode"></param>
		/// <param name="searchCriteria"></param>
		/// <returns></returns>
		public List<StringCollection> GetAllServicePointText(string postalCode, string cityName, int searchThreshold, SearchCriteria searchCriteria)
		{
			List<StringCollection> textList = new List<StringCollection>();
			List<ServicePoint> servicePointFound = new List<ServicePoint>(); // Create the final List of Service Point found

			int departmentNumber = ConvertPostalCodeToInt32(postalCode);

			// Retrieve a minimum number of service point by using a graph of departments
			// --------------------------------------------------------------------------
			uint threshold = MIN_SEARCH_THRESHOLD;
			do
			{
				List<int> resultList = DepartmentGraph.BreadthFirstSearch(departmentNumber, threshold);

				// Increase the range of research for next pass if necessary
				// FIXEME: what to do if (threshold > MAX_SEARCH_THRESHOLD) ?
				threshold++;

				// Clear the service point found list to prevent adding same service point found in a previous search using a lower threshold
				if (servicePointFound.Count != 0)
					servicePointFound.Clear();

				// Get all services points localized in this department list
				foreach (int currentDepartmentNumber in resultList)
				{
					// Add in the global list all services points localized in this department
					servicePointFound.AddRange(Program.ServicePointManager.FindAllServicePoint(ConvertPostalCodeToString(currentDepartmentNumber)));
				}

				// Handle special case with Corse in case number of service point found is less than the threshold limit
				// Notice that if the MIN_SEARCH_THRESHOLD is set to 0, this algorithm will be bugged because the search will be limited to one of the 2 departments
				if (departmentNumber == CORSE_DU_SUD_ID || departmentNumber == HAUTE_CORSE_ID)
					break;

			} while (servicePointFound.Count < searchThreshold);
			
			Log.WriteLine("...Found {0} services points using a graph threshold of {1}", servicePointFound.Count.ToString(), Convert.ToString(threshold - 1));

			// Compute the distance from onsite to all services point found using MapPoint WebService or a DSF graph search if no internet connection
			// --------------------------------------------------------------------------------------------------------------------------------------
			LocatorService locatorService = new LocatorService();
			LatLong origin = new LatLong();
			bool error = false;

			origin = GetLatLong(ref locatorService, postalCode, cityName);
			if (origin != null)
			{
				foreach (ServicePoint servicePoint in servicePointFound)
				{
					double distance = 0.0F;
					LatLong destination = new LatLong();
					
					destination = GetLatLong(ref locatorService, servicePoint.ZipCode, servicePoint.City);
					if (destination != null)
					{
						// Get the distance
						distance = locatorService.FindShortestRoute(origin, destination);

						// Add distance and compute cost into an onsite object assigned to the current service point
						servicePoint.Intervention = new OnSite();
						servicePoint.Intervention.AddDistance((float)distance * 2.0F);
						servicePoint.Intervention.CalculateCost(servicePoint.KilometricRate, servicePoint.PackageDeal);

						Log.WriteLine("...Distance from {0} {1} to {2} {3} is {4} km", postalCode, cityName, servicePoint.ZipCode, servicePoint.City, distance.ToString());
					}
					else
					{
						error = true;
						break; // Postal code and city name not found ?!
					}
				}
			}
			
			// Sort results and add them into final text list
 			// ----------------------------------------------
			if (origin != null && error == false)
			{
				// Sort the services points according to the search criteria
				servicePointFound.Sort(delegate(ServicePoint sp1, ServicePoint sp2)
				{
					switch (searchCriteria)
					{
						case SearchCriteria.Nearest:
							return sp1.Intervention.Distance.CompareTo(sp2.Intervention.Distance);
						case SearchCriteria.Cheapest:
						case SearchCriteria.Quickest:
						default:
							return sp1.Intervention.Cost.CompareTo(sp2.Intervention.Cost);
					}
				});

				// Now add formated text into the text list (used by the leaflet control)
				foreach (ServicePoint servicePoint in servicePointFound)
					textList.Add(GetServicePointAndSearchText(servicePoint));
			}


			return textList;
		}

		/// <summary>
		/// Get LatLong object either by using posta code neither with city.
		/// </summary>
		/// <param name="locatorService"></param>
		/// <param name="postalCode"></param>
		/// <param name="city"></param>
		/// <returns></returns>
		private LatLong GetLatLong(ref LocatorService locatorService, string postalCode, string city)
		{
			LatLong result = new LatLong();

			result = locatorService.FindLatLongFromPostCode(postalCode);
			if (result == null)
			{
				Log.WriteLine("...WARNING: postal code {0} not found by Bing Maps WebService", postalCode);

				if (city != null)
				{
					// Remove "CEDEX" from the city name else MapPoint will not be able to find the city
					string formattedCity = city.ToUpper();
					int startIndex = formattedCity.IndexOf("CEDEX");
					formattedCity = formattedCity.Remove(startIndex);

					result = locatorService.FindLatLongFromCity(formattedCity);
					if (result == null)
						Log.WriteLine("...WARNING: city name {0} not found by Bing Maps WebService", formattedCity);
					else
						Log.WriteLine("...Using city name {0}", formattedCity);
				}
			}
			else
			{
				Log.WriteLine("...Using postal code {0}", postalCode);
			}

			if (result == null)
				Log.WriteLine("...Cancel the search request");
			
			return result;
		}

		/// <summary>
		/// Convert the type of a postal code from string to int32.
		/// The string could be 5 or 2 characters long because the input may coming from Department class (2 characters) or from LocatorSerach class (5 characters).
		/// This method handle Corse departmeent exception but doesn't check the validity of the postal code.
		/// </summary>
		/// <param name="postalCode"></param>
		/// <returns></returns>
		private int ConvertPostalCodeToInt32(string postalCode)
		{
			int result = 0;

			string departmentNumber = postalCode.Substring(0, 2);
			switch (departmentNumber)
			{
				case "2A":
					result = CORSE_DU_SUD_ID;
					break;
				case "2B":
					result = HAUTE_CORSE_ID;
					break;
				case "20":
				{
					string subCode = postalCode.Substring(3, 1); // Third value must be 0, 1, 2 or 6
					switch (subCode)
					{
						case "0":
						case "1":
							result = CORSE_DU_SUD_ID;
							break;
						case "2":
						case "6":
							result = HAUTE_CORSE_ID;
							break;
					}
					break;
				}
				default :
					result = Convert.ToInt32(departmentNumber);
					break;
			}

			return result;
		}

		/// <summary>
		/// Convert the type of a postal code from int32 to string.
		/// This method handle Corse departmeent exception but doesn't check the validity of the postal code.
		/// </summary>
		/// <param name="postalCode"></param>
		/// <returns></returns>
		private string ConvertPostalCodeToString(int postalCode)
		{
			string result = String.Empty;

			switch (postalCode)
			{
				case CORSE_DU_SUD_ID:
					result = CORSE_DU_SUD;
					break;
				case HAUTE_CORSE_ID:
					result = HAUTE_CORSE;
					break;
				default:
					result = Convert.ToString(postalCode);
					break;
			}

			return result;
		}

		/// <summary>
		/// Get a formatted series of strings from a ServicePoint object
		/// </summary>
		/// <param name="servicePoint"></param>
		/// <returns></returns>
		private StringCollection GetServicePointAndSearchText(ServicePoint servicePoint)
		{
			StringCollection text = new StringCollection();

			// Format Name string
			text.Add(servicePoint.Name);

			// Format Address string
			StringBuilder Address = new StringBuilder();
			if (servicePoint.Address1 != String.Empty)
				Address.AppendLine(servicePoint.Address1);
			if (servicePoint.Address2 != String.Empty)
				Address.AppendLine(servicePoint.Address2);
			if (servicePoint.ZipCode != String.Empty)
				Address.AppendFormat("{0} {1}", servicePoint.ZipCode, servicePoint.City);
			text.Add(Address.ToString());

			// Format contact string
			StringBuilder Contact = new StringBuilder();
			Contact.Append("Contact: ");
			Contact.AppendLine(servicePoint.Contact);
			Contact.Append("Tèl: ");
			Contact.AppendLine(servicePoint.Phone);
			Contact.Append("Fax: ");
			Contact.AppendLine(servicePoint.Fax);
			text.Add(Contact.ToString());

			// Format Email string
			text.Add(servicePoint.Email);

			// Format Rate string
			StringBuilder Rates = new StringBuilder();
			if (servicePoint.HourlyRate != 0.0F)
				Rates.AppendFormat("Taux horaire: {0:C}\r\n", servicePoint.HourlyRate);
			if (servicePoint.KilometricRate != 0.0F)
				Rates.AppendFormat("Taux kilométrique: {0:C}/km\r\n", servicePoint.KilometricRate);
			if (servicePoint.PackageDeal != 0.0F)
				Rates.AppendFormat("Forfait: {0:C}\r\n", servicePoint.PackageDeal);
			if (servicePoint.Lunch != 0.0F)
				Rates.AppendFormat("Frais de vie: {0:C}", servicePoint.Lunch);
			text.Add(Rates.ToString());

			// Format Result string
			StringBuilder Result = new StringBuilder();
			if (servicePoint.Intervention != null)
			{
				if (servicePoint.Intervention.Distance != 0.0F)
				{
					Result.AppendFormat("Coût estimé: {0:C}\r\n", servicePoint.Intervention.Cost);
					Result.AppendFormat("Distance estimée: {0} km", servicePoint.Intervention.Distance);
				}
				else
					Result.AppendFormat("Même ville que le lieu d'intervention");
			}
			text.Add(Result.ToString());

			return text;
		}

		public List<Point> GetFlagLocation()
		{
			List<Point> PointList = new List<Point>();

			foreach (KeyValuePair<string, Department> Item in DepartmentList)
					PointList.Add(Item.Value.FlagLocation);

			return PointList;
		}

		public StringCollection GetFlagString()
		{
			StringCollection StringList = new StringCollection();

			foreach (KeyValuePair<string, Department> Item in DepartmentList)
					StringList.Add(Item.Value.Number);

			return StringList;
		}

		/// <summary>
		/// 
		/// </summary>
		public void BuildDepartmentGraph()
		{
			// Add all department number in the graph (the nodes)
			foreach (KeyValuePair<string, Department> department in DepartmentList)
			{
				int nodeID;
				// Handle exception with 2A & 2B deparment number
				if (department.Value.Number == CORSE_DU_SUD)
					nodeID = CORSE_DU_SUD_ID;
				else if (department.Value.Number == HAUTE_CORSE)
					nodeID = HAUTE_CORSE_ID;
				else
					nodeID = Convert.ToInt32(department.Value.Number);

				DepartmentGraph.AddNode(nodeID);
			}

			// Add nodes connections
			foreach (KeyValuePair<string, Department> department in DepartmentList)
			{
				foreach (Connection connection in department.Value.ConnectionList)
				{
					int nodeID;
					// Handle exception with 2A & 2B deparment number
					if (department.Value.Number == CORSE_DU_SUD)
						nodeID = CORSE_DU_SUD_ID;
					else if (department.Value.Number == HAUTE_CORSE)
						nodeID = HAUTE_CORSE_ID;
					else
						nodeID = Convert.ToInt32(department.Value.Number);

					DepartmentGraph.ConnectNode(nodeID, connection.NeighborNodeID, connection.Weight);
				}
			}
		}

		/// <summary>
		/// Load the settings file (using an XML document)
		/// </summary>
		/// <param name="Path"></param>
		/// <returns></returns>
		public bool LoadSettings(string Path)
		{
			bool Result = true;

			XmlDocument xmlSettings = new XmlDocument();

			xmlSettings.Load(Path);

			// Create a list XML node Department
			XmlNodeList xmlDepartmentNodeList = xmlSettings.GetElementsByTagName(XML_ELEMENT_DEPARTMENT);
			foreach (XmlNode xmlDepartmentNode in xmlDepartmentNodeList)
			{
				Department department = new Department();

				// Fill the Departement object
				department.Number = xmlDepartmentNode[XML_ELEMENT_NUMBER].InnerText;
				department.Name = xmlDepartmentNode[XML_ELEMENT_NAME].InnerText;
				department.ColorID = xmlDepartmentNode[XML_ELEMENT_COLORID].InnerText;
				XmlNode xmlFlagLocationNode = xmlDepartmentNode[XML_ELEMENT_FLAGLOCATION];
				department.FlagLocation.X = Convert.ToInt32(xmlFlagLocationNode[XML_ELEMENT_X].InnerText);
				department.FlagLocation.Y = Convert.ToInt32(xmlFlagLocationNode[XML_ELEMENT_Y].InnerText);
				
				// Fill the Connection list
				XmlNodeList xmlConnectionList = xmlDepartmentNode[XML_ELEMENT_NODECONNECTIONS].GetElementsByTagName(XML_ELEMENT_NODECONNECTION);
				foreach (XmlNode xmlConnectionNode in xmlConnectionList)
				{
					Connection connection = new Connection();
					int neighborNodeID;

					// Handle exception with 2A & 2B deparment number
					if (xmlConnectionNode[XML_ELEMENT_NODEID].InnerText == CORSE_DU_SUD)
						neighborNodeID = CORSE_DU_SUD_ID;
					else if (xmlConnectionNode[XML_ELEMENT_NODEID].InnerText == HAUTE_CORSE)
						neighborNodeID = HAUTE_CORSE_ID;
					else
						neighborNodeID = Convert.ToInt32(xmlConnectionNode[XML_ELEMENT_NODEID].InnerText);

					connection.NeighborNodeID = neighborNodeID;
					// Be careful with the culture code ',' for FR and '.' for US...
					connection.Weight = (float)Convert.ToDouble(xmlConnectionNode[XML_ELEMENT_WEIGHT].InnerText, System.Globalization.CultureInfo.GetCultureInfo("en-US").NumberFormat);
					department.ConnectionList.Add(connection);
				}

				// Add a new Departement in the global list using <key:ColoID><value:Departement>
				try
				{
					DepartmentList.Add(department.ColorID, department);
				}
				catch (ArgumentException ex)
				{
					Log.WriteLine("....ERROR :" + ex.Message);
					Result = false;
					break;
				}
			}

			return Result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool LoadUI()
		{
			bool Result = true;

			Assembly Assembly = Assembly.GetExecutingAssembly();

			// The resource stream name is built like this : <project namespace>.<Resources>.<filename.<format>> and beware of the case
			// Load each graphics stored as resources
			Stream Stream = Assembly.GetManifestResourceStream("SPL.Resources." + UI_MAP_FILENAME);
			if (this.bmpResourceMap != null)
				this.bmpResourceMap.Dispose();
			this.bmpResourceMap = new Bitmap(Stream);

			Stream = Assembly.GetManifestResourceStream("SPL.Resources." + UI_FLAG_FILENAME);
			if (this.bmpFlag != null)
				this.bmpFlag.Dispose();
			this.bmpFlag = new Bitmap(Stream);

			Stream = Assembly.GetManifestResourceStream("SPL.Resources." + UI_LEAFLET_GLYPH_FILENAME);
			if (this.bmpLeafletGlyph != null)
				this.bmpLeafletGlyph.Dispose();
			this.bmpLeafletGlyph = new Bitmap(Stream);

			Stream = Assembly.GetManifestResourceStream("SPL.Resources." + UI_LOCATOR_GLYPH_FILENAME);
			if (this.bmpLocatorGlyph != null)
				this.bmpLocatorGlyph.Dispose();
			this.bmpLocatorGlyph = new Bitmap(Stream);

			Stream.Close();

			// Update the Map by changing the color of each Department having at least a Service Point, and, update the data dictionary <key:ColorID><value:Departement>
			GenerateDepartmentsGraphicPathFromMap(this.bmpResourceMap);
			UpdateMapColor(this.bmpResourceMap, Properties.Settings.Default.Hue - UI_MIN_MAP_HUE); // Hue offset = Hue we want - Min Hue in Map

			return Result;
		}

		/// <summary>
		/// Create for each Department object of the Map object its associated GraphicsPath object.
		/// Unsafe code is used to increase scan speed.
		/// </summary>
		/// <param name="Map"></param>
		private void GenerateDepartmentsGraphicPathFromMap(Bitmap Map)
		{
			unsafe
			{
				// Lock the Bitmap object to access it unsafe 
				BitmapData bmpData = Map.LockBits(new Rectangle(0, 0, Map.Width, Map.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
				Byte* PixelPtr = (Byte*)bmpData.Scan0;
				int Offset = Map.Width % 4; // Using 24bit format
				Department Dept;
				string Color;
				Rectangle Rect = new Rectangle(0, 0, 1, 1);
				
				// Scan the bitmap in row[column]
				for (int Y = 0; Y < Map.Height; Y++, PixelPtr += Offset)
				{
					Rect.Y = Y;
					for (int X = 0; X < Map.Width; X++, PixelPtr += 3)
					{
						// Format the detected color (the PixelPtr[] is fill in BGR not RGB)
						Color = "255." + PixelPtr[2].ToString() + "." + PixelPtr[1].ToString() + "." + PixelPtr[0].ToString();
						// Find if exists the Department associated with the detected color
						if (DepartmentList.TryGetValue(Color, out Dept))
						{
							Rect.X = X;
							Dept.GraphicObject.AddRectangle(Rect);
						}
					}
				}
				// Unlock the Bitmap
				Map.UnlockBits(bmpData);
			}
		}

		/// <summary>
		/// Update the color of each Department object having one or more ServicePoint.
		/// </summary>
		/// <param name="Map"></param>
		private void UpdateMapColor(Bitmap Map, int HueOffset)
		{
			List<string> KeyList = new List<string>();
			Graphics g = Graphics.FromImage(Map);
			
			// Find all pair <key, value> in the dictionary to change and update the Map
			foreach (KeyValuePair<string, Department> Item in DepartmentList)
			{
				if (Program.ServicePointManager.IsPopulate(Item.Value.Number))
				{
					// Convert formatted color to Color type
					string[] ARGB = Item.Value.ColorID.Split('.');
					Color NewColor = Color.FromArgb(Convert.ToInt32(ARGB[0]), Convert.ToInt32(ARGB[1]), Convert.ToInt32(ARGB[2]), Convert.ToInt32(ARGB[3]));
					// Convert ARGB in HSL
					HSLColor HSL = ColorEx.ToHSL(NewColor);
					// Change Hue and convert back in ARGB
					NewColor = ColorEx.FromHSV(NewColor, (HSL.H + HueOffset) % 360, HSL.S, HSL.L);
					// Store the new colorID
					Item.Value.ColorID = NewColor.A.ToString() + "." + NewColor.R.ToString() + "." + NewColor.G.ToString() + "." + NewColor.B.ToString();
					
					// Store key because it must be change in the dictonary
					// This operation is done separetly because it is not possible to make changes in a foreach loop using dictionary as source
					KeyList.Add(Item.Key);
						
					// Fill-in the map with the new color
					Brush BrushColor = new SolidBrush(NewColor);
					g.FillPath(BrushColor, Item.Value.GraphicObject);
				}
			}

			g.Dispose();

			Department department;
			// Now update the Dictonary keys when the colorID has been changed
			foreach (string Key in KeyList)
			{
				// Extract and remove the Department object from the dictonary
				DepartmentList.TryGetValue(Key, out department);
				DepartmentList.Remove(Key);
				// Add it again with the new key
				DepartmentList.Add(department.ColorID, department);
			}
		}

		/// <summary>
		/// Convert a Color object in a formatted string object (using this template: "A.R.G.B")
		/// </summary>
		/// <param name="Color"></param>
		/// <returns></returns>
		private string StringColorFromColor(ref Color Color)
		{
			return Color.A.ToString() + "." + Color.R.ToString() + "." + Color.G.ToString() + "." + Color.B.ToString();
		}

		/// <summary>
		/// Send a string to all object which subscribe this event.
		/// </summary>
		/// <param name="text"></param>
		public void SendText(string text)
		{
			this.SendTextEvent(text);
		}

		/// <summary>
		/// Open default email client with a preformated email
		/// </summary>
		/// <param name="emailTo"></param>
		/// <returns></returns>
		public bool SendEmail(string emailTo)
		{
			bool result = true;

			string commandText = String.Format("mailto:{0}?cc={1}&subject={2}&body={3}", emailTo, Properties.Settings.Default.Manager_Email, Properties.Settings.Default.Email_Subject, Properties.Settings.Default.Email_Body);
			System.Diagnostics.Process emailProcess = new System.Diagnostics.Process();
			if (emailProcess != null)
			{
				emailProcess.StartInfo.FileName = commandText;
				emailProcess.StartInfo.UseShellExecute = true;
				emailProcess.Start();
				emailProcess.Close();

				result = true;
				Log.WriteLine("...Request default email client to send email to {0}", emailTo);
			}
			else
			{
				result = false;
				Log.WriteLine("...WARNING: application is not able to open default email client");
			}

			return result;
		}


		private static void CreateXmlSchema(string InputUri, string Path)
		{
			System.Xml.XmlReader oXmlReader = System.Xml.XmlReader.Create(InputUri);
			System.Xml.Schema.XmlSchemaSet oXmlSchemaSet = new System.Xml.Schema.XmlSchemaSet();
			System.Xml.Schema.XmlSchemaInference oXmlSchemaInference = new System.Xml.Schema.XmlSchemaInference();
			oXmlSchemaSet = oXmlSchemaInference.InferSchema(oXmlReader);
			System.IO.TextWriter red = new System.IO.StreamWriter(Path);
			foreach (System.Xml.Schema.XmlSchema oXmlSchema in oXmlSchemaSet.Schemas())
			{
				oXmlSchema.Write(red);
			}
		}
		#endregion
	}
}
