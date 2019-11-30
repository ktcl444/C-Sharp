using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;

namespace WindowsFormsApplication1
{
    public class TestMainPost
    {
        private static ArrayList threads = new ArrayList();
        public static Boolean isFinished = false;
        private static string url = "http://192.168.17.128:1000/";

        public string getUrl()
        {
            try
            {
                lock (threads)
                {
                    if (threads.Count > 0)
                    {
                        string tmp = threads[0].ToString();
                        threads.RemoveAt(0);
                        return tmp;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception)
            {

                return null;
            }
        }

        public void Process()
        {
            int max = 100;
            for (int i = 0; i < max; i++)
            {
                //new Thread(new Testsimulator)
            }
        }
  
    }
}
