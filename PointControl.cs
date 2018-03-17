using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace WFTest
{
    class PointControl
    {
        private Series curve;
        public PointControl(Chart chart, int indx) { curve = chart.Series[0]; }
        public PointControl(Series s) { curve = s;}
        public void Clear()
        {
            curve.Points.Clear();
        }
        public void AddPointsY(double[] dots)
        {
            foreach (double x in dots)
                AddPointY(x);
        }
        public void AddPointsXY(Dictionary<double, double> dots)
        {
            foreach (KeyValuePair<double, double> kvp in dots)
                AddPointXY(kvp.Key, kvp.Value);
        }
        public void AddPointY(double x)
        {
            double xPos = MaximumX;
            AddPointXY(++xPos, x);
        }
        public void AddPointXY(double x, double y)
        {
            curve.Points.AddXY(x, y);
        }
        public DataPoint GetPoint(double x)
        {

            return curve.Points.Where((DataPoint y) => { return y.XValue == x; }).First();
        }
        public double MaximumX
        {
            get
            {
                List<DataPoint> points = curve.Points.ToList();
                if (points.Count == 0)
                    return -1;
                double max = points.Max<DataPoint>((DataPoint x) => { return x.XValue; });
                return max;
            }
        }
        public double MinimumX
        {
            get
            {
                List<DataPoint> points = curve.Points.ToList();
                if (points.Count == 0)
                    return -1;
                double min = points.Min<DataPoint>((DataPoint x) => { return x.XValue; });
                return min;
            }
        }
        public double MaximumY
        {
            get
            {
                List<DataPoint> points = curve.Points.ToList();
                if (points.Count == 0)
                    return 0;
                double max = points.Max((DataPoint dp) => { return dp.YValues.Max(); });
                return max;
            }
        }
        public double MinimumY
        {
            get
            {
                List<DataPoint> points = curve.Points.ToList();
                if (points.Count == 0)
                    return 0;
                double min = points.Min((DataPoint dp) => { return dp.YValues.Min(); });
                return min;
            }
        }
        public void CopyTo(PointControl source)
        {
            foreach(DataPoint dp in this.curve.Points.ToArray())
            {
                source.curve.Points.AddXY(dp.XValue, dp.YValues[0]);
                
            }
        }
    }
}
