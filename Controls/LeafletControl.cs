/// <summary> 
///	LeafletControl class
///	
/// 
/// 2010/06/21
/// Copyright (c) 2010 - Sebastien PASSIER, All rights reserved
/// </summary> 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace SPL
{
	public partial class LeafletControl : UserControl
	{
		// FIXME : change this into Struct in spite of Class object
		private class LeafletDataEntry
		{
			#region Consts

			#endregion

			#region Variables

			#endregion

			#region Properties
			private string m_Title;
			public string Title
			{
				get { return m_Title; }
				set { m_Title = value; }
			}
			private string m_Address;
			public string Address
			{
				get { return m_Address; }
				set { m_Address = value; }
			}
			private string m_Contact;
			public string Contact
			{
				get { return m_Contact; }
				set { m_Contact = value; }
			}
			private string m_Email;
			public string Email
			{
				get { return m_Email; }
				set { m_Email = value; }
			}
			private string m_Rate;
			public string Rate
			{
				get { return m_Rate; }
				set { m_Rate = value; }
			}
			private string m_Result;
			public string Result
			{
				get { return m_Result; }
				set { m_Result = value; }
			}
			private SizeF m_TitleSize;
			public SizeF TitleSize
			{
				get { return m_TitleSize; }
				set { m_TitleSize = value; }
			}
			private SizeF m_AddressSize;
			public SizeF AddressSize
			{
				get { return m_AddressSize; }
				set { m_AddressSize = value; }
			}
			private SizeF m_ContactSize;
			public SizeF ContactSize
			{
				get { return m_ContactSize; }
				set { m_ContactSize = value; }
			}
			private SizeF m_EmailSize;
			public SizeF EmailSize
			{
				get { return m_EmailSize; }
				set { m_EmailSize = value; }
			}
			private SizeF m_RateSize;
			public SizeF RateSize
			{
				get { return m_RateSize; }
				set { m_RateSize = value; }
			}
			private SizeF m_ResultSize;
			public SizeF ResultSize
			{
				get { return m_ResultSize; }
				set { m_ResultSize = value; }
			}

			private bool m_Open;
			public bool Open
			{
				get { return m_Open; }
				set { m_Open = value; }
			}

			private Point m_BannerPosition;
			public Point BannerPosition
			{
				get { return m_BannerPosition; }
				set { m_BannerPosition = value; }
			}

			private Point m_PanelPosition;
			public Point PanelPosition
			{
				get { return m_PanelPosition; }
				set { m_PanelPosition = value; }
			}

			private SizeF m_BannerSize;
			public SizeF BannerSize
			{
				get { return m_BannerSize; }
				set { m_BannerSize = value; }
			}

			private SizeF m_PanelSize;
			public SizeF PanelSize
			{
				get { return m_PanelSize; }
				set { m_PanelSize = value; }
			}
			#endregion


			#region Constructor
			public LeafletDataEntry()
			{
				this.Open = true;
			}
			#endregion

			#region Methods
			// TODO; add methods here...
			#endregion
		}

		#region Consts
		private const int UI_BANNER_HEIGHT = 40;
		private const int UI_LEAF_BANNER_HEIGHT = 40;
		private const int UI_TEXT_INTERLEAVE = 8;
		private const int UI_TEXT_INDENT = 8;
		private const int UI_ARROWDOWN_X_OFFSET = 11;
		private const int UI_ARROWRIGHT_X_OFFSET = 11 + 3;

		private readonly Color COLOR_CONTROL_BOUNDARY = Color.FromArgb(255, 0, 0, 0);
		private readonly Color COLOR_DELIMITER = Color.FromArgb(255, 31, 31, 31);
		private readonly Color COLOR_BANNER_BORDER = Color.FromArgb(255, 92, 92, 92);
		private readonly Color COLOR_BANNER = Color.FromArgb(255, 77, 77, 77);
		private readonly Color COLOR_PANEL = Color.FromArgb(255, 56, 56, 56);
		private readonly Color COLOR_LEAF_BANNER_BORDER = Color.FromArgb(255, 76, 76, 76);
		private readonly Color COLOR_LEAF_BANNER_CLOSE = Color.FromArgb(255, 62, 62, 62);
		private readonly Color COLOR_LEAF_BANNER_OPEN = Color.FromArgb(255, 43, 43, 43);
		private readonly Color COLOR_LEAF_PANEL = Color.FromArgb(255, 50, 50, 50);

		private readonly Color COLOR_LEAF_TITLE = Color.FromArgb(255, 255, 127, 50);	// Coral
		private readonly Color COLOR_TEXT = Color.FromArgb(255, 255, 255, 240);	// Ivory
		private readonly Color COLOR_EMAIL = Color.FromArgb(255, 135, 206, 250);	// Light Sky Blue
		private readonly Color COLOR_INFO = Color.Cornsilk;
		private readonly Color COLOR_RESULT = Color.FromArgb(255, 255, 0, 0);	// Red
		private readonly Color COLOR_TEXT_SHADOW = Color.FromArgb(192, 0, 0, 0);

		private readonly Color COLOR_FOCUS_DARK = Color.FromArgb(255, 92, 127, 164);
		private readonly Color COLOR_FOCUS_LIGHT = Color.FromArgb(255, 108, 178, 253);

		private const string UI_ARROWDOWN_FILENAME = "ArrowDown.png";
		private const string UI_ARROWRIGHT_FILENAME = "ArrowRight.png";
		private const string UI_EMAIL_FILENAME = "Email.png";
		private const string UI_COPYNORMAL_FILENAME = "CopyN.png";
		private const string UI_COPYHOVER_FILENAME = "CopyH.png";
		private const string UI_COPYFOCUS_FILENAME = "CopyF.png";
		#endregion

		#region Variables
		private Pen m_Pen = null;
		private Brush m_BrushBanner = null;
		private Brush m_BrushPanel = null;
		private Brush m_BrushLeafBannerClose = null;
		private Brush m_BrushLeafBannerOpen = null;
		private Brush m_BrushLeafPanel = null;

		private Brush m_BrushLeafTitle = null;
		private Brush m_BrushText = null;
		private Brush m_BrushEmail = null;
		private Brush m_BrushInfo = null;
		private Brush m_BrushResult = null;
		private Brush m_BrushTextShadow = null;

		private SizeF m_TitleSize;
		
		private Font m_FontTitle = null;
		private Font m_FontLeafTitle = null;
		private Font m_FontText = null;
		private Font m_FontEmail = null;
		private Font m_FontResult = null;

		private Bitmap m_bmpArrowRight = null;
		private Bitmap m_bmpArrowDown = null;
		private Bitmap m_bmpEmail = null;
		private Bitmap m_bmpCopyNormal = null;
		private Bitmap m_bmpCopyHover = null;
		private Bitmap m_bmpCopyFocus = null;

		private List<LeafletDataEntry> LeafList = null;

		private bool m_IsHoverLink = false;
		private bool m_IsHoverCopy = false;
		private bool m_IsFocusCopy = false;
		#endregion

		#region Properties
		private string m_Title;
		public string Title
		{
			get { return m_Title; }
			set
			{
				m_Title = value;

				// compute the size of the title each time it changed
				Graphics g = this.CreateGraphics();
				m_TitleSize = g.MeasureString(m_Title, m_FontTitle);
				g.Dispose();
			}
		}
		private Bitmap m_Glyph = null;
		public Bitmap Glyph
		{
			get { return m_Glyph; }
			set { m_Glyph = value; }
		}
		#endregion

		#region Delegates & Events
		public delegate void ClipboardSetTextHandler(string Text);
		public event ClipboardSetTextHandler ClipboardSetTextEvent;
		#endregion

		#region Constructor
		public LeafletControl()
		{
			InitializeComponent();

			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			//this.SetStyle(ControlStyles.ResizeRedraw, true);
			this.SetStyle(ControlStyles.Selectable, true);
			this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			this.SetStyle(ControlStyles.UserMouse, true);

			this.Title = String.Empty;

			m_Pen = new Pen(COLOR_CONTROL_BOUNDARY);
			m_BrushBanner = new SolidBrush(COLOR_BANNER);
			m_BrushPanel = new SolidBrush(COLOR_PANEL);
			m_BrushLeafBannerClose = new SolidBrush(COLOR_LEAF_BANNER_CLOSE);
			m_BrushLeafBannerOpen = new SolidBrush(COLOR_LEAF_BANNER_OPEN);
			m_BrushLeafPanel = new SolidBrush(COLOR_LEAF_PANEL);
			m_BrushLeafTitle = new SolidBrush(COLOR_LEAF_TITLE);

			m_BrushLeafTitle = new SolidBrush(COLOR_LEAF_TITLE);
			m_BrushText = new SolidBrush(COLOR_TEXT);
			m_BrushEmail = new SolidBrush(COLOR_EMAIL);
			m_BrushInfo = new SolidBrush(COLOR_INFO);
			m_BrushResult = new SolidBrush(COLOR_RESULT);
			m_BrushTextShadow = new SolidBrush(COLOR_TEXT_SHADOW);

			m_FontTitle = new Font("Bodoni", 10.5F, FontStyle.Bold);
			m_FontLeafTitle = new Font("Segoe UI", 8, FontStyle.Bold);
			m_FontText = new Font("Segoe UI", 8, FontStyle.Regular);
			m_FontEmail = new Font("Segoe UI", 8, FontStyle.Underline);
			m_FontResult = new Font("Segoe UI", 9, FontStyle.Bold);

			// Fellowing bitmaps are fixed (Arrow, eMail and Copy glyphs)
			Assembly Assembly = Assembly.GetExecutingAssembly();
			Stream Stream = Assembly.GetManifestResourceStream("SPL.Resources." + UI_ARROWDOWN_FILENAME);
			if (m_bmpArrowDown != null)
				m_bmpArrowDown.Dispose();
			m_bmpArrowDown = new Bitmap(Stream);

			Stream = Assembly.GetManifestResourceStream("SPL.Resources." + UI_ARROWRIGHT_FILENAME);
			if (m_bmpArrowRight != null)
				m_bmpArrowRight.Dispose();
			m_bmpArrowRight = new Bitmap(Stream);

			Stream = Assembly.GetManifestResourceStream("SPL.Resources." + UI_EMAIL_FILENAME);
			if (m_bmpEmail != null)
			  m_bmpEmail.Dispose();
			m_bmpEmail = new Bitmap(Stream);

			Stream = Assembly.GetManifestResourceStream("SPL.Resources." + UI_COPYNORMAL_FILENAME);
			if (m_bmpCopyNormal != null)
				m_bmpCopyNormal.Dispose();
			m_bmpCopyNormal = new Bitmap(Stream);

			Stream = Assembly.GetManifestResourceStream("SPL.Resources." + UI_COPYHOVER_FILENAME);
			if (m_bmpCopyHover != null)
				m_bmpCopyHover.Dispose();
			m_bmpCopyHover = new Bitmap(Stream);

			Stream = Assembly.GetManifestResourceStream("SPL.Resources." + UI_COPYFOCUS_FILENAME);
			if (m_bmpCopyFocus != null)
				m_bmpCopyFocus.Dispose();
			m_bmpCopyFocus = new Bitmap(Stream);

			Stream.Close();

			LeafList = new List<LeafletDataEntry>();

			m_IsHoverLink = false;
			m_IsHoverCopy = false;
			m_IsFocusCopy = false;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Add an element in the control
		/// </summary>
		/// <param name="Title"></param>
		/// <param name="Address"></param>
		/// <param name="Contact"></param>
		/// <param name="Email"></param>
		/// <param name="Miscs"></param>
		public void Add(string Title, string Address, string Contact, string Email, string Rate, string Result)
		{
			LeafletDataEntry NewLeaf = new LeafletDataEntry();
			
			// Fill-in values
			NewLeaf.Title = Title;
			NewLeaf.Address = Address;
			NewLeaf.Contact = Contact;
			NewLeaf.Email = Email;
			NewLeaf.Rate = Rate;
			NewLeaf.Result = Result;

			// Compute the size of each string
			Graphics g = this.CreateGraphics();
			//g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
			NewLeaf.TitleSize = g.MeasureString(Title, m_FontLeafTitle);
			NewLeaf.AddressSize = g.MeasureString(Address, m_FontText);
			NewLeaf.ContactSize = g.MeasureString(Contact, m_FontText);
			NewLeaf.EmailSize = g.MeasureString(Email, m_FontEmail);
			NewLeaf.RateSize = g.MeasureString(Rate, m_FontText);
			NewLeaf.ResultSize = g.MeasureString(Result, m_FontResult);
			g.Dispose();

			// By default only the first of the list is open
			if (LeafList.Count != 0)
				NewLeaf.Open = false;
			else
				NewLeaf.Open = true;

			// Initialize the banner and panel position & size
			NewLeaf.BannerPosition = new Point(1, UI_BANNER_HEIGHT + 2 + (UI_LEAF_BANNER_HEIGHT + 2) * LeafList.Count);
			NewLeaf.BannerSize = new SizeF(this.ClientSize.Width - 2, UI_LEAF_BANNER_HEIGHT);
			NewLeaf.PanelPosition = new Point(NewLeaf.BannerPosition.X, NewLeaf.BannerPosition.Y + UI_LEAF_BANNER_HEIGHT + 1);
			NewLeaf.PanelSize = new SizeF(this.ClientSize.Width - 2, UI_TEXT_INTERLEAVE * 5 + NewLeaf.TitleSize.Height + NewLeaf.AddressSize.Height + NewLeaf.ContactSize.Height + NewLeaf.EmailSize.Height + NewLeaf.RateSize.Height + NewLeaf.ResultSize.Height);
			
			LeafList.Add(NewLeaf);
		}

		/// <summary>
		/// Remove all elements in the control (the control.Title is set to an empty string).
		/// Redraw the control when refresh is set to true.
		/// </summary>
		public void Clear(bool refresh = false)
		{
			this.Title = String.Empty;
			LeafList.Clear();

			if (refresh)
				this.Invalidate();
		}

		/// <summary>
		/// Adjust the size of the control. The control is automatically redraw
		/// </summary>
		/// <remarks>
		/// Only the heigth is resized
		/// </remarks>
		/// <param name="Offset"></param>
		public void Resizing(Size Offset)
		{
			// Only the height should be adjusted
			this.Height += Offset.Height;
			// Redraw the control
			this.Invalidate();
		}

		/// <summary>
		/// Draw the control
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LeafletControl_Paint(object sender, PaintEventArgs e)
		{
			// Initialize our canvas
			Rectangle destRect;
			Bitmap bmp = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
			Graphics g = Graphics.FromImage(bmp);
			
			// Set a good rendering hint for text
			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

			// Draw a boundary around the control
			m_Pen.Color = COLOR_CONTROL_BOUNDARY;
			g.DrawRectangle(m_Pen, 0, 0, this.ClientSize.Width - 1, this.ClientSize.Height - 1);
			
			// Draw banner
			m_Pen.Color = COLOR_BANNER_BORDER;
			g.DrawRectangle(m_Pen, 1, 1, this.ClientSize.Width - 3, UI_BANNER_HEIGHT - 1);
			g.FillRectangle(m_BrushBanner, 2, 2, this.ClientSize.Width - 4, UI_BANNER_HEIGHT - 2);
			
			// Draw delimiters (on the top and the bottom of the panel)
			m_Pen.Color = COLOR_DELIMITER;
			g.DrawLine(m_Pen, 1, UI_BANNER_HEIGHT + 2, this.ClientSize.Width - 2, UI_BANNER_HEIGHT + 2);
			g.DrawLine(m_Pen, 1, this.ClientSize.Height - 2, this.ClientSize.Width - 2, this.ClientSize.Height - 2);
			
			// Draw panel
			g.FillRectangle(m_BrushPanel, 1, UI_BANNER_HEIGHT + 2 + 1, this.ClientSize.Width - 2, this.ClientSize.Height - UI_BANNER_HEIGHT - 2 - 3);

			// Draw Glyph
			if (this.Glyph != null)
			{
				destRect = new Rectangle((UI_BANNER_HEIGHT - this.Glyph.Height) / 2, (UI_BANNER_HEIGHT - this.Glyph.Height) / 2, this.Glyph.Width, this.Glyph.Height);
				g.DrawImage(this.Glyph, destRect, 0, 0, this.Glyph.Width, this.Glyph.Height, GraphicsUnit.Pixel);
			}

			// Draw text banner
			if (this.Title != String.Empty)
			{
				if (this.Glyph != null)
				{
					g.DrawString(this.Title, m_FontTitle, m_BrushTextShadow, this.Glyph.Width + (UI_BANNER_HEIGHT - this.Glyph.Height) / 2 + UI_TEXT_INDENT, (UI_BANNER_HEIGHT - m_TitleSize.Height) / 2 + 1);
					g.DrawString(this.Title, m_FontTitle, m_BrushText, this.Glyph.Width + (UI_BANNER_HEIGHT - this.Glyph.Height) / 2 + UI_TEXT_INDENT, (UI_BANNER_HEIGHT - m_TitleSize.Height) / 2);
				}
				else
				{
					g.DrawString(this.Title, m_FontTitle, m_BrushTextShadow, UI_TEXT_INDENT, (UI_BANNER_HEIGHT - m_TitleSize.Height) / 2 + 1);
					g.DrawString(this.Title, m_FontTitle, m_BrushText, UI_TEXT_INDENT, (UI_BANNER_HEIGHT - m_TitleSize.Height) / 2);
				}
			}

			// Draw each leaf
			float VerticalOffset = 0.0F;
			foreach (LeafletDataEntry Leaf in LeafList)
			{
				if (Leaf.Open)
				{
					// Draw banner
					g.FillRectangle(m_BrushLeafBannerOpen, Leaf.BannerPosition.X, Leaf.BannerPosition.Y, Leaf.BannerSize.Width, Leaf.BannerSize.Height);
					// Draw a delimiter
					m_Pen.Color = COLOR_DELIMITER;
					g.DrawLine(m_Pen, Leaf.BannerPosition.X, Leaf.BannerPosition.Y + Leaf.BannerSize.Height, Leaf.BannerSize.Width, Leaf.BannerPosition.Y + Leaf.BannerSize.Height);
					// Draw panel
					g.FillRectangle(m_BrushLeafPanel, Leaf.PanelPosition.X, Leaf.PanelPosition.Y , Leaf.PanelSize.Width, Leaf.PanelSize.Height);
					// Draw a delimiter
					m_Pen.Color = COLOR_DELIMITER;
					g.DrawLine(m_Pen, Leaf.PanelPosition.X, Leaf.PanelPosition.Y + Leaf.PanelSize.Height, Leaf.PanelSize.Width, Leaf.PanelPosition.Y + Leaf.PanelSize.Height);
					
					// Draw arrow bitmap
					if (m_bmpArrowDown != null)
					{
						destRect = new Rectangle(Leaf.BannerPosition.X + UI_ARROWDOWN_X_OFFSET, Leaf.BannerPosition.Y + (int)(Leaf.BannerSize.Height - m_bmpArrowDown.Height) / 2, m_bmpArrowDown.Width, m_bmpArrowDown.Height);
						g.DrawImage(m_bmpArrowDown, destRect, 0, 0, m_bmpArrowDown.Width, m_bmpArrowDown.Height, GraphicsUnit.Pixel);
					}

					// Draw text banner
					if (Leaf.Title != String.Empty)
					{
						g.DrawString(Leaf.Title, m_FontLeafTitle, m_BrushTextShadow, UI_TEXT_INDENT * 4, Leaf.BannerPosition.Y + (Leaf.BannerSize.Height - Leaf.TitleSize.Height) / 2);
						g.DrawString(Leaf.Title, m_FontLeafTitle, m_BrushLeafTitle, UI_TEXT_INDENT * 4, Leaf.BannerPosition.Y - 1 + (Leaf.BannerSize.Height - Leaf.TitleSize.Height) / 2);
					}

					// Draw text panel
					float OffsetY = UI_TEXT_INTERLEAVE;

					if (Leaf.Address != String.Empty)
					{
						g.DrawString(Leaf.Address, m_FontText, m_BrushTextShadow, UI_TEXT_INDENT, Leaf.PanelPosition.Y + OffsetY + UI_TEXT_INTERLEAVE);
						g.DrawString(Leaf.Address, m_FontText, m_BrushText, UI_TEXT_INDENT, Leaf.PanelPosition.Y - 1 + OffsetY + UI_TEXT_INTERLEAVE);
					}

					OffsetY += Leaf.AddressSize.Height + UI_TEXT_INTERLEAVE;

					if (Leaf.Contact != String.Empty)
					{
						g.DrawString(Leaf.Contact, m_FontText, m_BrushTextShadow, UI_TEXT_INDENT, Leaf.PanelPosition.Y + OffsetY + UI_TEXT_INTERLEAVE);
						g.DrawString(Leaf.Contact, m_FontText, m_BrushText, UI_TEXT_INDENT, Leaf.PanelPosition.Y - 1 + OffsetY + UI_TEXT_INTERLEAVE);
					}

					OffsetY += Leaf.ContactSize.Height + UI_TEXT_INTERLEAVE;

					if (Leaf.Email != String.Empty)
					{
						// Draw email glyph before (WARNING: hard decrease the Y position of the email glyph of 1 pixel to center its position)
						if (m_bmpEmail != null)
						{
							destRect = new Rectangle(UI_TEXT_INDENT, Leaf.PanelPosition.Y + (int)OffsetY + UI_TEXT_INTERLEAVE - 1, m_bmpEmail.Width, m_bmpEmail.Height);
							g.DrawImage(m_bmpEmail, destRect, 0, 0, m_bmpEmail.Width, m_bmpEmail.Height, GraphicsUnit.Pixel);

							g.DrawString(Leaf.Email, m_FontText, m_BrushTextShadow, UI_TEXT_INDENT + m_bmpEmail.Width, Leaf.PanelPosition.Y + OffsetY + UI_TEXT_INTERLEAVE);
							g.DrawString(Leaf.Email, m_FontText, m_BrushEmail, UI_TEXT_INDENT + m_bmpEmail.Width, Leaf.PanelPosition.Y - 1 + OffsetY + UI_TEXT_INTERLEAVE);
						}
						else
						{
							g.DrawString(Leaf.Email, m_FontText, m_BrushTextShadow, UI_TEXT_INDENT, Leaf.PanelPosition.Y + OffsetY + UI_TEXT_INTERLEAVE);
							g.DrawString(Leaf.Email, m_FontText, m_BrushEmail, UI_TEXT_INDENT, Leaf.PanelPosition.Y - 1 + OffsetY + UI_TEXT_INTERLEAVE);
						}
					}

					OffsetY += Leaf.EmailSize.Height + UI_TEXT_INTERLEAVE;

					if (Leaf.Rate != String.Empty)
					{
						g.DrawString(Leaf.Rate, m_FontText, m_BrushTextShadow, UI_TEXT_INDENT, Leaf.PanelPosition.Y + OffsetY + UI_TEXT_INTERLEAVE);
						g.DrawString(Leaf.Rate, m_FontText, m_BrushInfo, UI_TEXT_INDENT, Leaf.PanelPosition.Y - 1 + OffsetY + UI_TEXT_INTERLEAVE);
					}

					OffsetY += Leaf.RateSize.Height + UI_TEXT_INTERLEAVE;

					if (Leaf.Result != String.Empty)
					{
						g.DrawString(Leaf.Result, m_FontResult, m_BrushTextShadow, UI_TEXT_INDENT, Leaf.PanelPosition.Y + OffsetY + UI_TEXT_INTERLEAVE);
						g.DrawString(Leaf.Result, m_FontResult, m_BrushResult, UI_TEXT_INDENT, Leaf.PanelPosition.Y - 1 + OffsetY + UI_TEXT_INTERLEAVE);
					}

					// Draw copy button
					if (m_IsFocusCopy)
					{
						if (m_bmpCopyFocus != null)
						{
							destRect = new Rectangle((int)Leaf.PanelSize.Width - m_bmpCopyNormal.Width + 1, Leaf.PanelPosition.Y + (int)Leaf.PanelSize.Height - m_bmpCopyFocus.Height, m_bmpCopyFocus.Width, m_bmpCopyFocus.Height);
							g.DrawImage(m_bmpCopyFocus, destRect, 0, 0, m_bmpCopyFocus.Width, m_bmpCopyFocus.Height, GraphicsUnit.Pixel);
						}
					} 
					else if (m_IsHoverCopy)
					{
						if (m_bmpCopyHover != null)
						{
							destRect = new Rectangle((int)Leaf.PanelSize.Width - m_bmpCopyHover.Width + 1, Leaf.PanelPosition.Y + (int)Leaf.PanelSize.Height - m_bmpCopyHover.Height, m_bmpCopyHover.Width, m_bmpCopyHover.Height);
							g.DrawImage(m_bmpCopyHover, destRect, 0, 0, m_bmpCopyHover.Width, m_bmpCopyHover.Height, GraphicsUnit.Pixel);
						}
					}
					else	if (m_bmpCopyNormal != null)
					{
						destRect = new Rectangle((int)Leaf.PanelSize.Width - m_bmpCopyNormal.Width + 1, Leaf.PanelPosition.Y + (int)Leaf.PanelSize.Height - m_bmpCopyNormal.Height, m_bmpCopyNormal.Width, m_bmpCopyNormal.Height);
						g.DrawImage(m_bmpCopyNormal, destRect, 0, 0, m_bmpCopyNormal.Width, m_bmpCopyNormal.Height, GraphicsUnit.Pixel);

						// Draw border if hover
						//if (m_IsHoverCopy)
						//{
						//  m_Pen.Color = COLOR_FOCUS_LIGHT;
						//  destRect.Width -= 1;
						//  destRect.Height -= 1;
						//  g.DrawRectangle(m_Pen, destRect);
						//  m_Pen.Color = COLOR_FOCUS_DARK;
						//  destRect.X -= 1;
						//  destRect.Y -= 1;
						//  destRect.Width += 2;
						//  destRect.Height += 2;
						//  g.DrawRectangle(m_Pen, destRect);
						//}
					}

					// Add a vertical offset for all the next Leaf
					VerticalOffset = Leaf.PanelSize.Height;
				}
				else
				{
					// Draw banner
					m_Pen.Color = COLOR_LEAF_BANNER_BORDER;
					g.DrawRectangle(m_Pen, Leaf.BannerPosition.X, Leaf.BannerPosition.Y + VerticalOffset, Leaf.BannerSize.Width - 1, Leaf.BannerSize.Height - 1);
					g.FillRectangle(m_BrushLeafBannerClose, Leaf.BannerPosition.X + 1, Leaf.BannerPosition.Y + VerticalOffset, Leaf.BannerSize.Width - 2, Leaf.BannerSize.Height - 2);
					// Draw a delimiter
					m_Pen.Color = COLOR_DELIMITER;
					g.DrawLine(m_Pen, Leaf.BannerPosition.X, Leaf.BannerPosition.Y + Leaf.BannerSize.Height + VerticalOffset, Leaf.BannerSize.Width, Leaf.BannerPosition.Y + Leaf.BannerSize.Height + VerticalOffset);

					// Draw arrow bitmap
					if (m_bmpArrowRight != null)
					{
						destRect = new Rectangle(Leaf.BannerPosition.X + UI_ARROWRIGHT_X_OFFSET, Leaf.BannerPosition.Y + + (int)VerticalOffset + (int)(Leaf.BannerSize.Height - m_bmpArrowRight.Height) / 2, m_bmpArrowRight.Width, m_bmpArrowRight.Height);
						g.DrawImage(m_bmpArrowRight, destRect, 0, 0, m_bmpArrowRight.Width, m_bmpArrowRight.Height, GraphicsUnit.Pixel);
					}

					// Draw text banner
					if (Leaf.Title != String.Empty)
					{
						g.DrawString(Leaf.Title, m_FontLeafTitle, m_BrushTextShadow, UI_TEXT_INDENT * 4, Leaf.BannerPosition.Y + VerticalOffset + (Leaf.BannerSize.Height - Leaf.TitleSize.Height) / 2);
						g.DrawString(Leaf.Title, m_FontLeafTitle, m_BrushLeafTitle, UI_TEXT_INDENT * 4, Leaf.BannerPosition.Y + VerticalOffset - 1 + (Leaf.BannerSize.Height - Leaf.TitleSize.Height) / 2);
					}
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
		private void LeafletControl_MouseClick(object sender, MouseEventArgs e)
		{
			Rectangle Bounds;
			int VerticalOffset = 0;

			// Check click on email link
			if (m_IsHoverLink)
			{
				LeafletDataEntry LeafOpen = LeafList.Find(IsOpen);
				if (LeafOpen != null)
				{
					if (!String.IsNullOrEmpty(LeafOpen.Email))
						Program.ApplicationCore.SendEmail(LeafOpen.Email); // Send Email to the current service point
				}
			}

			// Check click on arrow
			foreach (LeafletDataEntry Leaf in LeafList)
			{
				// Arrow test
				Bounds = new Rectangle(Leaf.BannerPosition.X + 11, Leaf.BannerPosition.Y + 14 + VerticalOffset, 11, 12);
				if (Bounds.Contains(e.Location))
				{
					if (Leaf.Open)
					{
						Leaf.Open = false;
					}
					else
					{
						LeafletDataEntry LeafOpen = LeafList.Find(IsOpen);
						if (LeafOpen != null)
							LeafOpen.Open = false;

						Leaf.Open = true;
					}
					
					// Redraw the control
					this.Invalidate();
					
					return;
				}

				// Offset next child position
				if (Leaf.Open)
					VerticalOffset = (int)Leaf.PanelSize.Height;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LeafletControl_MouseMove(object sender, MouseEventArgs e)
		{
			LeafletDataEntry LeafOpen = LeafList.Find(IsOpen);
			if (LeafOpen != null)
			{
				// Check if hover the link
				Rectangle Bounds = new Rectangle(LeafOpen.PanelPosition.X + UI_TEXT_INDENT + m_bmpEmail.Width, (int)(LeafOpen.PanelPosition.Y + LeafOpen.AddressSize.Height + LeafOpen.ContactSize.Height) + UI_TEXT_INTERLEAVE * 4, (int)LeafOpen.EmailSize.Width, (int)LeafOpen.EmailSize.Height);
				if (Bounds.Contains(e.Location))
				{
					m_IsHoverLink = true;
					this.Cursor = Cursors.Hand;
				}
				else if (m_IsHoverLink)
				{
					m_IsHoverLink = false;
					this.Cursor = Cursors.Default;
				}

				// Check if hover the copy button
				Bounds = new Rectangle((int)LeafOpen.PanelSize.Width - m_bmpCopyNormal.Width + 1, LeafOpen.PanelPosition.Y + (int)LeafOpen.PanelSize.Height - m_bmpCopyNormal.Height, m_bmpCopyNormal.Width, m_bmpCopyNormal.Height);
				if (Bounds.Contains(e.Location))
				{
					// Redraw only when the hover triggered
					if (m_IsHoverCopy == false)
						this.Invalidate();

					m_IsHoverCopy = true;
				}
				else if (m_IsHoverCopy)
				{
					m_IsHoverCopy = false;
					this.Invalidate();
				}
				// TODO
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LeafletControl_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				LeafletDataEntry LeafOpen = LeafList.Find(IsOpen);
				if (LeafOpen != null)
				{
					// Check Copy button pressed
					Rectangle Bounds = new Rectangle((int)LeafOpen.PanelSize.Width - m_bmpCopyFocus.Width + 1, LeafOpen.PanelPosition.Y + (int)LeafOpen.PanelSize.Height - m_bmpCopyFocus.Height, m_bmpCopyFocus.Width, m_bmpCopyFocus.Height);
					if (Bounds.Contains(e.Location))
					{
						m_IsFocusCopy = true;
						this.Invalidate();
					}
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LeafletControl_MouseUp(object sender, MouseEventArgs e)
		{
			// Be sure the Copy button has the focus before change anything
			if (e.Button == MouseButtons.Left && m_IsFocusCopy)
			{
				LeafletDataEntry LeafOpen = LeafList.Find(IsOpen);
				if (LeafOpen != null)
				{
					StringBuilder Text = new StringBuilder();
					Text.AppendLine(LeafOpen.Title.TrimEnd(Environment.NewLine.ToCharArray()));
					Text.AppendLine(LeafOpen.Address.TrimEnd(Environment.NewLine.ToCharArray()));
					Text.AppendLine(LeafOpen.Contact.TrimEnd(Environment.NewLine.ToCharArray()));
					Text.AppendLine(LeafOpen.Email.TrimEnd(Environment.NewLine.ToCharArray()));
					Text.AppendLine(LeafOpen.Rate.TrimEnd(Environment.NewLine.ToCharArray()));
					// Fire event to copy the containt of the leaf into the clipboard
					this.ClipboardSetTextEvent(Text.ToString());
				}

				m_IsFocusCopy = false;
				this.Invalidate();
			}
		}

		/// <summary>
		/// Find in a LeafletDataEntry list the first occurence of Open property = true.
		/// This is a predicate function used as a delegate in LeafList.Find(Predicate)
		/// </summary>
		/// <param name="Leaf"></param>
		/// <returns></returns>
		private bool IsOpen(LeafletDataEntry Leaf)
		{
			if (Leaf.Open)
				return true;
			else
				return false;
		}
		#endregion
	}
}
