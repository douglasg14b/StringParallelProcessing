using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StringParallelProcessing
{
    public class StringProcessor
    {
        public List<string> ProcessStringsParallel(List<string> input, int? maxParallelism = null)
        {
            List<string> output = new List<string>();
            Parallel.ForEach<string, LoopParams>(
                input,
                GetParallelOptions(maxParallelism),
                () => 
                {
                    return new LoopParams(new List<string>(), new char[1]);
                },
                (item, loop, local) =>
                {
                    char[] chars = local.CharArray;
                    //local.Add(ProcessString(item));
                    local.Strings.Add(ProcessString3(item, ref chars));
                    if(!Object.ReferenceEquals(local.CharArray, chars))
                    {
                        local.CharArray = chars;
                    }
                    return local;
                },
                (result) =>
                {
                    lock (output)
                    {
                        output.AddRange(result.Strings);
                    }
                }
            );

            return output;
        }

        public List<string> ProcessStringsParallel2(List<string> input, int? maxParallelism = null)
        {
            List<string> output = new List<string>(input.Count);
            int count = maxParallelism ?? 1;
            int segmentSize = (int)input.Count / count;
            Task< List<string>>[] tasks = new Task<List<string>>[count];
            

            for (int i = 0; i < count; i++)
            {
                int segmentsStart = segmentSize * i;
                int segmentsEnd = segmentSize * (i + 1);
                if(i == count - 1)
                {
                    segmentsEnd += input.Count - segmentsEnd - 1;
                }
                Task<List<string>> newTask = Task.Factory.StartNew<List<string>>(() =>
                {
                    List<string> taskOutput = new List<string>(segmentsEnd - segmentsStart);
                    char[] chars = new char[1];
                    for (int j = segmentsStart; j <= segmentsEnd; j++)
                    {
                        taskOutput.Add(ProcessString3(input[j], ref chars));
                    }
                    return taskOutput;
                });
                tasks[i] = newTask;
            }

            Task.WaitAll(tasks);

            foreach(Task< List<string>> task in tasks)
            {
                output.AddRange(task.Result);
            }

            return output;


            /*int count = input.Count;
            int outputIndex = 0;
            char[][] output = new char[count][];
            Parallel.ForEach<string, LoopParams2>(
                input,
                GetParallelOptions(maxParallelism),
                () =>
                {
                    return new LoopParams2(new char[1][], 1, 0);
                },
                (item, loop, local) =>
                {
                    int length = local.Length;
                    local.Chars[local.Index] = ProcessString4(item, ref length);
                    local.Index++;
                    return local;
                },
                (result) =>
                {
                    lock (output)
                    {
                        for(int i = 0; i < result.Chars.Length; i++)
                        {
                            output[outputIndex] = result.Chars[i];
                            outputIndex++;
                        }
                    }
                }
            );*/

            //return output;
        }

        private ParallelOptions GetParallelOptions(int? maxParallelism)
        {
            if(maxParallelism == null)
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
            char[] chars = new char[1];
            foreach (string item in input)
            {
                //output.Add(ProcessString(item));
                output.Add(ProcessString3(item, ref chars));
            }
            return output;
        }

        public char[][] ProcessStrings2(List<string> input)
        {
            int count = input.Count;
            int length = 1;

            List<string> output = new List<string>();
            char[][] charsOutput = new char[count][];
            for(int i = 0; i < count; i++)
            {
                charsOutput[i] = ProcessString4(input[i], ref length);
            }
            return charsOutput;
        }

        public string ProcessString(string input)
        {
            StringBuilder builder = new StringBuilder();
            for(int i = 0; i < input.Length; i++)
            {
                if(input[i] == '\n')
                {
                    builder.Append('\r');
                }
                builder.Append(input[i]);
            }
            return builder.ToString();
        }

        public string ProcessString2(string input)
        {
            char[] charArray = new char[(int)(input.Length * 1.3)];
            int location = 0;

            for (int i = 0; i < input.Length; i++)
            {
                if (charArray.Length + 2 <= i)
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

        public string ProcessString3(string input, ref char[] charArray)
        {
            if(charArray.Length < (int)(input.Length * 1.1))
            {
                Array.Resize<char>(ref charArray, (int)(input.Length * 1.1));
            }

            int location = 0;
            for (int i = 0; i < input.Length; i++)
            {
                if (charArray.Length + 2 <= i)
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
            if(location + 1 > charArray.Length)
            {
                for (int i = charArray.Length; i < location; i++)
                {
                    charArray[i] = new char();
                }
            }
            return new string(charArray, 0, location);
        }

        public char[] ProcessString4(string input, ref int length)
        {
            if (length < (int)(input.Length * 1.1))
            {
                length = (int)(input.Length * 1.1);
            }
            char[] charArray = new char[length];

            int location = 0;
            for (int i = 0; i < input.Length; i++)
            {
                if (charArray.Length + 2 <= i)
                {
                    length = (int)(length * 1.1);
                    Array.Resize<char>(ref charArray, length);
                }

                if (input[i] == '\n')
                {
                    charArray[location] = '\r';
                    location++;
                }
                charArray[location] = input[i];
                location++;
            }
            if (location + 1 > charArray.Length)
            {
                for (int i = charArray.Length; i < location; i++)
                {
                    charArray[i] = new char();
                }
            }
            return charArray;
        }
    }

    public class LoopParams
    {
        public LoopParams(List<string> strings, char[] chars)
        {
            Strings = strings;
            CharArray = chars;
        }
        public List<string> Strings { get; set; }
        public char[] CharArray { get; set; }
    }

    public class LoopParams2
    {
        public LoopParams2(char[][] chars, int length, int index)
        {
            Chars = chars;
            Length = length ;
            Index = index;
        }
        public char[][] Chars { get; set; }
        public int Length { get; set; }
        public int Index { get; set; }
    }
}
