using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            var s = DateTime.Now.Ticks.ToString();
            Console.WriteLine(s);
            Console.ReadLine();
        }
    }
}
