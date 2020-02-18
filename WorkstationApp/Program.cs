using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Drawing;  
using System.Drawing.Imaging;

namespace WorkstationApp { 
    //https://proglib.io/p/kak-upravlyat-kompyuterom-so-smartfona-po-wi-fi-pishem-android-prilozhenie-na-s-2020-02-17
    class Program {
        public static TcpClient client;
        private static TcpListener listener;
        private static string ipString;

        static void Main(string[] args) {
            IPEndPoint ep = PortListener();
            Console.WriteLine(
                "===================================================\n" +
                $"Started listening requests at: {ep.Address}:{ep.Port}\n" +
                "===================================================");
            client = listener.AcceptTcpClient();
            Console.WriteLine("Connected to client!" + " \n");

            ClientConnect();
            GetScreenshot();
        }

        public static string GetMachineIp() {
            IPAddress[] localIp = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress address in localIp) {
                if (address.AddressFamily == AddressFamily.InterNetwork) {
                    ipString = address.ToString();
                }
            }

            return ipString;
        }

        public static IPEndPoint PortListener() {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(GetMachineIp()), 1234);
            listener = new TcpListener(ep);
            listener.Start();

            return ep;
        }

        public static void ClientConnect() {
            try {
                const int bytesize = 1024 * 1024;
                byte[] buffer = new byte[bytesize];
                string x = client.GetStream().Read(buffer, 0, bytesize).ToString();
                var data = ASCIIEncoding.ASCII.GetString(buffer);
                if (data.ToUpper().Contains("SLP2")) {
                    Console.WriteLine("Pc is going to Sleep Mode!" + " \n");
                    Sleep();
                }
            }
            catch (Exception exc) {
                client.Dispose();
                client.Close();
            }
        }

        static void Sleep() {  
            Application.SetSuspendState(PowerState.Suspend, true, true);  
        }

        public static void GetScreenshot() {
            while (client.Connected) {  
                try {  
                    const int bytesize = 1024 * 1024;  
                    byte[] buffer = new byte[bytesize];  
                    string x = client.GetStream().Read(buffer, 0, bytesize).ToString();  
                    string data = ASCIIEncoding.ASCII.GetString(buffer);  
                    if (data.ToUpper().Contains("SLP2")) {  
                        Console.WriteLine("Pc is going to Sleep Mode!" + " \n");  
                        Sleep();  
                    }  
                    else if (data.ToUpper().Contains("SHTD3")) {  
                        Console.WriteLine("Pc is going to Shutdown!" + " \n");  
                        Shutdown();  
                    } else if (data.ToUpper().Contains("TSC1")) {  
                        Console.WriteLine("Take Screenshot!" + " \n");  
                        var bitmap = SaveScreenshot();  
                        var stream = new MemoryStream();  
                        bitmap.Save(stream, ImageFormat.Bmp);  
                        sendData(stream.ToArray(), client.GetStream());  
                    }
                } catch (Exception exc) {  
                    client.Dispose();  
                    client.Close();  
                }
            } 
        }

        static void Shutdown() {  
            System.Diagnostics.Process.Start("Shutdown", "-s -t 10");  
        }  

        static Bitmap SaveScreenshot() {  
            var bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format32bppArgb);  
            var gfxScreenshot = Graphics.FromImage(bmpScreenshot);  
            gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);  
            return bmpScreenshot;  
        }  

        // Convert image to bytecode
        static void sendData(byte[] data, NetworkStream stream) {  
            int bufferSize = 1024;  
            byte[] dataLength = BitConverter.GetBytes(data.Length);  
            stream.Write(dataLength, 0, 4);  
            int bytesSent = 0;  
            int bytesLeft = data.Length;  
            while (bytesLeft > 0) {  
                int curDataSize = Math.Min(bufferSize, bytesLeft);  
                stream.Write(data, bytesSent, curDataSize);  
                bytesSent += curDataSize;  
                bytesLeft -= curDataSize;  
            }  
        } 
    }
}