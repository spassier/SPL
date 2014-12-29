using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;


namespace SPL
{
	public partial class MainForm : Form
	{
		#region Consts
		private string WINDOW_TITLE = "MERCURA - Service Point Locator";
		private string WINDOW_ICON = "SPL.ico";
		#endregion

		#region Variables
		private Size m_MaximumSize;
		#endregion

		#region Methods
		public MainForm()
		{
			InitializeComponent();

			// Set MainForm properties
			System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
			this.Icon = new Icon(assembly.GetManifestResourceStream("SPL.Resources." + WINDOW_ICON));
			this.Text = WINDOW_TITLE;
			this.BackColor = Color.Black;

			// Subscribe MapControl events
			this.mapControl1.UnselectionEvent += new MapControl.UnselectionEventHandler(OnMapUnselection);
			this.mapControl1.SelectionEvent += new MapControl.SelectionEventHandler(OnMapSelection);
			
			// Subscribe LeaftLet events
			this.leafletControl1.ClipboardSetTextEvent += new LeafletControl.ClipboardSetTextHandler(OnClipboardSetText);

			// Subscribe Locator event
			this.locatorControl1.SearchEvent += new LocatorControl.SearchEventHandler(OnLocatorSearch);

			// Beware, the maximum size of the form depends on the children size
			m_MaximumSize = this.SetMaximumSizeFromClientArea(new Size(leafletControl1.Width + mapControl1.MaximumSize.Width, mapControl1.MaximumSize.Height));

			// Initialize children controls
			mapControl1.Map = Program.ApplicationCore.bmpResourceMap;
			mapControl1.Flag = Program.ApplicationCore.bmpFlag;
			leafletControl1.Glyph = new Bitmap(Program.ApplicationCore.bmpLeafletGlyph);
			locatorControl1.Glyph = new Bitmap(Program.ApplicationCore.bmpLocatorGlyph);
		}

		/// <summary>
		/// WndProc iverride to handle specific behaviour
		/// </summary>
		/// <param name="Message"></param>
		/// <remarks>
		/// WM_SHOWFIRSTINSTANCE: 
		/// Received by the first instance of the application to bring itself to the top of all others windows
		/// Sent by futher instance of the application (using a mutex)
		/// WM_SIZING:
		/// No flickering by handling WM_SIZING messages because C# wait the finish of resizing before redraw the form
		/// </remarks>
		protected override void WndProc(ref Message Message)
		{
			// Mutex message 
			if (Message.Msg == ApplicationCommon.WM_SHOWFIRSTINSTANCE)
			{
				WinAPI.ShowWindow(this.Handle, WinAPI.SW_SHOWNORMAL);
				WinAPI.SetForegroundWindow(this.Handle);
			}
			else
			{
				// Handle other WinApi messages
				switch (Message.Msg)
				{
					// WM_SIZING message
					case WinAPI.WM_SIZING:
					{
						WinAPI.RECT FormBounds = (WinAPI.RECT)Marshal.PtrToStructure(Message.LParam, typeof(WinAPI.RECT));

						switch (Message.WParam.ToInt32())
						{
							case WinAPI.WMSZ_LEFT:
							{
								if (FormBounds.Right - FormBounds.Left > m_MaximumSize.Width)
									FormBounds.Left = FormBounds.Right - m_MaximumSize.Width;
								break;
							}
							case WinAPI.WMSZ_RIGHT:
							{
								if (FormBounds.Right - FormBounds.Left > m_MaximumSize.Width)
									FormBounds.Right = FormBounds.Left + m_MaximumSize.Width;
								break;
							}
							case WinAPI.WMSZ_TOP:
							{
								if (FormBounds.Bottom - FormBounds.Top > m_MaximumSize.Height)
									FormBounds.Top = FormBounds.Bottom - m_MaximumSize.Height;
								break;
							}
							case WinAPI.WMSZ_TOPLEFT:
							{
								if (FormBounds.Bottom - FormBounds.Top > m_MaximumSize.Height)
									FormBounds.Top = FormBounds.Bottom - m_MaximumSize.Height;
								if (FormBounds.Right - FormBounds.Left > m_MaximumSize.Width)
									FormBounds.Left = FormBounds.Right - m_MaximumSize.Width;
								break;
							}
							case WinAPI.WMSZ_TOPRIGHT:
							{
								if (FormBounds.Bottom - FormBounds.Top > m_MaximumSize.Height)
									FormBounds.Top = FormBounds.Bottom - m_MaximumSize.Height;
								if (FormBounds.Right - FormBounds.Left > m_MaximumSize.Width)
									FormBounds.Right = FormBounds.Left + m_MaximumSize.Width;
								break;
							}
							case WinAPI.WMSZ_BOTTOM:
							{
								if (FormBounds.Bottom - FormBounds.Top > m_MaximumSize.Height)
									FormBounds.Bottom = FormBounds.Top + m_MaximumSize.Height;
								break;
							}
							case WinAPI.WMSZ_BOTTOMLEFT:
							{
								if (FormBounds.Bottom - FormBounds.Top > m_MaximumSize.Height)
									FormBounds.Bottom = FormBounds.Top + m_MaximumSize.Height;
								if (FormBounds.Right - FormBounds.Left > m_MaximumSize.Width)
									FormBounds.Left = FormBounds.Right - m_MaximumSize.Width;
								break;
							}
							case WinAPI.WMSZ_BOTTOMRIGHT:
							{
								if (FormBounds.Bottom - FormBounds.Top > m_MaximumSize.Height)
									FormBounds.Bottom = FormBounds.Top + m_MaximumSize.Height;
								if (FormBounds.Right - FormBounds.Left > m_MaximumSize.Width)
									FormBounds.Right = FormBounds.Left + m_MaximumSize.Width;
								break;
							}
						}

						Marshal.StructureToPtr(FormBounds, Message.LParam, true);

						// Resize children controls
						Size Offset = new Size((FormBounds.Right - FormBounds.Left) - this.Width, (FormBounds.Bottom - FormBounds.Top) - this.Height);
						mapControl1.Resizing(Offset);
						leafletControl1.Resizing(Offset);
						// TODO: add resizing method of LocatorControl

						break;
					}
				}
			}

			base.WndProc(ref Message);
		}

