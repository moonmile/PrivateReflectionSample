using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Type t = typeof(string).GetType();
            t.GetFields(BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic);

        }
    }
}
