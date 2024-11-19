using Gooey.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gooey
{
    internal class FacePictureBox : PictureBox
    {
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        private readonly object lockObject = new object();

        double opened = 0;
        double closed = 0;

        public FacePictureBox()
        {
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            timer.Enabled = false;
            CloseMouth();
        }

        public void OpenMouth()
        {
            Image = Resources.mouth_open;
            Invalidate();
        }

        public void CloseMouth()
        {
            Image = Resources.idle;
            Invalidate();
        }
    }
}
