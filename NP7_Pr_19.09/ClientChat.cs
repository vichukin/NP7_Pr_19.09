using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace NP7_Pr_19._09
{
    public partial class ClientChat : Form
    {
        public delegate void TextDelegate(string t);
        UdpClient multiCastClient;
        UdpClient udpClient;
        void UpdateTextBox(string t)
        {
            StringBuilder sb = new StringBuilder(textBox2.Text);
            sb.AppendLine(t);
            textBox2.Text = sb.ToString();
        }
        public ClientChat(UdpClient client)
        {
            InitializeComponent();
            multiCastClient = client;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var MulticastIp = IPAddress.Parse("224.0.0.1");
            //var buf = Encoding.Unicode.GetBytes("connect");
            //multiCastClient.SendAsync(buf,new IPEndPoint(MulticastIp,2024));
            udpClient = new UdpClient();
            udpClient.JoinMulticastGroup(MulticastIp);
            //IPAddress ip = IPAddress.Any;
            //udpClient.Client.Bind( new IPEndPoint( ip, 2024));
            udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, 2024));
            Task.Run(async () => await ListenerAsync());
            MessageBox.Show("connected");
            button1.Enabled = false;

        }
        async Task ListenerAsync()
        {
            while (true)
            {
                if (udpClient == null)
                    break;
                try
                {
                    var remoteIP = new IPEndPoint(IPAddress.Parse("192.168.0.103"), 2024);
                    //byte[] buf = new byte[1024];
                    UdpReceiveResult data = await udpClient.ReceiveAsync();
                    string mes = Encoding.Unicode.GetString(data.Buffer);
                    mes = $"Message from server: {mes}";
                    textBox2.BeginInvoke(new TextDelegate(UpdateTextBox), mes);
                    //MessageBox.Show(mes);


                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    throw;
                }
            }
        }
    }
}
