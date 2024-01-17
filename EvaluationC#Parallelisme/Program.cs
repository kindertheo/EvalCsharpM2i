using System.Diagnostics;

// Faire un calcul de la somme de tous les nombres entre 1 et 3000 en opération parallèle
static int ParallelSum(int start, int end)
{
    int sum = 0;
    object lockObject = new object();

    Parallel.For(start, end + 1, () => 0, (i, loop, partialSum) =>
    {
        partialSum += i;
        return partialSum;
    }, partialSum =>
    {
        lock (lockObject)
        {
            sum += partialSum;
        }
    });
    return sum;
}


// Traiter deux fichiers pour compter le nombre de mots (séparateur " ")
static int ParallelCountWords(string content)
{
    return content.Split(new char[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
}
static int ParallelCountLorem(string content)
{
    int count = content.Split(new char[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries)
               .Count(w => w == "Lorem");
    return count;
}


static async Task<int> ParallelCountsAsync(string filePath)
{
    string BaseDir = AppDomain.CurrentDomain.BaseDirectory;
    string content = await File.ReadAllTextAsync(BaseDir + filePath);
    int countWords = ParallelCountWords(content);
    Console.WriteLine("Number of words : " + countWords.ToString() + " for " + filePath);
    int countLorem = ParallelCountLorem(content);
    Console.WriteLine("Number of lorems : " + countLorem.ToString() + " for " + filePath);
    return countWords + countLorem;
}

static async Task Main()
{
    var timer = new Stopwatch();
    timer.Start();

    // Afficher le résultat du calcul de la somme des nombres entre 1 et 3000
    int parallelSum = ParallelSum(0, 3000);
    Console.WriteLine("Sum between all numbers from 1 to 3000 : " + parallelSum);

    string pathFile1 = "files/Eval_file1.txt";
    string pathFile2 = "files/Eval_file2.txt";

    Task<int> totalCountFile1 = Task.Run( () => ParallelCountsAsync(pathFile1));
    Task<int> totalCountFile2 = Task.Run( () => ParallelCountsAsync(pathFile2));

    int[] results = await Task.WhenAll(totalCountFile1, totalCountFile2);

    int sumOfAll = parallelSum + results[0] + results[1];

    Console.WriteLine("Total sum : " +  sumOfAll.ToString());

    timer.Stop();
    TimeSpan ts = timer.Elapsed;
    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
    ts.Hours, ts.Minutes, ts.Seconds,
    ts.Milliseconds / 10);
    Console.WriteLine("RunTime " + elapsedTime);

}

await Main();