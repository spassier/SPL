namespace SPL
{
	partial class LocatorControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.edtSearch = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// edtSearch
			// 
			this.edtSearch.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.edtSearch.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.edtSearch.ForeColor = System.Drawing.Color.Ivory;
			this.edtSearch.Location = new System.Drawing.Point(126, 16);
			this.edtSearch.MaxLength = 63;
			this.edtSearch.Name = "edtSearch";
			this.edtSearch.Size = new System.Drawing.Size(122, 16);
			this.edtSearch.TabIndex = 0;
			this.edtSearch.TabStop = false;
			this.edtSearch.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.edtSearch.WordWrap = false;
			this.edtSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.edtZipCode_KeyDown);
			// 
			// LocatorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.edtSearch);
			this.DoubleBuffered = true;
			this.Name = "LocatorControl";
			this.Size = new System.Drawing.Size(300, 48);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.LocatorControl_Paint);
			this.Leave += new System.EventHandler(this.LocatorControl_Leave);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LocatorControl_MouseDown);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.LocatorControl_MouseMove);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox edtSearch;
	}
}
