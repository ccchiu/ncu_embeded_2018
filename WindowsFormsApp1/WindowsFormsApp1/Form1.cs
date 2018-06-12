using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.Text.RegularExpressions;
using System.Windows.Forms.DataVisualization.Charting;

namespace WindowsFormsApp1
{
   
    public partial class Form1 : Form
    {
        private SerialPort _serialPort = null;
        private bool receiving;
        private Thread t;
        private double[] tempArray= new double[60];
        private double[] humiArray = new double[60];
        private int alertTemp = 40;


        public Form1()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            
            InitializeComponent();
        }

        private delegate void UpdateUICallBack(string value, Control ctl);
        private void UpdateUI(string value, Control ctl)
        {
            if (this.InvokeRequired)
            {
                UpdateUICallBack uu = new UpdateUICallBack(UpdateUI);
                this.Invoke(uu, value, ctl);
            }
            else
            {
                ctl.Text = value;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            // Gets an array of serial port names for the current computer
            string[] serialPorts = SerialPort.GetPortNames();

            // Fill in Combobox with serial port names
            cbListPorts.DataSource = serialPorts;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_serialPort != null && _serialPort.IsOpen)
                _serialPort.Close();

            receiving = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (btnOpen.Text == "Open")
            {
                // Set up serial port
                _serialPort = new SerialPort(cbListPorts.SelectedItem.ToString(),
                                                9600, Parity.None, 8, StopBits.One);

                try
                {
                    // Open serial port
                    _serialPort.Open();
                    btnOpen.Text = "Close";
                    // btnLED.Enabled = true;
                    receiving = true;
                    t = new Thread(ReadComport);
                    t.IsBackground = true;
                    t.Start();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Open() error: " + ex.Message);
                }
            }
            else
            {
               // MessageBoxButtons("Close");
                _serialPort.Close();
                receiving = false;
                btnOpen.Text = "Open";
                // btnLED.Enabled = false;
            }
      
    }
        private void ReadComport()
        {

            while (receiving)
            {
                if (_serialPort.BytesToRead > 0)
                { 
                    string rawdata;
                    //Console.WriteLine(_serialPort.ReadLine());    
                    rawdata = _serialPort.ReadLine().Trim().TrimEnd('\n').TrimEnd('\r'); 
                    string[] data= rawdata.Split(',');
                    string pattern = @"[a-zA-Z=]";
                    //string replacement = Regex.Replace(s, @"\t|\n|\r", "");
                    string replacement = "";
                    Regex rgx = new Regex(pattern);
                    string Temperature = rgx.Replace(data[0], replacement);
                    string Humidity= rgx.Replace(data[1], replacement);
                    Console.WriteLine(Temperature);
                    //Console.WriteLine(Humidity);


                    string strReplaceT = Temperature.Replace(".", "");
                    int timesT = (Temperature.Length - strReplaceT.Length) / ".".Length;
                    if (timesT <= 1)
                    {
                        tempArray[tempArray.Length - 1] = Convert.ToDouble(Temperature);
                    }
                    else
                    {
                        Temperature = "28";
                    }
                    

                    /* process serial data dupicate '." */
                    string strReplace = Humidity.Replace(".", "");
                    int times = (Humidity.Length - strReplace.Length) / ".".Length;
                    if (times <= 1)
                    {
                        humiArray[humiArray.Length - 1] = Convert.ToDouble(Humidity);
                    }
                    else {
                        Humidity = "30";
                    }


                    Array.Copy(tempArray, 1, tempArray, 0, tempArray.Length - 1);
                    Array.Copy(humiArray, 1, humiArray, 0, humiArray.Length - 1);

                    if (chart1.IsHandleCreated)
                    {
                        this.Invoke((MethodInvoker)delegate { UpdateCpuChart(); });
                    }
                    
                    // Console.WriteLine(result);
                    UpdateUI(Temperature, label3);
                    UpdateUI(Humidity, label4);


                    int warning_temp = Convert.ToInt32(Convert.ToDouble(Temperature));
                    Console.WriteLine("w" + warning_temp);
                    Console.WriteLine("a" + alertTemp);

                    if (warning_temp >= alertTemp)
                    {
                        warningbox(true);
                    }
                    else {
                        warningbox(false);
                    }


                }
                else
                {
                    
                }
              Thread.Sleep(200);    
                   
            }

        }

        private void UpdateCpuChart()
        {
            chart1.Series["Temperature"].Points.Clear();
            chart1.Series["Humidity"].Points.Clear();

            for (int i = 0; i < tempArray.Length - 1; ++i)
            {
                chart1.Series["Temperature"].Points.AddY(tempArray[i]);
                chart1.Series["Humidity"].Points.AddY(humiArray[i]);
            }
        }

        

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label6.Text = trackBar1.Value.ToString();
            alertTemp= trackBar1.Value;
        }


        private void warningbox(bool show)
        {

            //MessageBox.Show("溫度高於或低於警示值"+alertTemp, "警告",MessageBoxButtons.OK, MessageBoxIcon.Error);
           
                Console.WriteLine("Warning....");
                this.Invoke((MethodInvoker)delegate { UpdatePic(show); });

        }

        private void UpdatePic(bool show)
        {
            Console.WriteLine("Warning...PIC.");
            pictureBox1.Visible = show;
            if (show) {
                timer1.Start();
            }else {
                timer1.Stop();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            pictureBox1.Visible = !pictureBox1.Visible;

        }
    }
}



