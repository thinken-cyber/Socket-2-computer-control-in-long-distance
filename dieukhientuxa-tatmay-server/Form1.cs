using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace dieukhientuxa_tatmay_server
{
    public partial class Form1 : Form
    {
        private TcpListener server;
        private Thread listenerThread;
        private bool isServerRunning;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnStartServer_Click(object sender, EventArgs e)
        {
            try
            {
                int port = int.Parse(txtPort.Text.Trim()); // Lấy port từ giao diện
                server = new TcpListener(IPAddress.Any, port);
                server.Start();

                isServerRunning = true;
                listenerThread = new Thread(ListenForClients) { IsBackground = true };
                listenerThread.Start();

                AddLog("Server đã khởi động và đang lắng nghe...");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi khởi động server: {ex.Message}", "Lỗi");
            }
        }

        private void ListenForClients()
        {
            try
            {
                while (isServerRunning)
                {
                    if (!server.Pending())
                    {
                        Thread.Sleep(100); // Giảm tải CPU khi không có kết nối
                        continue;
                    }

                    using (TcpClient client = server.AcceptTcpClient())
                    using (NetworkStream stream = client.GetStream())
                    {
                        byte[] buffer = new byte[1024];
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);

                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                        AddLog($"[{DateTime.Now}] Nhận lệnh: {message}");

                        if (message.Equals("shutdown", StringComparison.OrdinalIgnoreCase))
                        {
                            AddLog($"[{DateTime.Now}] Lệnh 'shutdown' được nhận. Đang thực hiện tắt máy...");
                            ShutdownMachine();
                        }
                        else
                        {
                            AddLog($"[{DateTime.Now}] Lệnh không hợp lệ: {message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (isServerRunning) // Chỉ log lỗi nếu server đang chạy
                {
                    AddLog($"Lỗi trong vòng lặp lắng nghe: {ex.Message}");
                }
            }
        }

        private void ShutdownMachine()
        {
            try
            {
                Process.Start("shutdown", "/s /t 0"); // Lệnh tắt máy ngay lập tức
            }
            catch (Exception ex)
            {
                AddLog($"[{DateTime.Now}] Lỗi khi tắt máy: {ex.Message}");
            }
        }

        private void btnStopServer_Click(object sender, EventArgs e)
        {
            try
            {
                isServerRunning = false;
                server?.Stop();
                listenerThread?.Join(); // Đợi thread dừng an toàn
                AddLog($"[{DateTime.Now}] Server đã dừng!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi dừng server: {ex.Message}", "Lỗi");
            }
        }

        private void AddLog(string logMessage)
        {
            if (lstLog.InvokeRequired)
            {
                lstLog.Invoke(new Action(() => lstLog.Items.Add(logMessage)));
            }
            else
            {
                lstLog.Items.Add(logMessage);
            }
        }
    }
}
