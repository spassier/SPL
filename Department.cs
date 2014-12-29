/// <summary>
///	Department class
/// 
/// 2010/07/25
/// Copyright (c) 2010 - Sebastien PASSIER, All rights reserved
/// </summary>

//using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
//using System.Text;

namespace SPL
{
	class Department
	{
		public string Number;
		public string Name;
		public string ColorID;
		public Point FlagLocation;
		public GraphicsPath GraphicObject;
		public List<Connection> ConnectionList;

		#region Constructors
		public Department()
		{
			Number = string.Empty;
			Name = string.Empty;
			ColorID = string.Empty;
			FlagLocation = new Point(0, 0);
			GraphicObject = new GraphicsPath();
			ConnectionList = new List<Connection>();
		}

		public Department(string number, string name, string colorID, Point flagLocation)
		{
			Number = number;
			Name = name;
			ColorID = colorID;
			FlagLocation = flagLocation;
			GraphicObject = new GraphicsPath();
			ConnectionList = new List<Connection>();
		}
		#endregion
	}
}
