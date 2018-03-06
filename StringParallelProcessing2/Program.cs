using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringParallelProcessing2
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<int, TimeSpan> parallelProcessingTimes = new Dictionary<int, TimeSpan>();
            TimeSpan processingTime;

            int logicalProcessors = Environment.ProcessorCount;

            int items = 100000; //100,000
            int iterations = 100;
            Stopwatch stopwatch = new Stopwatch();

            StringProcessor stringProcessor = new StringProcessor();
            List<string> testData = GenerateTestData(items);

            /*Console.WriteLine("Single Threaded: ");
            stringProcessor.ProcessStrings(testData); //Warmup
            for (int i = 0; i < iterations; i++)
            {
                Console.SetCursorPosition(4, 1);
                Console.Write(i+1);
                stopwatch.Start();
                stringProcessor.ProcessStrings(testData);
                stopwatch.Stop();
            }

            Console.Clear();
            processingTime = stopwatch.Elapsed;
            stopwatch.Reset();

            for(int i = 0; i < logicalProcessors; i++)
            {
                Console.SetCursorPosition(0, 0);
                Console.WriteLine($"Multi Threaded: {i+1} processors");
                stringProcessor.ProcessStringsParallel(testData); //Warmup
                for (int j = 0; j < iterations; j++)
                {
                    Console.SetCursorPosition(4, 1);
                    Console.Write(j + 1);
                    stopwatch.Start();
                    stringProcessor.ProcessStringsParallel(testData, i+1);
                    stopwatch.Stop();
                }
                Console.WriteLine();
                parallelProcessingTimes.Add(i + 1, stopwatch.Elapsed);
                stopwatch.Reset();
                Console.Clear();
            }*/

            Console.SetCursorPosition(0, 0);
            Console.WriteLine($"Multi Threaded: 8 processors");
            stringProcessor.ProcessStringsParallel(testData); //Warmup
            for (int j = 0; j < iterations; j++)
            {
                Console.SetCursorPosition(4, 1);
                Console.Write(j + 1);
                stopwatch.Start();
                stringProcessor.ProcessStringsParallel(testData, 8);
                stopwatch.Stop();
            }
            Console.WriteLine();
            parallelProcessingTimes.Add(8, stopwatch.Elapsed);
            stopwatch.Reset();
            Console.Clear();


            //Console.WriteLine($"Single Threaded: {processingTime.TotalMilliseconds / iterations}ms Per Loop");
            foreach (var time in parallelProcessingTimes)
            {
                Console.WriteLine($"Multi Threaded {time.Key} Processors: {time.Value.TotalMilliseconds / iterations}ms Per Loop");
            }


            Console.ReadLine();
        }

        static List<string> GenerateTestData(int iterations)
        {
            List<string> output = new List<string>();
            for (int i = 0; i < iterations; i++)
            {
                output.Add(" This is a test string. /n With new lines. /n Seperating it. /n");
            }
            return output;
        }
    }
}