		/// <summary>
		/// Define the maximum size of the form according to a given area
		/// </summary>
		/// <param name="ClientArea"></param>
		/// <returns></returns>
		private Size SetMaximumSizeFromClientArea(Size ClientArea)
		{
			Size NewSize = new Size();

			NewSize.Width = ClientArea.Width + SystemInformation.FrameBorderSize.Width * 2;
			NewSize.Height = ClientArea.Height + SystemInformation.FrameBorderSize.Height * 2 + SystemInformation.CaptionHeight;

			return NewSize;
		}

		/// <summary>
		/// Handle Map unselection event. Occurs when a department is clicked on the Map after been selected.
		/// All entries in the leafletControl are removed.
		/// </summary>
		private void OnMapUnselection()
		{
			leafletControl1.Clear(true);
			//leafletControl1.Invalidate();
		}

		/// <summary>
		/// Handle Map selection event. Occurs when a department is clicked on the Map.
		/// All entries in the leafletControl are removed and a new entry is added for each Service Point object found in the selected department
		/// </summary>
		/// <param name="ColorID"></param>
		private void OnMapSelection(Color ColorID)
		{
			leafletControl1.Clear(false);

			// Change the title of the leaflet control
			leafletControl1.Title = Program.ApplicationCore.GetDepartmentTitle(ColorID);

			// Create a new entry in the leaflet control for each service point found in the selected department 
			List<StringCollection> textList = Program.ApplicationCore.GetAllServicePointText(ColorID);
			foreach (StringCollection textCollection in textList)
				leafletControl1.Add(textCollection[0], textCollection[1], textCollection[2], textCollection[3], textCollection[4], textCollection[5]);
			
			leafletControl1.Invalidate();
		}

		/// <summary>
		/// 
		/// </summary>
		private void OnClipboardSetText(string Text)
		{
			Clipboard.Clear();
			Clipboard.SetText(Text, TextDataFormat.UnicodeText);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="postalCode"></param>
		private void OnLocatorSearch(string searchText)
		{
			Log.WriteLine("> Starting new search");

			bool result = true;

			// Backup the cursor shape and set a waitcursor
			Cursor previousCursor = this.Cursor;
			this.Cursor = Cursors.WaitCursor;
			
			// Parse the user request and extract postal code and/or a city name
			string postalCode, city;
			Program.ApplicationCore.ParseSearch(searchText, out postalCode, out city);

			if (postalCode == String.Empty)
			{
				result = false;
				Program.ApplicationCore.ShowInfoMessage("Code postal erroné ou manquant", InfoMessageType.Warning);
				Log.WriteLine("...WARNING: invalid request because postal code is null");
			}
			else if (Program.ApplicationCore.IsValidePostalCode(postalCode))
			{
				result = true;
				Log.WriteLine("...Postal code {0} validated", postalCode);
			}
			else
			{
				result = false;
				Program.ApplicationCore.ShowInfoMessage("Code postal invalide", InfoMessageType.Warning);
				Log.WriteLine("...WARNING: invalid postal code {0}", postalCode);
			}

			// Create new entries in the leaflet control for each service point selected
			if (result)
			{
				// Clear the Map and Leaflet controls
				mapControl1.Clear();
				leafletControl1.Clear(true);

				Program.ApplicationCore.ShowInfoMessage("Recherche en cours...", InfoMessageType.Normal);

				List<StringCollection> textList = Program.ApplicationCore.GetAllServicePointText(postalCode, city, locatorControl1.SearchThreshold, locatorControl1.SearchCriteria);
				for (int i = 0; i < locatorControl1.SearchThreshold && i < textList.Count; i++)
					leafletControl1.Add(textList[i][0], textList[i][1], textList[i][2], textList[i][3], textList[i][4], textList[i][5]);
				//foreach (StringCollection textCollection in textList)
				//  leafletControl1.Add(textCollection[0], textCollection[1], textCollection[2], textCollection[3], textCollection[4], textCollection[5]);
				if (textList.Count == 0)
					Program.ApplicationCore.ShowInfoMessage("Recherche annulée: erreur de connexion", InfoMessageType.Error);
				else
					Program.ApplicationCore.ShowInfoMessage("Recherche terminée !", InfoMessageType.Normal);
			}

			// Set back the previous cursor shape
			this.Cursor = previousCursor;
		}
		#endregion
	}
}
