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
using System.IO;
using System.Diagnostics;

namespace UPDATE_ARDUINO
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                comboBox_com.DataSource = SerialPort.GetPortNames();
            }
            catch (Exception ex)
            {
                // Hiển thị thông báo lỗi trong MessageBox
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                comboBox_com.DataSource = SerialPort.GetPortNames();
            }
            catch (Exception ex)
            {
                // Hiển thị thông báo lỗi trong MessageBox
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "HEX Files (*.hex)|*.hex",
                    Title = "Select HEX File"
                };

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    tb_HexFilePath.Text = openFileDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                // Hiển thị thông báo lỗi trong MessageBox
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                string hexFilePath = tb_HexFilePath.Text;
                string comPort = comboBox_com.Text;

                if (string.IsNullOrEmpty(hexFilePath) || string.IsNullOrEmpty(comPort))
                {
                    MessageBox.Show("Please select a HEX file and COM port.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DialogResult result = MessageBox.Show(
                   "Bạn có chắc chắn muốn cập nhật firmware không?",
                   "Xác nhận",
                   MessageBoxButtons.YesNo,
                   MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Call AVRDude
                    UploadHexFile(hexFilePath, comPort);
                }
            }
            catch (Exception ex)
            {
                // Hiển thị thông báo lỗi trong MessageBox
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void UploadHexFile(string hexFilePath, string comPort)
        {

            // Lấy đường dẫn tới file avrdude.exe và avrdude.conf trong thư mục dự án
            string avrdudePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "avrdude");
            //string avrdudePath = @"C:\Users\PC khoa\AppData\Local\Arduino15\packages\arduino\tools\avrdude\6.3.0-arduino17/bin/avrdude";
            string avrdudeConfPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "avrdude.conf");

            // Arguments for avrdude
            string  arguments = $"-C\"{avrdudeConfPath}\" -v -patmega2560 -cwiring -P{comPort} -b115200 -D -Uflash:w:\"{hexFilePath}\":i";


            // Start process
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = avrdudePath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            process.Start();

            // Read output
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            // Display output or error
            if (process.ExitCode == 0)
            {
                tb_terminal.Text = "Upload successful: " + '\n';
                tb_terminal.Text = tb_terminal.Text + output + error;
            }
            else
            {
                tb_terminal.Text = "Upload failed: " + '\n';
                tb_terminal.Text = tb_terminal.Text + error;
            }
        }
    }
}
