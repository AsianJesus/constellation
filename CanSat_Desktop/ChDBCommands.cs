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
        private string comGetID, comGetData,comGetFlights;
        private string comGetIDDef, comGetDataDef, comGetFlightsDef;
        public ChDBCommands()
        {
            InitializeComponent();
        }
        public string DefComID
        {
            set
            {
                tbID.Text = value;
                comGetIDDef = value;
            }
        }
        public string DefComFlights
        {
            set
            {
                tbFlights.Text = value;
                comGetFlightsDef = value;
            }
        }
        public string DefComData
        {
            set
            {
                tbData.Text = value;
                comGetDataDef = value;
            }
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
            if (tbData.Text == comGetDataDef && tbID.Text == comGetIDDef && tbFlights.Text == comGetFlightsDef)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
            comGetID = tbID.Text;
            comGetData = tbData.Text;
            comGetFlights = tbFlights.Text;
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
        public string GetData
        {
            get { return comGetData; }
        }
        public string GetFlights
        {
            get { return comGetFlights; }
        }
    }
}
