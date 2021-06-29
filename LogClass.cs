using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PingTest
{
    class LogClass
    {
        private Queue<string> m_queMsg = new Queue<string>();
        private Thread m_thLog;
        private string m_strPath = "";
        public string SavePath
        {
            get { return m_strPath; }
            set { m_strPath = value; }
        }
        public LogClass()
        {
            m_thLog = new Thread(new ThreadStart(th_Log));
            m_thLog.Start();
        }

        public void Destroy()
        {
            if (m_thLog.IsAlive)
            {
                m_thLog.Abort(100);
            }
        }

        public void PushLog(string strMsg)
        {
            m_queMsg.Enqueue(strMsg);
        }

        private void WriteLog(string strMsg)
        {
            string strName = $"{DateTime.Now:yyyyMMdd}.txt";
            if (m_strPath.LastIndexOf('\\') < m_strPath.Length - 2)
                m_strPath += "\\";

            StreamWriter sw = new StreamWriter(m_strPath + strName, true);
            sw.WriteLine(strMsg);
            sw.Flush();
            sw.Close();
        }

        private void th_Log()
        {
            while (true)
            {
                Thread.Sleep(10);
                if(m_queMsg.Count > 0)
                {
                    WriteLog(m_queMsg.Dequeue());
                }
            }
        }
        
    }
}
