class Program
{

    static void Main(string[] args)
    {
        TestClass test = new TestClass();
        test.Execute();
    }
}
class TestClass
{

    private string Result { get; set; } = "";

    public void Execute()
    {
        OnClick();
        Console.WriteLine(Result);
    }

    private void OnClick()
    {
        Result = "";
        List<TimeSpan> list = new();
        int n = (int)Math.Pow(10, 6);

        for (int i = 0; i < 10; i++)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var primes = FindPrimes(n).ToArray();
            var count = primes.Length;
            stopwatch.Stop();
            list.Add(stopwatch.Elapsed);
        }
        Result = $"n={n},ave={list.Select(x => x.TotalMilliseconds).Average()}ms,max={list.Max()}";
    }

    // O(NlogN)程度の計算量でN以下の素数を列挙する
    // パフォーマンス測定用
    private IEnumerable<int> FindPrimes(int n)
    {
        if (n < 0)
        {
            return Array.Empty<int>();
        }

        var isPrime = new bool[n + 1];
        Array.Fill(isPrime, true);
        isPrime[0] = false;
        isPrime[1] = false;

        for (int i = 2; i < isPrime.Length; i++)
        {
            if (!isPrime[i])
            {
                continue;
            }
            for (int j = i << 1; j < isPrime.Length; j += i)
            {
                isPrime[j] = false;
            }
        }

        return isPrime.Select((x, i) => (x, i)).Where(tuple => tuple.x).Select(tuple => tuple.i);
    }
}