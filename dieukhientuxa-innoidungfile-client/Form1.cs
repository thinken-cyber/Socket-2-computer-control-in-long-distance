using System;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace dieukhientuxa_inchutrenmanhinh_client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnSendFile_Click(object sender, EventArgs e)
        {
            string serverIp = txtServerIP.Text.Trim();

            // Kiểm tra IP server có được nhập
            if (string.IsNullOrWhiteSpace(serverIp))
            {
                MessageBox.Show("Vui lòng nhập địa chỉ IP của server.");
                return;
            }

            // Kiểm tra port hợp lệ
            if (!int.TryParse(txtPort.Text.Trim(), out int port) || port <= 0 || port > 65535)
            {
                MessageBox.Show("Vui lòng nhập port hợp lệ (1 - 65535).");
                return;
            }

            string fileName = txtFileName.Text.Trim();

            // Kiểm tra tên file không rỗng
            if (string.IsNullOrWhiteSpace(fileName))
            {
                MessageBox.Show("Vui lòng nhập tên file.");
                return;
            }

            try
            {
                // Kết nối đến server
                using (TcpClient client = new TcpClient(serverIp, port))
                {
                    NetworkStream stream = client.GetStream();

                    // Gửi tên file đến server
                    byte[] data = Encoding.UTF8.GetBytes(fileName);
                    stream.Write(data, 0, data.Length);

                    // Nhận phản hồi từ server
                    byte[] buffer = new byte[4096]; // Tăng kích thước buffer để nhận dữ liệu lớn hơn
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);

                    if (bytesRead > 0)
                    {
                        string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        txtResponse.Text = response;
                    }
                    else
                    {
                        txtResponse.Text = "Không nhận được phản hồi từ server.";
                    }
                }
            }
            catch (SocketException ex)
            {
                MessageBox.Show($"Không thể kết nối đến server. Kiểm tra địa chỉ IP và port.\nLỗi: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}");
            }
        }
    }
}
