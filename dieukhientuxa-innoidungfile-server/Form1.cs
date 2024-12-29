using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace dieukhientuxa_inchutrenmanhinh_server
{
    public partial class Form1 : Form
    {
        private TcpListener server;
        private Thread serverThread;
        private bool isServerRunning = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnStartServer_Click(object sender, EventArgs e)
        {
            // Kiểm tra port hợp lệ
            if (!int.TryParse(txtPort.Text.Trim(), out int port) || port <= 0 || port > 65535)
            {
                MessageBox.Show("Vui lòng nhập port hợp lệ (1 - 65535).");
                return;
            }

            serverThread = new Thread(() => StartServer(port))
            {
                IsBackground = true // Đảm bảo thread tự động dừng khi ứng dụng thoát
            };
            serverThread.Start();
            btnStartServer.Enabled = false;
        }

        private void StartServer(int port)
        {
            try
            {
                server = new TcpListener(IPAddress.Any, port);
                server.Start();
                isServerRunning = true;

                Invoke(new Action(() =>
                {
                    lstConnections.Items.Add($"Server started on port {port}. Waiting for connections...");
                }));

                while (isServerRunning)
                {
                    // Chấp nhận kết nối từ client
                    TcpClient client = server.AcceptTcpClient();

                    Invoke(new Action(() =>
                    {
                        lstConnections.Items.Add("Client connected: " + client.Client.RemoteEndPoint);
                    }));

                    // Xử lý client kết nối trong một thread riêng
                    Thread clientThread = new Thread(() => HandleClient(client))
                    {
                        IsBackground = true
                    };
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                if (isServerRunning) // Chỉ hiển thị lỗi khi server đang chạy
                {
                    Invoke(new Action(() =>
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }));
                }
            }
            finally
            {
                server?.Stop();
                isServerRunning = false;
                Invoke(new Action(() =>
                {
                    lstConnections.Items.Add("Server stopped.");
                    btnStartServer.Enabled = true;
                }));
            }
        }

        private void HandleClient(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                // Đọc tên file từ client (bao gồm cả phần mở rộng)
                byte[] fileNameBuffer = new byte[1024];
                int bytesRead = stream.Read(fileNameBuffer, 0, fileNameBuffer.Length);
                string fileName = Encoding.UTF8.GetString(fileNameBuffer, 0, bytesRead).Trim();

                // Cập nhật giao diện với tên file nhận được
                Invoke(new Action(() =>
                {
                    txtFileName.Text = fileName;
                }));

                // Đường dẫn thư mục bạn muốn tìm file
                string searchDirectory = @"E:\Files";
                string response;

                try
                {
                    // Tìm file trong thư mục chỉ định (bao gồm thư mục con)
                    string[] files = Directory.GetFiles(searchDirectory, fileName, SearchOption.AllDirectories);

                    if (files.Length > 0)
                    {
                        // Nếu file được tìm thấy, đọc nội dung file
                        string fileContent = File.ReadAllText(files[0]);
                        response = fileContent;
                    }
                    else
                    {
                        // Nếu không tìm thấy file
                        response = "File not found!";
                    }
                }
                catch (Exception ex)
                {
                    response = "Error: " + ex.Message;
                }

                // Gửi kết quả về client
                byte[] responseBuffer = Encoding.UTF8.GetBytes(response);
                stream.Write(responseBuffer, 0, responseBuffer.Length);

                // Cập nhật kết quả trên giao diện
                Invoke(new Action(() =>
                {
                    txtResponse.Text = response;
                }));

                // Đóng kết nối
                client.Close();
            }
            catch (Exception ex)
            {
                Invoke(new Action(() =>
                {
                    MessageBox.Show("Error handling client: " + ex.Message);
                }));
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Dừng server khi đóng form
            if (isServerRunning)
            {
                isServerRunning = false;
                server?.Stop();
            }
        }
    }
}
