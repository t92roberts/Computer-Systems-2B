using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Memory
{
    public partial class Form1 : Form
    {
        Random rand = new Random();

        public Form1()
        {
            InitializeComponent();
        }

        private void flashTimer_Tick(object sender, EventArgs e)
        {
            flashColour();
        }

        private void flashTimer_Elapsed(object sender, EventArgs e)
        {
            flashTimer.Stop();
            resetColours();
        }

        private void betweenFlashTimer_Tick(object sender, EventArgs e)
        {
            
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            flashTimer.Start();
            
        }

        private void flashColour()
        {
            switch (rand.Next(2))
            {
                case 0:
                    btnRed.BackColor = Color.White;
                    break;
                case 1:
                    btnBlue.BackColor = Color.White;
                    break;
                default:
                    break;
            }
        }

        private void resetColours()
        {
            btnRed.BackColor = Color.Red;
            btnBlue.BackColor = Color.Blue;
        }
    }
}
