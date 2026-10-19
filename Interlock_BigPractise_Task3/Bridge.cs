using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Interlock_BigPractise_Task3
{
    internal class Bridge
    {
        private object locker = new object();
        private int onBridge = 0;
        private int currentDir = -1;
        private int passedCount = 0;
        private int leftWait = 0;
        private int rightWait = 0;
        public int changes = 0;
        public List<double> leftTimes = new List<double>();
        public List<double> rightTimes = new List<double>();

        public void Enter(Car car)
        {
            DateTime start = DateTime.Now;

            lock (locker)
            {
                if (car.side == 0) leftWait++;
                else rightWait++;

                while (true)
                {
                    bool empty = (onBridge == 0);
                    bool otherSideWaiting = (car.side == 0 && rightWait > 0) || (car.side == 1 && leftWait > 0);

                    if (empty)
                    {
                        if (currentDir != -1 && currentDir != car.side)
                            changes++;

                        currentDir = car.side;
                        passedCount = 0;
                        break;
                    }

                    bool sameDirection = (currentDir == car.side);
                    bool hasSpace = (onBridge < 3);
                    bool underLimit = (!otherSideWaiting || passedCount < 5);

                    if (sameDirection && hasSpace && underLimit)break;

                    Monitor.Wait(locker);
                }

                if (car.side == 0) leftWait--;
                else rightWait--;

                int cnt = Interlocked.Increment(ref onBridge);
                if (cnt > 3) Console.WriteLine("АВАРІЯ!");

                passedCount++;

                car.waitTime = (DateTime.Now - start).TotalMilliseconds;
                if (car.side == 0) leftTimes.Add(car.waitTime);
                else rightTimes.Add(car.waitTime);

                string s = (car.side == 0) ? "Лівий" : "Правий";
                string t = DateTime.Now.ToString("HH:mm:ss.fff");
                Console.WriteLine("[" + t + "] Авто #" + car.id + " (" + s + ") чекало " + car.waitTime.ToString("0") + " мс, на мосту");
            }
        }

        public void Leave(Car car)
        {
            Thread.Sleep(500);

            lock (locker)
            {
                Interlocked.Decrement(ref onBridge);

                string s = (car.side == 0) ? "Лівий" : "Правий";
                string t = DateTime.Now.ToString("HH:mm:ss.fff");
                Console.WriteLine("[" + t + "] Авто #" + car.id + " (" + s + ") з'їхало");

                if (onBridge == 0) currentDir = -1;

                Monitor.PulseAll(locker);
            }
        }
    }
}
