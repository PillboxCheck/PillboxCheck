using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pillbox.chatItems
{
    public partial class Outgoing : UserControl
    {
        public Outgoing()
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
            int h = label1.Top + Utils.getHeightText(label1) + 10;

            //label1.Height = Utils.getHeightText(label1) + 15;

            //guna2ContainerControl1.Height = label1.Top + guna2ContainerControl1.Top + label1.Height;
            this.Height = label1.Top + h;
        }

        private void guna2ContainerControl1_Click(object sender, EventArgs e)
        {

        }

        private void Outgoing_Load(object sender, EventArgs e)
        {

        }
    }
}
