/// <summary>
///	MapControl class
/// 
/// 2010/06/17
/// Copyright (c) 2010 - Sebastien PASSIER, All rights reserved
/// </summary>

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace SPL
{
	public partial class MapControl : UserControl
	{
		#region Consts
		private readonly Color COLOR_AREA_SELECTED = Color.FromArgb(255, 220, 0, 0);
		private readonly Color COLOR_AREA_UNSELECTED = Color.FromArgb(0, 0, 0, 0);
		
		private const string UI_DEFAULT_MAP_FILENAME = "FranceMap.png";
		#endregion

		#region Variables
		private Brush m_BrushFlag;
		private Font m_FontFlag;

		private GraphicsPath m_GraphicsPath;

		private Color m_PreviousColor;
		//private Color m_SelectedColor;

		private Brush m_BrushAreaSelected;

		private Point m_MouseOrigin;
		private Point m_CanvasOrigin;

		private bool m_IsMoving;

		private List<Point> m_FlagLocationList;
		private StringCollection m_FlagStringList;
		#endregion

		#region Properties
		private Bitmap m_Map = null;
		public Bitmap Map
		{
			get { return m_Map; }
			set
			{
				m_Map = value;
				if (m_Map != null)
					this.MaximumSize = new Size(this.m_Map.Width, this.m_Map.Height);
				else
					this.MaximumSize = new Size(0, 0);
			}
		}
		private Bitmap m_Flag = null;
		public Bitmap Flag
		{
			get { return m_Flag; }
			set { m_Flag = value; }
		}
		#endregion

		#region Delegates & Events
		public delegate void UnselectionEventHandler();
		public event UnselectionEventHandler UnselectionEvent;

		public delegate void SelectionEventHandler(Color ColorID);
		public event SelectionEventHandler SelectionEvent;
		#endregion

		#region Constructor
		public MapControl()
		{
			InitializeComponent();

			// Set needed style options
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);

			//this.SetStyle(ControlStyles.ResizeRedraw, true);
			this.SetStyle(ControlStyles.Selectable, true);
			//this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			this.SetStyle(ControlStyles.UserMouse, true);

			// Load default bitmap
			Assembly Assembly = Assembly.GetExecutingAssembly();
			Stream Stream = Assembly.GetManifestResourceStream("SPL.Resources." + UI_DEFAULT_MAP_FILENAME);
			if (this.Map != null)
				this.Map.Dispose();
			this.Map = new Bitmap(Stream);
			Stream.Close();

			// Get data from UiData class
			// FIXME: this is a trash solution. For example if the Map is dynamically change those data will be wrong
			// FIXME: could be call in Load event ?
			m_FlagLocationList = Program.ApplicationCore.GetFlagLocation();
			m_FlagStringList = Program.ApplicationCore.GetFlagString();

			m_FontFlag = new Font("Segoe UI", 10.0F, FontStyle.Bold);
			m_BrushFlag = new SolidBrush(Color.Black);

			m_GraphicsPath = null;

			m_PreviousColor = COLOR_AREA_UNSELECTED;
			//m_SelectedColor = COLOR_AREA_UNSELECTED;
			m_BrushAreaSelected = new SolidBrush(COLOR_AREA_SELECTED);
			
			m_MouseOrigin = new Point(0, 0);
			m_CanvasOrigin = new Point(0, 0);

			m_IsMoving = false;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Adjust the size of the control. The control is automatically redraw
		/// </summary>
		/// <param name="Offset"></param>
		public void Resizing(Size Offset)
		{
			// Adjust the size of the control
			this.Width += Offset.Width;
			this.Height += Offset.Height;
			// Redraw the control
			this.Invalidate();
		}

		/// <summary>
		/// 
		/// </summary>
		public void Clear()
		{
			// Restore the previous color only if something was selected
			if (!m_PreviousColor.Equals(COLOR_AREA_UNSELECTED))
			{
				if (m_GraphicsPath != null)
				{
					Graphics g = Graphics.FromImage(this.Map);

					Brush BrushPrevious = new SolidBrush(m_PreviousColor);
					g.FillPath(BrushPrevious, m_GraphicsPath);
					BrushPrevious.Dispose();

					m_PreviousColor = COLOR_AREA_UNSELECTED;

					g.Dispose();

					this.Invalidate();
				}
			}
		}

		/// <summary>
		/// Draw the control
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MapControl_Paint(object sender, PaintEventArgs e)
		{
			Bitmap bmp = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
			Graphics g = Graphics.FromImage(bmp);

			// Set rendering options
			g.InterpolationMode = InterpolationMode.Default;
			g.PixelOffsetMode = PixelOffsetMode.None;
			g.SmoothingMode = SmoothingMode.None;

			// Draw stuff here
			if (this.Map != null)
			{
				Rectangle destRect = new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height);
				Rectangle srcDest = GetCanvasBounds();
				g.DrawImage(this.Map, destRect, srcDest, GraphicsUnit.Pixel);
			}

			// Draw flag and department number on the fly
			if (this.Flag != null)
			{
				// Draw flag
				if (m_FlagLocationList != null)
				{
					foreach (Point Item in m_FlagLocationList)
						g.DrawImage(this.Flag, Item.X - m_CanvasOrigin.X, Item.Y - m_CanvasOrigin.Y);
				}
				// Draw number
				if (m_FlagStringList != null)
				{
					for (int i = 0; i < m_FlagStringList.Count; i++)
						g.DrawString(m_FlagStringList[i], m_FontFlag, m_BrushFlag, m_FlagLocationList[i].X + 1 - m_CanvasOrigin.X, m_FlagLocationList[i].Y - m_CanvasOrigin.Y);
				}
			}

			// Finally draw our picture in the UserControl
			e.Graphics.DrawImage(bmp, 0, 0, this.ClientSize.Width, this.ClientSize.Height);

			// Release resources
			g.Dispose();
			bmp.Dispose();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MapControl_MouseDown(object sender, MouseEventArgs e)
		{
			// Right button down clic = start canvas moving over the Map
			if (e.Button == MouseButtons.Right)
			{
				// Get mouse starting move position
				m_MouseOrigin = e.Location;
				// The control is moving now
				m_IsMoving = true;
				// Change cursor form
				this.Cursor = Cursors.NoMove2D;
			}

			// Left button down clic = change Department color object if the color at mouse position is a Department color 
			if (e.Button == MouseButtons.Left)
			{
				Graphics g = Graphics.FromImage(this.Map);

				Color CurrentColor = this.Map.GetPixel(e.X + m_CanvasOrigin.X, e.Y + m_CanvasOrigin.Y);

				// Change nothing in case the color does not correspond to a Department (black border or background) execpt the  color used when a graphic object is selected
				if (Program.ApplicationCore.IsDepartmentColor(ref CurrentColor) || CurrentColor.Equals(COLOR_AREA_SELECTED))
				{ 
					// Case1: click again on the same Department (shown in red on the map) means restore the previous Department color
					if (CurrentColor.Equals(COLOR_AREA_SELECTED))
					{
						if (m_GraphicsPath != null)
						{
							Brush BrushPrevious = new SolidBrush(m_PreviousColor);
							g.FillPath(BrushPrevious, m_GraphicsPath);
							BrushPrevious.Dispose();

							m_PreviousColor = COLOR_AREA_UNSELECTED;

							// Fire Unselection event
							this.UnselectionEvent();
						}
					}
					else
					{
						// Case2: nothing previously selected
						if (m_PreviousColor.Equals(COLOR_AREA_UNSELECTED))
						{
							// Get the GraphicsPath according to the Department color
							GraphicsPath ResultPath = Program.ApplicationCore.GetGraphicsPathFromColor(ref CurrentColor);
							// Track the color
							m_PreviousColor = CurrentColor;
							// Draw the path in red directly into the map (no transformation matrix to apply during drawing and let the draw method simple and ligth)
							g.FillPath(m_BrushAreaSelected, ResultPath);
							// Tack the GraphicsPath
							m_GraphicsPath = ResultPath;

							// Fire Selection event
							this.SelectionEvent(CurrentColor);
						}
						else
						{
							// Case3: click on a Department whereas a previous one is selected
							
							// Restore the previous color of the previous Department
							Brush BrushPrevious = new SolidBrush(m_PreviousColor);
							g.FillPath(BrushPrevious, m_GraphicsPath);
							BrushPrevious.Dispose();
							
							// Get the GraphicsPath according to the Department color
							GraphicsPath ResultPath = Program.ApplicationCore.GetGraphicsPathFromColor(ref CurrentColor);
							// Track the color
							m_PreviousColor = CurrentColor;
							// Draw the path in red directly into the map (no transformation matrix to apply during drawing and let the draw method simple and ligth)
							g.FillPath(m_BrushAreaSelected, ResultPath);
							// Track the GraphicsPath
							m_GraphicsPath = ResultPath;

							// Fire Selection event
							this.SelectionEvent(CurrentColor);
						}
					}
				}
				
				g.Dispose();

				// Redraw the control
				this.Invalidate();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MapControl_MouseMove(object sender, MouseEventArgs e)
		{
			if (m_IsMoving)
			{
				// Compute canvas origin location
				m_CanvasOrigin.X += m_MouseOrigin.X - e.X;
				m_CanvasOrigin.Y += m_MouseOrigin.Y - e.Y;
				// Clip it, the canvas must be in the bounds of the Map size
				ClipCanvasOrigin();
				// Track mouse move
				m_MouseOrigin = e.Location;

				// Redraw the control
				this.Invalidate();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MapControl_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				// The control stop moving
				m_IsMoving = false;

				// Restore the cursor
				this.Cursor = Cursors.Default;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MapControl_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			// Validate a selection only when the user is not moving the map
			if (m_IsMoving == false)
			{
				//Color color = this.Map.GetPixel(e.X + m_MapOrigin.X, e.Y + m_MapOrigin.Y);
				//Log.WriteLine("Color = {0}", color.ToString());
			}
		}

		/// <summary>
		/// Clip the canvas origin. The canvas area must stay in the bounds of the Map
		/// </summary>
		private void ClipCanvasOrigin()
		{
			if (m_CanvasOrigin.X < 0)
				m_CanvasOrigin.X = 0;
			else if (m_CanvasOrigin.X > this.Map.Width - this.ClientSize.Width)
				m_CanvasOrigin.X = this.Map.Width - this.ClientSize.Width;

			if (m_CanvasOrigin.Y < 0)
				m_CanvasOrigin.Y = 0;
			else if (m_CanvasOrigin.Y > this.Map.Height - this.ClientSize.Height)
				m_CanvasOrigin.Y = this.Map.Height - this.ClientSize.Height;
		}

		/// <summary>
		/// Returns the origin and size of the canvas, that is, the visual part of the Map
		/// </summary>
		/// <returns></returns>
		private Rectangle GetCanvasBounds()
		{
			return new Rectangle(m_CanvasOrigin.X, m_CanvasOrigin.Y, this.ClientSize.Width, this.ClientSize.Height);
		}

		#endregion


	}
}
