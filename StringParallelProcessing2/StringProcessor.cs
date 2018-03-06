using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StringParallelProcessing2
{
    public class StringProcessor
    {
        public List<string> ProcessStringsParallel(List<string> input, int? maxParallelism = null)
        {
            List<string> output = new List<string>();
            Parallel.ForEach<string, List<string>>(
                input,
                GetParallelOptions(maxParallelism),
                () => new List<string>(),
                (item, loop, local) =>
                {
                    //local.Add(ProcessString(item));
                    local.Add(ProcessString2(item));
                    return local;
                },
                (result) =>
                {
                    lock (output)
                    {
                        output.AddRange(result);
                    }
                }
            );

            return output;
        }

        private ParallelOptions GetParallelOptions(int? maxParallelism)
        {
            if (maxParallelism == null)
            {
                return new ParallelOptions();
            }
            return new ParallelOptions()
            {
                MaxDegreeOfParallelism = maxParallelism.Value
            };
        }


        public List<string> ProcessStrings(List<string> input)
        {
            List<string> output = new List<string>();
            foreach (string item in input)
            {
                //output.Add(ProcessString(item));
                output.Add(ProcessString2(item));
            }
            return output;
        }


        public string ProcessString(string input)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '\n')
                {
                    builder.Append('\r');
                }
                builder.Append(input[i]);
            }
            return builder.ToString();
        }

        public string ProcessString2(string input)
        {
            char[] charArray = new char[(int)(input.Length * 1.1)];
            int location = 0;

            for (int i = 0; i < input.Length; i++)
            {
                if(charArray.Length + 2 <= i)
                {
                    Array.Resize<char>(ref charArray, (int)(charArray.Length * 1.1));
                }

                if (input[i] == '\n')
                {
                    charArray[location] = '\r';
                    location++;
                }
                charArray[location] = input[i];
                location++;
            }
            return new string(charArray);
        }
    }
}
