using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinFormsClient.Client;

namespace XamarinFormsClient
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OperationsPage : ContentPage
    {
        public OperationsPage()
        {
            InitializeComponent();
        }

        // Sleep
        private void Sleep_Clicked(object sender, EventArgs e)  
        {  
            TcpClient client = Connection.Instance.client;  
            NetworkStream stream = client.GetStream();  
            String s = "SLP2";  
            byte[] message = Encoding.ASCII.GetBytes(s);  
            stream.Write(message, 0, message.Length);  
        }  
 
        // Shutdown
        private void Shutdown_Clicked(object sender, EventArgs e)  
        {  
            TcpClient client = Connection.Instance.client;  
            NetworkStream stream = client.GetStream();  
            String s = "SHTD3";  
            byte[] message = Encoding.ASCII.GetBytes(s);  
            stream.Write(message, 0, message.Length);  
        }  
 
        // Screenshot
        private void Screenshot_Clicked(object sender, EventArgs e)  
        {  
            TcpClient client = Connection.Instance.client;  
            NetworkStream stream = client.GetStream();  
            String s = "TSC1";  
            byte[] message = Encoding.ASCII.GetBytes(s);  
            stream.Write(message, 0, message.Length);  
            byte[] data = getData(client);  
            imageView.Source = ImageSource.FromStream(() => new MemoryStream(data));  
        }  
  
        // Get data from server
        public byte[] getData(TcpClient client)  
        {  
            NetworkStream stream = client.GetStream();  
            byte[] fileSizeBytes = new byte[4];  
            int bytes = stream.Read(fileSizeBytes, 0, fileSizeBytes.Length);  
            int dataLength = BitConverter.ToInt32(fileSizeBytes, 0);  
  
            int bytesLeft = dataLength;  
            byte[] data = new byte[dataLength];  
  
            int buffersize = 1024;  
            int bytesRead = 0;  
  
            while (bytesLeft > 0)  
            {  
                int curDataSize = Math.Min(buffersize, bytesLeft);  
                if (client.Available < curDataSize)  
                    curDataSize = client.Available;  
                bytes = stream.Read(data, bytesRead, curDataSize);  
                bytesRead += curDataSize;  
                bytesLeft -= curDataSize;  
            }  
            return data;  
        }  
    }
}