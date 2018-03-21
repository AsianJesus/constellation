using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Windows.Forms.DataVisualization.Charting;
using Microsoft.Win32;

namespace CanSat_Desktop
{
    public partial class CanSat : Form
    {
        Dictionary<string, string> mysqlCommands = new Dictionary<string,string>(){
            { "getID", "SELECT id,name FROM flies ORDER BY start DESC LIMIT 1;" },
            {"getInfo",
                "SELECT packetID, temp, pressure, gx,gz,gy,height,hum,co2,nh3,no2,speed,voltage,batch,recTime, flyTime FROM data WHERE flyID = @id AND recTime > @lasttime ORDER BY packetID ;" }
        };
        string mysqlcomGetInfo, mysqlcomGetID;
        const string serverDef = "localhost", userDef = "root", passwDef = "", databaseDef = "cansat";
        string server, user, passw, database;
        double lastTime;
        int flyID;
        int pocketCount;
        DBControl db;
        PointControl plotTemp,plotPress,plotMF,plotHeight,plotIllum,plotVolt,plotCharge, plotHum,plotCO,plotNH,plotNO;
        PointControl plotSelected;
        string imagePath = "image\\"; 
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            double lX, rX, uY, dY;
            lX = plotSelected.MinimumX;
            rX = plotSelected.MaximumX;
            if (trackBar1.Value == trackBar1.Minimum || trackBar1.Value == 0)
            {
                uY = plotSelected.MaximumY * 1.05;
                dY = plotSelected.MinimumY * 0.95;
            }
            else
            {
                double dist = rX - lX;
                dist = dist * ((double)trackBar1.Value / (double)trackBar1.Maximum);
                double offset = (rX - lX)* 0.05;
                lX = dist - offset;
                rX = dist + offset;
                uY = plotSelected.MaximumAtRange(lX, rX)* 1.1 + plotSelected.MaximumY*0.1;
                dY = plotSelected.MinimumAtRange(lX, rX) * 0.9;
            }
            if (lX == rX || dY == uY)
                return;
            viewSelected.Resize(lX, dY, rX, uY);
        }

        private void btnFullsize_Click(object sender, EventArgs e)
        {
            trackBar1.Value = trackBar1.Minimum;
            trackBar1_Scroll(sender, e);
        }

