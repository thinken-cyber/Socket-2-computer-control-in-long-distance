using System;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace dieukhientuxa_tatmay_client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnSendCommand_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                string serverIp = txtServerIp.Text.Trim();
                if (string.IsNullOrEmpty(serverIp))
                {
                    MessageBox.Show("Vui lòng nhập địa chỉ IP của Server!", "Lỗi đầu vào");
                    return;
                }

                if (!int.TryParse(txtPort.Text.Trim(), out int port) || port <= 0 || port > 65535)
                {
                    MessageBox.Show("Vui lòng nhập port hợp lệ (1-65535)!", "Lỗi đầu vào");
                    return;
                }

                string command = txtCommand.Text.Trim();
                if (string.IsNullOrEmpty(command))
                {
                    MessageBox.Show("Vui lòng nhập lệnh cần gửi!", "Lỗi đầu vào");
                    return;
                }

                // Gửi lệnh đến Server
                using (TcpClient client = new TcpClient())
                {
                    client.Connect(serverIp, port); // Kết nối tới Server
                    using (NetworkStream stream = client.GetStream())
                    {
                        byte[] data = Encoding.UTF8.GetBytes(command);
                        stream.Write(data, 0, data.Length); // Gửi dữ liệu

                        MessageBox.Show($"Lệnh '{command}' đã được gửi thành công!", "Thông báo");
                    }
                }
            }
            catch (SocketException ex)
            {
                MessageBox.Show($"Không thể kết nối tới Server: {ex.Message}", "Lỗi kết nối");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi");
            }
        }
    }
}
