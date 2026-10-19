using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Interlock_BigPractise_Task3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Bridge bridge = new Bridge();
            Random rnd = new Random();
            int total = 20;

            Thread[] threads = new Thread[total];

            for (int i = 0; i < total; i++)
            {
                int id = i + 1;
                int side = rnd.Next(0, 2);

                threads[i] = new Thread(() =>
                {
                    Thread.Sleep(rnd.Next(100, 801));

                    Car car = new Car();
                    car.id = id;
                    car.side = side;

                    bridge.Enter(car);
                    bridge.Leave(car);
                });
            }

            foreach (var t in threads) t.Start();
            foreach (var t in threads) t.Join();

            Console.WriteLine("\n================ СТАТИСТИКА ================");
            Console.WriteLine("Змін напрямку: " + bridge.changes);

            if (bridge.leftTimes.Count > 0)
                Console.WriteLine("Лівий - Середній час: " + bridge.leftTimes.Average().ToString("0.0") + " мс | Макс: " + bridge.leftTimes.Max().ToString("0.0") + " мс");

            if (bridge.rightTimes.Count > 0)
                Console.WriteLine("Правий - Середній час: " + bridge.rightTimes.Average().ToString("0.0") + " мс | Макс: " + bridge.rightTimes.Max().ToString("0.0") + " мс");
        }
    }
}