        private void btnSaveImg_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(imagePath))
                Directory.CreateDirectory(imagePath);
            string fileName = DateTime.Now.ToBinary().ToString() + "-" + plotSelected.Name;
            chart2.SaveImage(imagePath + fileName + ".png", ChartImageFormat.Png);
        }

        private void changeImageDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult ans = folderBrowserDialog1.ShowDialog();
            if(ans == DialogResult.OK)
                imagePath = folderBrowserDialog1.SelectedPath + "\\image\\";
        }

        private void changeFlyIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InputID form = new InputID();
            DialogResult res = form.ShowDialog(this);
            if(res == DialogResult.OK)
                flyID = form.Result;
            ResetEverything();
            UpdateInfo();
        }

        private void updateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetEverything();
            UpdateInfo();
        }
        private void ResetEverything()
        {
            plotCharge.Clear();
            plotCO.Clear();
            plotHeight.Clear();
            plotHum.Clear();
            plotIllum.Clear();
            plotMF.Clear();
            plotNH.Clear();
            plotNO.Clear();
            plotPress.Clear();
            plotSelected.Clear();
            plotTemp.Clear();
            plotVolt.Clear();
            gpsX.Clear();
            gpsY.Clear();
            gpsZ.Clear();
            analogClock1.Date = new DateTime(0);
            lastTime = 0;
            pocketCount = 0;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            DBConfigure form = new DBConfigure();
            DialogResult result = form.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                SaveInReg(
                        new Dictionary<string, string>()
                        {
                            { "server", form.Server },
                            { "uid", form.User },
                            { "passw", form.Password },
                            { "database",form.Database }
                        });
                server = form.Server;
                user = form.User;
                passw = form.Password;
                database = form.Database;
                InitializeConnection();
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            ChDBCommands form = new ChDBCommands();
            DialogResult result = form.ShowDialog(this);
            if(result == DialogResult.OK)
            {
                if(form.GetID != "")
                {
                    SaveInReg(new Dictionary<string, string>() { { "comGetID", form.GetID } });
                }
                if (form.GetInfo != "")
                {
                    SaveInReg(new Dictionary<string, string>() { { "comGetInfo", form.GetInfo } });
                }
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            SaveInReg(
                        new Dictionary<string, string>()
                        {
                            { "server", serverDef },
                            { "uid", userDef },
                            { "passw", passwDef},
                            { "database",databaseDef},
                            {"comGetInfo",mysqlCommands["getInfo"] },
                            {"comGetID", mysqlCommands["getID"] }
                        });
        }
        private void SaveInReg(Dictionary<string,string> pairs)
        {
            foreach(var kv in pairs)
            {
                Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Cansat_desktop", kv.Key, kv.Value);
            }
        }
        private string GetFromReg(string key, string def = "NULL")
        {
            return Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Cansat_desktop", key, def).ToString();
        }
        ViewControlUnit viewSelected;
        public CanSat()
        {
            InitializeComponent();
        }
        private void LoadInfo()
        {
            server = GetFromReg("server", "");
            if (server == "")
                server = serverDef;
            passw = GetFromReg("passw", "NULL");
            if (passw == "")
                passw = passwDef;
            user = GetFromReg("uid", "");
            if (user == "")
                user = userDef;
            database = GetFromReg("database", "");
            if (database == "")
                database = databaseDef;
            mysqlcomGetID = GetFromReg("comGetID", "");
            if (mysqlcomGetID == "")
                mysqlcomGetID = mysqlCommands["getID"];
            mysqlcomGetInfo = GetFromReg("comGetInfo", "");
            if (mysqlcomGetInfo == "")
                mysqlcomGetInfo = mysqlCommands["getInfo"];
        }
        private void InitializeConnection()
        { 
            db = new DBControl(server, user, passw, database);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadInfo();
            InitializeConnection();
            plotTemp = new PointControl(chart1.Series[0]);
            plotPress = new PointControl(chart1.Series[1]);
            plotMF = new PointControl(chart1.Series[2]);
            plotHeight = new PointControl(chart1.Series[3]);
            plotIllum = new PointControl(chart1.Series[4]);
            plotVolt = new PointControl(chart1.Series[5]);
            plotCharge = new PointControl(chart1.Series[6]);
            plotHum = new PointControl(chart1.Series[7]);
            plotCO = new PointControl(chart1.Series[8]);
            plotNH = new PointControl(chart1.Series[9]);
            plotNO = new PointControl(chart1.Series[10]);
            plotSelected = new PointControl(chart2.Series[0]);
            viewSelected = new ViewControlUnit(chart2.ChartAreas[0]);
            flyID = getFlyID();
            lastTime = 0;
            pocketCount = 0;
            UpdateInfo();
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }
        private int getFlyID()
        {
            MySqlCommand comm = new MySqlCommand(mysqlcomGetID);
            int id = 0;
            try
            {
                List<List<string>> execResults = db.ExetuceQuery(comm);
                id = int.Parse(execResults[0][0]);
            }
            catch
            {

            }
            return id;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        private void chart1_Click(object sender, EventArgs e)
        {
        }

        private void chart1_Click_1(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            chart1.SaveImage("C:\\Users\\fruit\\Desktop\\1.jpg",ChartImageFormat.Jpeg);
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {           
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*
             
Temperature
Pressure
MagField
Height
Illimunation
Voltage
Charge
Humidity
CO
NH3
NO2
             
             */
            plotSelected.Clear();
            trackBar1.Value = trackBar1.Minimum;
            switch (comboBox1.SelectedIndex)
            {
                case 1:
                    plotSelected.CopyFrom(plotTemp);
                    break;
                case 2:
                    plotSelected.CopyFrom(plotPress);
                    break;
                case 3:
                    plotSelected.CopyFrom(plotMF);
                    break;
                case 4:
                    plotSelected.CopyFrom(plotHeight);
                    break;
                /*case 5:
                    plotSelected.CopyFrom(plotIllum);
                    break;*/
                case 6:
                    plotSelected.CopyFrom(plotVolt);
                    break;
                case 7:
                    plotSelected.CopyFrom(plotCharge);
                    break;
                case 8:
                    plotSelected.CopyFrom(plotHum);
                    break;
                case 9:
                    plotSelected.CopyFrom(plotCO);
                    break;
                case 10:
                    plotSelected.CopyFrom(plotNH);
                    break;
                case 11:
                    plotSelected.CopyFrom(plotNO);
                    break;
            }
            trackBar1_Scroll(sender, e);
        }
        private void UpdateInfo()
        {
            MySqlCommand comm = new MySqlCommand(mysqlcomGetInfo);
            comm.Parameters.AddWithValue("@id", flyID);
            comm.Parameters.AddWithValue("@lasttime", lastTime);
            try
            {
                //packetID, temp, pressure, gx,gz,gy,height,hum,co2,nh3,no2,speed,voltage,batch
                db.Command = comm;
                List<List<String>> result = db.ExecuteQuery();
                pocketCount += result[0].Count();
                List<double> temp = result[1].Where((string s) => { return s != ""; }).Select((string s) => { return double.Parse(s); }).ToList();
                List<double> press = result[2].Where((string s) => { return s != ""; }).Select((string s) => { return double.Parse(s); }).ToList();
                double gx = result[3].Where((string s)=> { return s != ""; }).Select((string s) => {  return double.Parse(s); }).Last();
                double gz = result[4].Where((string s) => { return s != ""; }).Select((string s) => { return double.Parse(s); }).Last();
                double gy = result[5].Where((string s) => { return s != ""; }).Select((string s) => { return double.Parse(s); }).Last();
                List<double> height = result[6].Where((string s) => { return s != ""; }).Select((string s) => { return double.Parse(s); }).ToList();
                List<double> hum = result[7].Where((string s) => { return s != ""; }).Select((string s) => { return double.Parse(s); }).ToList();
                List<double> co2 = result[8].Where((string s) => { return s != ""; }).Select((string s) => { return double.Parse(s); }).ToList();
                List<double> nh3 = result[9].Where((string s) => { return s != ""; }).Select((string s) => { return double.Parse(s); }).ToList();
                List<double> no2 = result[10].Where((string s) => { return s != ""; }).Select((string s) => { return double.Parse(s); }).ToList();
                List<double> speed = result[11].Where((string s) => { return s != ""; }).Select((string s) => { return double.Parse(s); }).ToList();
                List<double> voltage = result[12].Where((string s) => { return s != ""; }).Select((string s) => { return double.Parse(s); }).ToList();
                List<double> batch = result[13].Where((string s) => { return s != ""; }).Select((string s) => { return double.Parse(s); }).ToList();
                lastTime = result[14].Select((string s) => { return double.Parse(s); }).Count() != 0 ? result[14].Select((string s) => { return double.Parse(s); }).Max() : 0;
                double time = result[15].Select((string s) => { return double.Parse(s); }).Max();

                plotTemp.AddPointsY(temp.ToArray());
                plotPress.AddPointsY(press.ToArray());
                plotHeight.AddPointsY(height.ToArray());
                plotHum.AddPointsY(hum.ToArray());
                plotCO.AddPointsY(co2.ToArray());
                plotNH.AddPointsY(nh3.ToArray());
                plotNO.AddPointsY(no2.ToArray());
                plotVolt.AddPointsY(voltage.ToArray());
                plotCharge.AddPointsY(batch.ToArray());
                gpsX.Text = gx.ToString();
                gpsY.Text = gy.ToString();
                gpsZ.Text = gz.ToString();
                if (progressBar1.Maximum < pocketCount)
                    progressBar1.Maximum = pocketCount;
                progressBar1.Value = pocketCount;
                analogClock1.Date = new DateTime((long)(time * 10000000));
            }
            catch
            {

            }
        }
    } 
}
