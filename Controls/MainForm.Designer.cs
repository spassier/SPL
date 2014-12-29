namespace SPL
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.locatorControl1 = new SPL.LocatorControl();
			this.mapControl1 = new SPL.MapControl();
			this.leafletControl1 = new SPL.LeafletControl();
			this.SuspendLayout();
			// 
			// locatorControl1
			// 
			this.locatorControl1.Glyph = null;
			this.locatorControl1.Location = new System.Drawing.Point(0, 633);
			this.locatorControl1.Name = "locatorControl1";
			this.locatorControl1.Size = new System.Drawing.Size(300, 67);
			this.locatorControl1.TabIndex = 3;
			// 
			// mapControl1
			// 
			this.mapControl1.Flag = null;
			this.mapControl1.Location = new System.Drawing.Point(300, 0);
			this.mapControl1.Map = ((System.Drawing.Bitmap)(resources.GetObject("mapControl1.Map")));
			this.mapControl1.MaximumSize = new System.Drawing.Size(686, 700);
			this.mapControl1.Name = "mapControl1";
			this.mapControl1.Size = new System.Drawing.Size(686, 700);
			this.mapControl1.TabIndex = 2;
			this.mapControl1.TabStop = false;
			// 
			// leafletControl1
			// 
			this.leafletControl1.Glyph = null;
			this.leafletControl1.Location = new System.Drawing.Point(0, 0);
			this.leafletControl1.Name = "leafletControl1";
			this.leafletControl1.Size = new System.Drawing.Size(300, 634);
			this.leafletControl1.TabIndex = 1;
			this.leafletControl1.TabStop = false;
			this.leafletControl1.Title = "";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(986, 700);
			this.Controls.Add(this.locatorControl1);
			this.Controls.Add(this.mapControl1);
			this.Controls.Add(this.leafletControl1);
			this.DoubleBuffered = true;
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Form1";
			this.ResumeLayout(false);

		}

		#endregion

		private MapControl mapControl1;
		private LeafletControl leafletControl1;
		private LocatorControl locatorControl1;

	}
}

