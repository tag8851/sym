using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sym
{
    class Program
    {
        static void Main(string[] args)
        {
            Alg1 alg = new Alg1();
            //Alg2 alg = new Alg2();
            //Alg3 alg = new Alg3();

            alg.Init();

            alg.Run();

            alg.Terminate();
        }
    }
}
