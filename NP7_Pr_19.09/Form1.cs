using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace NP7_Pr_19._09
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        UdpClient multicastclient;
        IPAddress multicastIP = null;
        List<IPEndPoint> clients = new List<IPEndPoint>();
        int multicastport = 2024;
        async Task ListenerAsync()
        {
            while (true)
            {
                if (multicastclient == null)
                    break;
                try
                {
                    var remoteIP = new IPEndPoint(IPAddress.Any, 2024);
                    //byte[] buf = new byte[1024];
                    UdpReceiveResult data = await multicastclient.ReceiveAsync();
                    string mes = Encoding.Unicode.GetString(data.Buffer);
                    if (mes.Contains("connect"))
                    {
                        clients.Add(remoteIP);
                    }
                    else
                    {
                        foreach (var client in clients)
                        {
                            await multicastclient.SendAsync(data.Buffer, client);
                        }
                    }
                    //textBox3.BeginInvoke(new TextDelegate(UpdateTextBox), mes);
                    MessageBox.Show(data.RemoteEndPoint.ToString());


                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    throw;
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            multicastIP = IPAddress.Parse("224.0.0.1");
            multicastclient = new UdpClient();
            multicastclient.JoinMulticastGroup(multicastIP, 2);
            multicastclient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            IPEndPoint local = new IPEndPoint(IPAddress.Parse("192.168.0.103"), 2024);
            //multicastclient.Client.Bind(local);
            multicastclient.MulticastLoopback = true;
            //Task.Run(async () => await ListenerAsync());
            button2.Enabled = true;
            button3.Enabled = true;
            textBox1.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ClientChat form = new ClientChat(multicastclient);
            form.Show();
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            string text = textBox1.Text;
            var msg = Encoding.Unicode.GetBytes(text);
            await multicastclient.SendAsync(msg, new IPEndPoint(multicastIP, multicastport));
            textBox1.Text = "";
        }
    }
}
