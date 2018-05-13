using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace StringParallelProcessing
{
    class Program
    {
        static void Main(string[] args)
        {
            List<TimeRecord> parallelProcessingTimes = new List<TimeRecord>();
            TimeSpan processingTime;

            int logicalProcessors = Environment.ProcessorCount;

            int items = 100000; //100,000
            int iterations = 10;
            Stopwatch stopwatch = new Stopwatch();

            StringProcessor stringProcessor = new StringProcessor();
            List<string> testData = GenerateTestData(items);

            Console.WriteLine("Single Threaded: ");
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
                Console.WriteLine($"Multi Threaded Type 1: {i+1} processors");
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
                parallelProcessingTimes.Add(new TimeRecord(i + 1, 1 , stopwatch.Elapsed));
                stopwatch.Reset();
                Console.Clear();
            }

            for (int i = 0; i < logicalProcessors; i++)
            {
                Console.SetCursorPosition(0, 0);
                Console.WriteLine($"Multi Threaded Type 2: {i + 1} processors");
                stringProcessor.ProcessStringsParallel(testData); //Warmup
                for (int j = 0; j < iterations; j++)
                {
                    Console.SetCursorPosition(4, 1);
                    Console.Write(j + 1);
                    stopwatch.Start();
                    stringProcessor.ProcessStringsParallel2(testData, i + 1);
                    stopwatch.Stop();
                }
                Console.WriteLine();
                parallelProcessingTimes.Add(new TimeRecord(i + 1, 2, stopwatch.Elapsed));
                stopwatch.Reset();
                Console.Clear();
            }

            for (int i = 0; i < logicalProcessors; i++)
            {
                Console.SetCursorPosition(0, 0);
                Console.WriteLine($"Multi Threaded Type 3: {i + 1} processors");
                stringProcessor.ProcessStringsParallel(testData); //Warmup
                for (int j = 0; j < iterations; j++)
                {
                    Console.SetCursorPosition(4, 1);
                    Console.Write(j + 1);
                    stopwatch.Start();
                    stringProcessor.ProcessStringsParallel3(testData, i + 1);
                    stopwatch.Stop();
                }
                Console.WriteLine();
                parallelProcessingTimes.Add(new TimeRecord(i + 1, 3, stopwatch.Elapsed));
                stopwatch.Reset();
                Console.Clear();
            }


            Console.WriteLine($"Single Threaded: {processingTime.TotalMilliseconds/iterations}ms Per Loop");
            Console.WriteLine();
            int currentVersion = 1;
            foreach(TimeRecord item in parallelProcessingTimes)
            {
                if(item.Version != currentVersion)
                {
                    Console.WriteLine();
                    currentVersion = item.Version;
                }
                Console.WriteLine($"Multi Threaded v{item.Version} {item.Processors} Processors: {item.Time.TotalMilliseconds / iterations}ms Per Loop");
            }
            

            Console.ReadLine();
        }

        static List<string> GenerateTestData(int iterations)
        {
            List<string> output = new List<string>();
            for(int i = 0; i < iterations; i++)
            {
                output.Add(" This is a test string. /n With new lines. /n Seperating it. /n");
            }
            return output;
        }
    }

    public class TimeRecord
    {
        public TimeRecord(int processors, int version, TimeSpan time)
        {
            Processors = processors;
            Version = version;
            Time = time;
        }

        public int Processors { get; set; }
        public int Version { get; set; }
        public TimeSpan Time { get; set; }
    }
}
