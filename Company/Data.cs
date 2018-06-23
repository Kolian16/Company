using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company
{
   public static class Data
    {
        public delegate void MyEvent(int number);
        public static MyEvent EventHandler;
        public delegate void MyEventSalary(string text);
        public static MyEventSalary Salary;
    }
}
