namespace Map
{
    partial class frmMap
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
            this.pnlZoom = new System.Windows.Forms.Panel();
            this.pbMap = new System.Windows.Forms.PictureBox();
            this.pnlZoom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbMap)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlZoom
            // 
            this.pnlZoom.AutoScroll = true;
            this.pnlZoom.Controls.Add(this.pbMap);
            this.pnlZoom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlZoom.Location = new System.Drawing.Point(0, 0);
            this.pnlZoom.Name = "pnlZoom";
            this.pnlZoom.Padding = new System.Windows.Forms.Padding(5);
            this.pnlZoom.Size = new System.Drawing.Size(316, 268);
            this.pnlZoom.TabIndex = 1;
            // 
            // pbMap
            // 
            this.pbMap.Image = global::Map.Resource1.PlainMap;
            this.pbMap.Location = new System.Drawing.Point(0, 0);
            this.pbMap.Name = "pbMap";
            this.pbMap.Size = new System.Drawing.Size(890, 550);
            this.pbMap.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbMap.TabIndex = 1;
            this.pbMap.TabStop = false;
            // 
            // frmMap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(316, 268);
            this.Controls.Add(this.pnlZoom);
            this.MaximizeBox = false;
            this.Name = "frmMap";
            this.Text = "Карта мира";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmMap_Load);
            this.ResizeEnd += new System.EventHandler(this.frmMap_ResizeEnd);
            this.LocationChanged += new System.EventHandler(this.frmMap_LocationChanged);
            this.pnlZoom.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbMap)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlZoom;
        private System.Windows.Forms.PictureBox pbMap;

    }
}

