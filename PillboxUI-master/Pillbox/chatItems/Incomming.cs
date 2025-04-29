using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.Logging;
using System.Diagnostics;

namespace Pillbox.chatItems
{
    public partial class Incomming : UserControl
    {
        public Incomming()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        public string Message
        {
            get
            {
                return label1.Text;
            }
            set
            {
                label1.Text = value;
                AdjustHeight();
            }
        }

        void AdjustHeight()
        {
            pictureBox1.Location = new Point(4, 3);
            int h = label1.Top+Utils.getHeightText(label1)+20;

            //label1.Height = Utils.getHeightText(label1) + 15;

            //guna2ContainerControl1.Height = label1.Top + guna2ContainerControl1.Top + label1.Height;
            this.Height = label1.Top+ h;
        }

        private void Incomming_Load(object sender, EventArgs e)
        {

        }

        public Image Avatar { get { return pictureBox1.Image; } set { pictureBox1.Image = value; } }
    }
}
