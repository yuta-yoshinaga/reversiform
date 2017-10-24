using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReversiForm
{
    public partial class Reversi : Form
    {
        public Reversi()
        {
            InitializeComponent();
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
			TableLayoutPanelCellPosition pos = this.tableLayoutPanel1.GetCellPosition((Control)sender);

            Console.WriteLine("click x=" + pos.Column + " y=" + pos.Row);
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
