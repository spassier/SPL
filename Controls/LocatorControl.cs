/// <summary>
///	LocatorControl class
/// 
/// 2010/07/14
/// Copyright (c) 2010 - Sebastien PASSIER, All rights reserved
/// </summary>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace SPL
{
	public enum SearchCriteria { Cheapest, Nearest, Quickest };

	public partial class LocatorControl : UserControl
	{
		#region Consts
		private const int UI_BANNER_HEIGHT = 40;
		private const int UI_DEFAULT_GLYPH_WIDTH = 32;
		private const int UI_TEXT_INDENT = 8;

		private readonly Color COLOR_CONTROL_BOUNDARY = Color.FromArgb(255, 0, 0, 0);
		private readonly Color COLOR_DELIMITER = Color.FromArgb(255, 31, 31, 31);
		private readonly Color COLOR_BANNER_BORDER = Color.FromArgb(255, 92, 92, 92);
		private readonly Color COLOR_BANNER = Color.FromArgb(255, 77, 77, 77);
		private readonly Color COLOR_PANEL = Color.FromArgb(255, 56, 56, 56);

		private readonly Color COLOR_TEXTBOX_BACKGROUND = Color.FromArgb(255, 50, 50, 50);

		private readonly Color COLOR_TEXT = Color.FromArgb(255, 255, 255, 240);	// Ivory
		private readonly Color COLOR_TEXT_SHADOW = Color.FromArgb(192, 0, 0, 0);
		private readonly Color COLOR_TEXT_ERROR = Color.FromArgb(255, 255, 0, 0);

		private const string UI_SEARCH_FILENAME = "Search.png";

		private const string UI_BANNER_TEXT = "Code Postal / Ville";

		private const int DEFAULT_SEARCH_THRESHOLD = 3;
		private const int MIN_SEARCH_THRESHOLD = 1;
		private const int MAX_SEARCH_THRESHOLD = 8;
		#endregion

		#region Variables
		private Bitmap m_bmpSearch = null;
		private Pen m_Pen = null;
		private Brush m_BrushBanner = null;
		private Brush m_BrushPanel = null;
		private Brush m_BrushText = null;
		private Brush m_BrushTextError = null;
		private Brush m_BrushTextShadow = null;
		private Font m_FontText = null;

		private SizeF m_BannerTextSize;

		private string m_StatusText = null;
		private SizeF m_StatusTextSize;
		
		private bool m_IsHoverSearch = false;
		#endregion

		#region Properties
		private Bitmap m_Glyph = null;
		public Bitmap Glyph
		{
			get { return m_Glyph; }
			set { m_Glyph = value; }
		}

		private SearchCriteria m_SearchCriteria = SearchCriteria.Cheapest;
		public SearchCriteria SearchCriteria
		{
			get { return m_SearchCriteria; }
			set { m_SearchCriteria = value; }
		}

		private int m_SearchThreshold = DEFAULT_SEARCH_THRESHOLD;
		public int SearchThreshold
		{
			get { return m_SearchThreshold; }
			set 
			{ 
				if (value < MIN_SEARCH_THRESHOLD)
					m_SearchThreshold = MIN_SEARCH_THRESHOLD;
				else if (value > MAX_SEARCH_THRESHOLD)
					m_SearchThreshold = MAX_SEARCH_THRESHOLD;
				else
					m_SearchThreshold = value;
			}
		}
		#endregion

		#region Delegates & Events
		public delegate void SearchEventHandler(string postalCode);
		public SearchEventHandler SearchEvent;
		#endregion

		#region Constructor
		public LocatorControl()
		{
			InitializeComponent();

			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			//this.SetStyle(ControlStyles.ResizeRedraw, true);
			this.SetStyle(ControlStyles.Selectable, true);
			this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			this.SetStyle(ControlStyles.UserMouse, true);

			// Subscribe Log event
			Program.ApplicationCore.SendTextEvent += new CoreLayer.SendTextEventHandler(OnLogSendText);

			// Fellowing bitmap is fixed (Magnifying glass))
			Assembly Assembly = Assembly.GetExecutingAssembly();
			Stream Stream = Assembly.GetManifestResourceStream("SPL.Resources." + UI_SEARCH_FILENAME);
			if (m_bmpSearch != null)
				m_bmpSearch.Dispose();
			m_bmpSearch = new Bitmap(Stream);

			Stream.Close();

			// Graphics initialization
			m_Pen = new Pen(COLOR_CONTROL_BOUNDARY);
			m_BrushBanner = new SolidBrush(COLOR_BANNER);
			m_BrushPanel = new SolidBrush(COLOR_PANEL);
			m_BrushText = new SolidBrush(COLOR_TEXT);
			m_BrushTextShadow = new SolidBrush(COLOR_TEXT_SHADOW);
			m_BrushTextError = new SolidBrush(COLOR_TEXT_ERROR);

			m_FontText = new Font("Segoe UI", 8.0F, FontStyle.Bold);

			edtSearch.BackColor = COLOR_TEXTBOX_BACKGROUND;
			edtSearch.Font = new Font("Segoe UI", 11.0F, FontStyle.Regular);
			edtSearch.ForeColor = COLOR_TEXT;
			edtSearch.Top = (UI_BANNER_HEIGHT - edtSearch.Height + 2) / 2;
			int bmpWidth = 0;
			if (m_bmpSearch != null)
				bmpWidth = m_bmpSearch.Width;
			edtSearch.Left = this.Width - edtSearch.Width - bmpWidth - UI_TEXT_INDENT;

			Graphics g = this.CreateGraphics();
			m_BannerTextSize = g.MeasureString(UI_BANNER_TEXT, m_FontText);
			g.Dispose();

			// Settings initialization
			this.SearchThreshold = Properties.Settings.Default.MIN_SERVICEPOINT_FOUND;
		}
		#endregion

		#region Methods
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LocatorControl_Paint(object sender, PaintEventArgs e)
		{
			Rectangle destRect;
			Bitmap bmp = new Bitmap(this.Width, this.Height);
			Graphics g = Graphics.FromImage(bmp);

			// Set a good rendering hint for text
			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

			// Draw a boundary around the control
			m_Pen.Color = COLOR_CONTROL_BOUNDARY;
			g.DrawRectangle(m_Pen, 0, 0, this.Width - 1, this.Height - 1);

			// Draw banner
			m_Pen.Color = COLOR_BANNER_BORDER;
			g.DrawRectangle(m_Pen, 1, 1, this.Width - 3, UI_BANNER_HEIGHT - 1);
			g.FillRectangle(m_BrushBanner, 2, 2, this.Width - 4, UI_BANNER_HEIGHT - 2);

			// Draw delimiters (on the top and the bottom of the panel)
			m_Pen.Color = COLOR_DELIMITER;
			g.DrawLine(m_Pen, 1, UI_BANNER_HEIGHT + 2, this.Width - 2, UI_BANNER_HEIGHT + 2);
			g.DrawLine(m_Pen, 1, this.Height - 2, this.Width - 2, this.Height - 2);

			// Draw panel
			g.FillRectangle(m_BrushPanel, 1, UI_BANNER_HEIGHT + 2 + 1, this.Width - 2, this.Height - UI_BANNER_HEIGHT - 2 - 3);

			// Draw Glyph
			if (this.Glyph != null)
			{
				destRect = new Rectangle((UI_BANNER_HEIGHT - this.Glyph.Height) / 2, (UI_BANNER_HEIGHT - this.Glyph.Height) / 2, this.Glyph.Width, this.Glyph.Height);
				g.DrawImage(this.Glyph, destRect, 0, 0, this.Glyph.Width, this.Glyph.Height, GraphicsUnit.Pixel);
			}

			// Makup a little the TextBox Control
			// Draw a border around
			m_Pen.Color = COLOR_CONTROL_BOUNDARY;
			g.DrawRectangle(m_Pen, edtSearch.Left - 1, edtSearch.Top - 1, edtSearch.Width + 1, edtSearch.Height + 1);
			// Draw Search bitmap on the right of the TextBox 
			destRect = new Rectangle(edtSearch.Left + edtSearch.Width, edtSearch.Top - 1, m_bmpSearch.Width, m_bmpSearch.Height);
			g.DrawImage(m_bmpSearch, destRect, 0, 0, m_bmpSearch.Width, m_bmpSearch.Height, GraphicsUnit.Pixel);
			
			// Draw text
			if (this.Glyph != null)
			{
				g.DrawString(UI_BANNER_TEXT, m_FontText, m_BrushTextShadow, this.Glyph.Width + (UI_BANNER_HEIGHT - this.Glyph.Height), (UI_BANNER_HEIGHT - m_BannerTextSize.Height) / 2 + 1);
				g.DrawString(UI_BANNER_TEXT, m_FontText, m_BrushText, this.Glyph.Width + (UI_BANNER_HEIGHT - this.Glyph.Height), (UI_BANNER_HEIGHT - m_BannerTextSize.Height) / 2);
			}
			else
			{
				g.DrawString(UI_BANNER_TEXT, m_FontText, m_BrushTextShadow, UI_DEFAULT_GLYPH_WIDTH + (UI_BANNER_HEIGHT - UI_DEFAULT_GLYPH_WIDTH), (UI_BANNER_HEIGHT - m_BannerTextSize.Height) / 2 + 1);
				g.DrawString(UI_BANNER_TEXT, m_FontText, m_BrushText, UI_DEFAULT_GLYPH_WIDTH + (UI_BANNER_HEIGHT - UI_DEFAULT_GLYPH_WIDTH), (UI_BANNER_HEIGHT - m_BannerTextSize.Height) / 2);
			}

			// Draw status information
			if (!String.IsNullOrEmpty(m_StatusText))
			{
				string flag = m_StatusText.Substring(m_StatusText.Length - 2);
				m_StatusText = m_StatusText.Remove(m_StatusText.Length - 2);	// Remove the flag string
				switch (flag)
				{
					case CoreLayer.UI_TEXT_ERROR:
						g.DrawString(m_StatusText, m_FontText, m_BrushTextError, UI_TEXT_INDENT, UI_BANNER_HEIGHT + 2 + 1 + (this.Height - UI_BANNER_HEIGHT - 2 - 3 - m_StatusTextSize.Height) / 2);
						break;
					case CoreLayer.UI_TEXT_WARNING:
						g.DrawString(m_StatusText, m_FontText, m_BrushTextError, UI_TEXT_INDENT, UI_BANNER_HEIGHT + 2 + 1 + (this.Height - UI_BANNER_HEIGHT - 2 - 3 - m_StatusTextSize.Height) / 2);
						break;
					case CoreLayer.UI_TEXT_NORMAL:
					default:
						g.DrawString(m_StatusText, m_FontText, m_BrushText, UI_TEXT_INDENT, UI_BANNER_HEIGHT + 2 + 1 + (this.Height - UI_BANNER_HEIGHT - 2 - 3 - m_StatusTextSize.Height) / 2);
						break;
				}
			}

			// Finally draw our picture in the UserControl
			e.Graphics.DrawImage(bmp, 0, 0, this.Width, this.Height);

			// Release resources
			g.Dispose();
			bmp.Dispose();
		}

		/// <summary>
		/// Erase message when leaving the control
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LocatorControl_Leave(object sender, EventArgs e)
		{
			m_StatusText = String.Empty;	// Clear the status meesage
			edtSearch.Clear();						// Clear text from the edit box
			this.Invalidate();
		}

		/// <summary>
		/// Handle cursor form when mouse is hover the "Search" bitmap
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LocatorControl_MouseMove(object sender, MouseEventArgs e)
		{
			// Check if hover the "Search" bitmap
			Rectangle Bounds = new Rectangle(edtSearch.Left + edtSearch.Width, edtSearch.Top - 1, m_bmpSearch.Width, m_bmpSearch.Height);
			if (Bounds.Contains(e.Location))
			{
				m_IsHoverSearch = true;
				this.Cursor = Cursors.Hand;
			}
			else if (m_IsHoverSearch)
			{
				m_IsHoverSearch = false;
				this.Cursor = Cursors.Default;
			}
		}

		/// <summary>
		/// Handle mous click on the "search" bitmap
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LocatorControl_MouseDown(object sender, MouseEventArgs e)
		{
			if (m_IsHoverSearch)
				this.SearchEvent(edtSearch.Text);
		}

		/// <summary>
		/// Handle Log OnLogSendText event.
		/// This method shows messages like error in code postal, status of a search and so on
		/// </summary>
		/// <param name="text"></param>
		private void OnLogSendText(string text)
		{
			m_StatusText = text;
			
			// Compute the size of the string
			Graphics g = this.CreateGraphics();
			m_StatusTextSize = g.MeasureString(m_StatusText, m_FontText);
			g.Dispose();

			// Redraw the control
			this.Invalidate();
		}

		/// <summary>
		/// Handle textbox validation using ENTER key
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtZipCode_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				// Clear the status message to remove previous one
				m_StatusText = String.Empty;
				this.Invalidate();
				// Fire search event
				this.SearchEvent(edtSearch.Text);
			}
		}

		#endregion
	}
}
