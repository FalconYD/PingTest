using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using FalconWpf;

namespace PingTest
{
    public class DataResource
    {
        private string strIPAdd = "127.0.0.1";
        public string IPAdd
        {
            get { return strIPAdd; }
            set { strIPAdd = value; }
        }

        private int nPingDelay = 10000;
        public int PingDelay
        {
            get { return nPingDelay; }
            set { nPingDelay = value; }
        }
    }
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        Thread m_thPing;
        LogClass mc_Log = new LogClass();
        Queue<string> m_queMsg = new Queue<string>();
        DataResource resource = new DataResource();
        Ping ping = new Ping();
        PingOptions options = new PingOptions();
        PingReply reply;
        public MainWindow()
        {
            InitializeComponent();
            if (!XmlManager.LoadXml("PingTest.xml", resource))
            {
                XmlManager.SaveXml("PingTest.xml", resource);
            }
            options.DontFragment = true;
            m_thPing = new Thread(new ThreadStart(Th_Ping));
            m_thPing.Start();
            WriteLog($"Program Start. IP : {resource.IPAdd}, Ping Delay : {resource.PingDelay}");
        }

        public void WriteLog(string strMsg)
        {
            strMsg = strMsg.Insert(0, $"[{DateTime.Now:yyyy.MM.dd_HH:mm:ss}] ");
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                listMessage.Items.Add(strMsg);
                if (listMessage.Items.Count > 500)
                    listMessage.Items.RemoveAt(0);
                listMessage.ScrollIntoView(listMessage.Items[listMessage.Items.Count - 1]);
            }));
            mc_Log.PushLog(strMsg);
        }

        private void Th_Ping()
        {
            while (true)
            {
                Thread.Sleep(resource.PingDelay);
                fn_PingTest();
            }
        }

        private void fn_PingTest()
        {
            try
            {
                //전송할 데이터를 입력
                byte[] buffer = ASCIIEncoding.ASCII.GetBytes("This_Ping_Test_Program.");

                reply = ping.Send(IPAddress.Parse(resource.IPAdd), (int)(resource.PingDelay / 2.0), buffer, options);

                WriteLog($"{reply.Status.ToString()}, {reply.RoundtripTime} ms");
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mc_Log.Destroy();
            if (m_thPing.IsAlive)
                m_thPing.Abort();
        }
    }
}
