using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Map
{
    public partial class frmMap : Form
    {
        double zoomFactor;
        Size mapInitialSize;

        public frmMap()
        {
            zoomFactor = 2.0;
            InitializeComponent();
            mapInitialSize = Size;
        }

        private void frmMap_Load(object sender, EventArgs e)
        {
            //this.FormBorderStyle = FormBorderStyle.None;
            //this.WindowState = FormWindowState.Maximized;
            RestorePositionAndSize();
        }

        private void frmMap_LocationChanged(object sender, EventArgs e)
        {
            RestorePositionAndSize();
        }

        private void RestorePositionAndSize()
        {
            this.TopMost = true;
            var screenSize = Screen.PrimaryScreen.Bounds;
            this.Size = new System.Drawing.Size(screenSize.Width, screenSize.Height);
            this.Location = new Point(0, 0);
            ResizePictureBox();
        }

        private void ResizePictureBox()
        {
            pbMap.Size = new Size((int)(mapInitialSize.Width * zoomFactor), 
                (int)(mapInitialSize.Height * zoomFactor));
        }

        private void frmMap_ResizeEnd(object sender, EventArgs e)
        {
            RestorePositionAndSize();
        }

    }
}
