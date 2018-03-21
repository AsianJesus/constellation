using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CanSat_Desktop
{
    public partial class ChDBCommands : Form
    {
        private string comGetID, comGetInfo;
        public ChDBCommands()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void ChDBCommands_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "" && textBox1.Text == "")
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
            comGetID = textBox2.Text;
            comGetInfo = textBox1.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
        public string GetID
        {
            get { return comGetID; }
        }
        public string GetInfo
        {
            get { return comGetInfo; }
        }
    }
}
