using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dieukhientuxa_inchulenmanhinh_client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string domain = txtDomain.Text.Trim();
            string serverIp = txtServerIp.Text.Trim();
            string portText = txtPort.Text.Trim();

            if (string.IsNullOrEmpty(domain) || string.IsNullOrEmpty(serverIp) || string.IsNullOrEmpty(portText))
            {
                MessageBox.Show("Vui lòng nhập tên miền, địa chỉ IP của máy chủ và cổng!");
                return;
            }

            if (!int.TryParse(portText, out int port))
            {
                MessageBox.Show("Cổng không hợp lệ!");
                return;
            }

            SendRequest(domain, serverIp, port);
        }

        private void SendRequest(string domain, string serverIp, int port)
        {
            try
            {
                using (TcpClient client = new TcpClient(serverIp, port))
                using (NetworkStream stream = client.GetStream())
                {
                    byte[] data = Encoding.ASCII.GetBytes(domain);
                    stream.Write(data, 0, data.Length);
                }
                MessageBox.Show("Yêu cầu đã gửi thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối: {ex.Message}");
            }
        }
    }
    }
