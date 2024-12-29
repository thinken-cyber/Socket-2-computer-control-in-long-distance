using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace dieukhientuxa_sudungtrinhduyet_server
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            string ipAddress = txtServerIp.Text.Trim();
            string portText = txtPort.Text.Trim();

            if (string.IsNullOrEmpty(ipAddress) || string.IsNullOrEmpty(portText))
            {
                MessageBox.Show("Vui lòng nhập địa chỉ IP và cổng!");
                return;
            }

            if (!int.TryParse(portText, out int port))
            {
                MessageBox.Show("Cổng không hợp lệ!");
                return;
            }

            StartServer(ipAddress, port);
        }

        private void StartServer(string ipAddress, int port)
        {
            try
            {
                TcpListener listener = new TcpListener(IPAddress.Parse(ipAddress), port);
                listener.Start();
                MessageBox.Show("Máy 2 đang lắng nghe yêu cầu...");

                // Lắng nghe yêu cầu từ Máy 1
                listener.BeginAcceptTcpClient(AcceptClientCallback, listener);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi bắt đầu lắng nghe: {ex.Message}");
            }
        }

        private void AcceptClientCallback(IAsyncResult ar)
        {
            TcpListener listener = (TcpListener)ar.AsyncState;
            TcpClient client = listener.EndAcceptTcpClient(ar);
            listener.BeginAcceptTcpClient(AcceptClientCallback, listener); // Tiếp tục lắng nghe các kết nối mới

            using (NetworkStream stream = client.GetStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                string domain = reader.ReadLine();
                if (!string.IsNullOrEmpty(domain))
                {
                    OpenBrowser(domain);
                }
            }

            client.Close();
        }
        private void OpenBrowser(string domain)
        {
            try
            {
                // Mở trang web trong Google Chrome hoặc trình duyệt mặc định
                string url = "https://" + domain;
                System.Diagnostics.Process.Start("chrome", url); // Sử dụng Google Chrome
                                                                 // Nếu không có Chrome, bạn có thể dùng: System.Diagnostics.Process.Start(url);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở trình duyệt: {ex.Message}");
            }
        }






    }
}
