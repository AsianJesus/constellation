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
            { "getID", "SELECT id,name from flies order by start desc limit 1;" },
            {"getInfo",
                "SELECT packetID, recTime, flyTime, temp, press,height,speed,humidity,voltage,batch, gpsX,gpsZ,gpsY,co2,nh3,no2 FROM data WHERE flyID = @id AND recTime > @lasttime GROUP BY packetID ORDER BY packetID ;"},
            {"getFlights",
                "SELECT id,name,start,end FROM flies ORDER BY start DESC;"}
        };
        string comGetInfo, comGetID,comGetFlights;
        const string dbHostDef = "localhost", dbUIDDEf = "ct_ga", dbPasswDef = "11235ElViN", dbNameDef = "cansat";
        double lastTime;
        int flyID;
        string flyName;
        List<Packet> receivedPackets;
        DBControl dbConnection;
        PointControl plotTemp,plotPress,plotSpeed,plotHeight,plotVolt,plotCharge, plotHum,plotCO,plotNH,plotNO;
        string imagePath = "image\\"; 
        
        private double? ConvertTonullable(string text)
        {
            double result;
            return Double.TryParse(text, out result) ? (double?)result : null;
        }
        private void btnFullsize_Click(object sender, EventArgs e)
        {
        }

        private void btnSaveImg_Click(object sender, EventArgs e)
        {
        }

        private void changeImageDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult ans = folderBrowserDialog1.ShowDialog();
            if (ans == DialogResult.OK)
                imagePath = folderBrowserDialog1.SelectedPath + "\\plots";
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
            UpdateInfo();
        }
        private void ResetEverything()
        {
            plotCharge.Clear();
            plotCO.Clear();
            plotHeight.Clear();
            plotHum.Clear();
            plotSpeed.Clear();
            plotNH.Clear();
            plotNO.Clear();
            plotPress.Clear();
            plotTemp.Clear();
            plotVolt.Clear();
            tbGPSX.Clear();
            tbGPSY.Clear();
            tbGPSZ.Clear();
            analogClock1.Date = new DateTime(0);
            lastTime = 0;
            receivedPackets.Clear();
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
                string dbHost = form.Server;
                string dbUID = form.User;
                string dbPassw = form.Password;
                string dbName = form.Database;
                InitializeConnection(dbHost,dbUID,dbPassw,dbName);
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            ChDBCommands form = new ChDBCommands();
            form.DefComData = comGetInfo;
            form.DefComID = comGetID;
            form.DefComFlights = comGetFlights;
            DialogResult result = form.ShowDialog(this);
            if(result == DialogResult.OK)
            {
                Dictionary<string, string> valuesForSave = new Dictionary<string, string>();
                if(form.GetID != comGetID)
                {                    
                    comGetID = form.GetID;
                    getFlyID(out flyID, out flyName);
                    valuesForSave.Add("comGetID", comGetID);
                }
                if (form.GetData != comGetInfo)
                {
                    comGetInfo = form.GetData;
                    valuesForSave.Add("comGetInfo", comGetInfo);
                }
                if(form.GetFlights != comGetFlights)
                {
                    comGetFlights = form.GetFlights;
                    valuesForSave.Add("comGetFlights", comGetFlights);
                }
                if (valuesForSave.Count != 0)
                {
                    ResetEverything();
                    getFlyID(out flyID, out flyName);
                    UpdateInfo();
                }
                SaveInReg(valuesForSave);
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            SaveInReg(
                        new Dictionary<string, string>()
                        {
                            { "server", dbHostDef },
                            { "uid", dbUIDDEf },
                            { "passw", dbPasswDef},
                            { "database",dbNameDef},
                            {"comGetInfo",mysqlCommands["getInfo"] },
                            {"comGetID", mysqlCommands["getID"] },
                            {"comGetFlights",mysqlCommands["getFlights"] }
                        });
            string dbHost = dbHostDef;
            string dbUID = dbUIDDEf;
            string dbPassw = dbPasswDef;
            string dbName = dbNameDef;
            comGetID = mysqlCommands["getInfo"];
            comGetInfo = mysqlCommands["getID"];
            comGetFlights = mysqlCommands["getFlights"];
            InitializeConnection(dbHost,dbUID,dbPassw,dbName);
        }
        private void SaveInReg(Dictionary<string,string> pairs)
        {
            foreach(var kv in pairs)
            {
                Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Cansat_desktop", kv.Key, kv.Value);
            }
        }

        private void saveEveryPlotToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (!Directory.Exists(imagePath))
            {
                Directory.CreateDirectory(imagePath);
            }
            string folderName = imagePath + "\\plots\\" + DateTime.Now.ToString();
            if (!Directory.Exists(folderName))
                Directory.CreateDirectory(folderName);
            SaveImage(cTemp, folderName);
            SaveImage(cPress, folderName);
            SaveImage(cSpeed, folderName);
            SaveImage(cHeight, folderName);
        }
        private void SaveImage(Chart chart, string folderName)
        {
            chart.SaveImage(folderName + "//" +  chart.Name + ".png", ChartImageFormat.Png);
        }
        private void updateToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (flyID == -1)
                getFlyID(out flyID, out flyName);
            Update();
        }

        private void resetAndUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetEverything();
            if (flyID == -1)
                getFlyID(out flyID, out flyName);
            UpdateInfo();
        }

        private void gpsX_TextChanged(object sender, EventArgs e)
        {

        }

        private void CanSat_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void CanSat_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.F5)
            {
                    updateToolStripMenuItem1_Click(sender, e);
            }
            if (e.KeyData == Keys.F12)
                resetAndUpdateToolStripMenuItem_Click(sender, e);
        }

        private void chart3_Click(object sender, EventArgs e)
        {

        }

        private void chart8_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            Flight_List flist = new Flight_List();
            flist.Show(this);
            try
            {
                var result = dbConnection.ExecuteQuery(new MySqlCommand(comGetFlights));
                if (result.Count == 0)
                {
                    return;
                }
                flist.IDs.Lines = result[0].ToArray();
                flist.Names.Lines = result[1].ToArray();
                flist.Starts.Lines = result[2].ToArray();
                flist.Ends.Lines = result[3].ToArray();
            }
            catch
            {
                MessageBox.Show("Error appeared on loading");
            }
        }

        private void cPress_DoubleClick(object sender, EventArgs e)
        {
            ShowGraph(plotPress);
        }

        private void cSpeed_DoubleClick(object sender, EventArgs e)
        {
            ShowGraph(plotSpeed);
        }

        private void cHeight_DoubleClick(object sender, EventArgs e)
        {
            ShowGraph(plotHeight);
        }

        private void cVoltage_DoubleClick(object sender, EventArgs e)
        {
            ShowGraph(plotVolt);
        }

        private void cCharge_DoubleClick(object sender, EventArgs e)
        {
            ShowGraph(plotCharge);
        }

        private void cHum_DoubleClick(object sender, EventArgs e)
        {
            ShowGraph(plotHum);
        }

        private void cGases_DoubleClick(object sender, EventArgs e)
        {
            ShowGraph(new PointControl[]{ plotCO,plotNO,plotNH });
        }

        private void cTemp_DoubleClick(object sender, EventArgs e)
        {
            ShowGraph(plotTemp);
        }

        private string GetFromReg(string key, string def = "NULL")
        {
            return Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Cansat_desktop", key, def).ToString();
        }
        public CanSat()
        {
            InitializeComponent();
        }
        private Dictionary<string,string> LoadInfo()
        {
            string dbHost = dbHostDef;//GetFromReg("server", dbHostDef);
            string dbPassw = dbPasswDef;//GetFromReg("passw", dbPasswDef);
            string dbUID = dbUIDDEf;// GetFromReg("uid", dbUIDDEf);
            string dbName = dbNameDef;//GetFromReg("database", dbNameDef);
            comGetID = mysqlCommands["getID"];//GetFromReg("comGetID", mysqlCommands["getID"]);
            comGetInfo = mysqlCommands["getInfo"];//GetFromReg("comGetInfo", mysqlCommands["getInfo"]);
            comGetFlights = mysqlCommands["getFlights"];//GetFromReg("comGetFlights", mysqlCommands["getFlights"]);
            return new Dictionary<string, string> { { "host", dbHost }, { "password", dbPassw }, { "uid", dbUID }, { "database", dbName } }; 
        }

        private void cSpeed_Click(object sender, EventArgs e)
        {

        }

        private void InitializeConnection(string dbHost,string dbUID,string dbPassw, string dbName)
        { 
            dbConnection = new DBControl(dbHost, dbUID, dbPassw, dbName);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            KeyPreview = true;
            Dictionary<string, string> dbConnInfo = LoadInfo();
            InitializeConnection(dbConnInfo["host"], dbConnInfo["uid"], dbConnInfo["password"], dbConnInfo["database"]);
            plotTemp = new PointControl(cTemp.Series[0]);
            plotPress = new PointControl(cPress.Series[0]);
            plotSpeed = new PointControl(cSpeed.Series[0]);
            plotHeight = new PointControl(cHeight.Series[0]);
            plotVolt = new PointControl(cVoltage.Series[0]);
            plotCharge = new PointControl(cCharge.Series[0]);
            plotHum = new PointControl(cHum.Series[0]);
            plotCO = new PointControl(cGas.Series[0]);
            plotNH = new PointControl(cGas.Series[2]);
            plotNO = new PointControl(cGas.Series[1]);
            getFlyID(out flyID, out flyName);
            tbFlyID.Text = flyID.ToString();
            tbFlyName.Text = flyName;
            lastTime = 0;
            receivedPackets = new List<Packet>();
            UpdateInfo();
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }
        private bool getFlyID(out int id, out string name)
        {
            MySqlCommand comm = new MySqlCommand(comGetID);
            try
            {
                List<List<string>> execResults = dbConnection.ExecuteQuery(comm);
                id = int.Parse(execResults[0][0]);
                name = execResults[0][1];
                return true;
            }
            catch
            {
                id = -1;
                name = "";
                return false;
            }
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
            cPress.SaveImage("C:\\Users\\fruit\\Desktop\\1.jpg",ChartImageFormat.Jpeg);
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //if (dbConnection.IsOpen() && flyID != -1)
            //    UpdateInfo();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
        private void UpdateInfo()
        {
            List<List<String>> rawData;
           // try
            //{
                MySqlCommand comm = new MySqlCommand(comGetInfo);
                comm.Parameters.AddWithValue("@id", flyID);
                comm.Parameters.AddWithValue("@lasttime", lastTime);
                dbConnection.Command = comm;
                rawData = dbConnection.ExecuteQuery();
           // }
            //catch
            //{
            //    return;
            //}
            Packet temp;
            List<Packet> packets = new List<Packet>();
            if (rawData == null || rawData.Count() == 0 || rawData[0].Count() == 0)
                return;
            var packIDs = from pack in receivedPackets
                          select pack.PacketID;
            int packID;
            foreach(List<String> row in rawData)
            {
                //packetID,  recTime,flyTime, temp, pressure,height,speed,hum,voltage,batch, gx,gz,gy,co2,nh3,no2
                packID = int.Parse(row[0]);
                if (packIDs.Contains(packID))
                    continue;
                temp = new Packet(
                        packID,
                        Convert.ToDouble(row[1]),
                        ConvertTonullable(row[2]),
                        ConvertTonullable(row[3]),
                        ConvertTonullable(row[4]),
                        ConvertTonullable(row[5]),
                        ConvertTonullable(row[6]),
                        ConvertTonullable(row[7]),
                        ConvertTonullable(row[8]),
                        ConvertTonullable(row[9]),
                        new GPSCoordinates(ConvertTonullable(row[10]), ConvertTonullable(row[11]), ConvertTonullable(row[12])),
                        new Gases(ConvertTonullable(row[13]),ConvertTonullable(row[14]),ConvertTonullable(row[15]))
                    );
                receivedPackets.Add(temp);
            }
            if (receivedPackets.Count == 0)
                return;
            bool refacture = (from p in packets
                              where p.PacketID < receivedPackets.Max(x => x.PacketID)
                              select p.PacketID).Count() == 0;
            if (refacture)
            {
                receivedPackets.AddRange(packets);
                packets = receivedPackets;
                plotTemp.Clear();
                plotPress.Clear();
                plotHeight.Clear();
                plotCharge.Clear();
                plotCO.Clear();
                plotNH.Clear();
                plotNO.Clear();
                plotVolt.Clear();
                plotHum.Clear();
                tbGPSX.Clear();
                tbGPSY.Clear();
                tbGPSZ.Clear();
            }
            plotTemp.AddPointsXY(
                    packets.Where(p => { return p.Temperature != null; }).ToDictionary(p => { return (double)p.PacketID; }, p => { return (double)p.Temperature; })
                    );
            plotPress.AddPointsXY(
                    packets.Where(p => { return p.Pressure != null; }).ToDictionary(p => { return (double)p.PacketID; }, p => { return (double)p.Pressure; })
                    );
            plotHeight.AddPointsXY(
                    packets.Where(p => { return p.Height != null; }).ToDictionary(p => { return (double)p.PacketID; }, p => { return (double)p.Height; })
                    );
            plotSpeed.AddPointsXY(
                    packets.Where(p => { return p.Speed != null; }).ToDictionary(p => { return (double)p.PacketID; }, p => { return (double)p.Speed; })
                    );
            plotHum.AddPointsXY(
                    packets.Where(p => { return p.Humidity!= null; }).ToDictionary(p => { return (double)p.PacketID; }, p => { return (double)p.Humidity; })
                    );
            plotVolt.AddPointsXY(
                    packets.Where(p => { return p.Voltage != null; }).ToDictionary(p => { return (double)p.PacketID; }, p => { return (double)p.Voltage; })
                    );
            plotCharge.AddPointsXY(
                    packets.Where(p => { return p.Voltage != null; }).ToDictionary(p => { return (double)p.PacketID; }, p => { return (double)p.Voltage; })
                    );
            plotCO.AddPointsXY(
                    packets.Where(p => { return p.Gases.CO2 != null; }).ToDictionary(p => { return (double)p.PacketID; }, p => { return (double)p.Gases.CO2; })
                    );
            plotNH.AddPointsXY(
                    packets.Where(p => { return p.Gases.NH3 != null; }).ToDictionary(p => { return (double)p.PacketID; }, p => { return (double)p.Gases.NH3; })
                    );
            plotNO.AddPointsXY(
                    packets.Where(p => { return p.Gases.NO2 != null; }).ToDictionary(p => { return (double)p.PacketID; }, p => { return (double)p.Gases.NO2; })
                    );
            var positions = packets.Where(p => { return p.GpsX != null && p.GpsY != null && p.GpsZ != null; });
            if (positions.Count() != 0)
            {
                tbGPSX.Text = positions.Last().GpsX.ToString();
                tbGPSY.Text = positions.Last().GpsY.ToString();
                tbGPSZ.Text = positions.Last().GpsZ.ToString();
            }
        }
        private void ShowGraph(PointControl plot)
        {
            GraphicViewer graphic = new GraphicViewer();
            graphic.Name = plot.Name;
            graphic.Plots = new PointControl[]{ plot};
            graphic.Show(this);
        }
        private void ShowGraph(PointControl[] plots)
        {
            GraphicViewer graphic = new GraphicViewer();
            graphic.Plots = plots;
            graphic.Show(this);
        }
    } 
}
