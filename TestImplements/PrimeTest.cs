namespace TestImplement
{
    public static class PrimeTest
    {
        public static string RunTest(int n)
        {
            const int testSampleCount = 10;
            TimeSpan[] array = new TimeSpan[testSampleCount];

            for (int i = 0; i < testSampleCount; i++)
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                var primes = FindPrimes(n).ToArray();
                var count = primes.Length;
                stopwatch.Stop();
                array[i] = stopwatch.Elapsed;
            }
            return $"n={n},ave={array.Select(x => x.TotalMilliseconds).Average()}ms,max={array.Max().TotalMilliseconds}ms";
        }

        // O(NlogN)程度の計算量でN以下の素数を列挙する
        // パフォーマンス測定用
        private static IEnumerable<int> FindPrimes(int n)
        {
            if (n < 0)
            {
                return Array.Empty<int>();
            }

            var isNotPrime = new bool[n + 1];
            isNotPrime[0] = true;
            isNotPrime[1] = true;

            for (int i = 2; i < isNotPrime.Length; i++)
            {
                if (isNotPrime[i])
                {
                    continue;
                }
                for (int j = i << 1; j < isNotPrime.Length; j += i)
                {
                    isNotPrime[j] = true;
                }
            }

            return isNotPrime.Select((x, i) => (x, i)).Where(tuple => !tuple.x).Select(tuple => tuple.i);
        }
    }
}