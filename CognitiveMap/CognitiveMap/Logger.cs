using System;
using System.Collections.Generic;
using System.Text;

namespace CognitiveMap
{
    class Logger
    {
        int PosTop = 0;
        public Logger()
        {
            PosTop = Console.CursorTop;
            Console.WriteLine(PosTop);
            Console.SetCursorPosition(0, PosTop);
        }
        public void Log(int epoch, int numEpochs, double avrgCost, double minCost, double maxCost)
        {
            if (((epoch % 10) == 0 || (epoch == 1)) && epoch > 0)
            {
                Console.SetCursorPosition(0, (epoch / 50) - 1 + PosTop);
                Console.WriteLine(String.Format("[avrg, min] = [{0:00.00}, {1:00.00}] <- cost, epoch = {2}", avrgCost, minCost, epoch));
            }
            Console.SetCursorPosition(0, (epoch / 50) + PosTop);
            Console.WriteLine(String.Format("[avrg, min, max] = [{0:00.00}, {1:00.00}, {2:00.00}] <- cost  ", avrgCost, minCost, maxCost));
            drawProgressBar(epoch, numEpochs);
        }

        void drawProgressBar(int epoch, int numEpochs)
        {
            double progress = (double)epoch / numEpochs;
            const int length = 10;
            StringBuilder outputBuilder = new StringBuilder();
            outputBuilder.Append('[');
            outputBuilder.Append('#', (int)(progress * length));
            outputBuilder.Append('-', (int)Math.Ceiling((1.0d - progress) * length));
            outputBuilder.Append(String.Format("] <=> {0}% [{1}/{2}] epochs", (int)(progress * 100), epoch, numEpochs));
            Console.Write(outputBuilder);
        }
    }
}
