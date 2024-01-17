using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluationC_Parallelisme
{

    class Program
    {
        static void Main(string[] args)
        {
            int wordCount1 = ParallelCountWords("files / Eval_file1.txt");
            Console.WriteLine($"Nombre de mots pour le fichier 1 : {wordCount1}");
            int wordCount2 = ParallelCountWords("files / Eval_file2.txt");
            Console.WriteLine($"Nombre de mots pour le fichier 2 : {wordCount2}");
        }

        static int ParallelCountWords(string filePath)
        {
            string content = File.ReadAllText(filePath);
            string[] words = content.Split(' ');

            int processorCount = Environment.ProcessorCount;
            int segmentSize = words.Length / processorCount;
            List<Task<int>> tasks = new List<Task<int>>();

            for (int i = 0; i < processorCount; i++)
            {
                int start = i * segmentSize;
                int end = (i == processorCount - 1) ? words.Length : start + segmentSize;

                tasks.Add(Task.Run(() => CountWordsSegment(words, start, end)));
            }

            Task.WaitAll(tasks.ToArray());

            int total = 0;
            foreach (var task in tasks)
            {
                total += task.Result;
            }

            return total;
        }

        static int CountWordsSegment(string[] words, int start, int end)
        {
            int count = 0;
            for (int i = start; i < end; i++)
            {
                if (!string.IsNullOrEmpty(words[i]))
                {
                    count++;
                }
            }

            return count;
        }
    }
}
