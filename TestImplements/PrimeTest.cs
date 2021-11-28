namespace TestImplement
{
    public static class PrimeTest
    {
        public static async Task RunPrimeTest(this Tester tester, int powN)
        {
            var result = tester.CreateNewCondition($"N=10^{powN}");

            const int testSampleCount = 10;
            int n = (int)Math.Pow(10, powN);
            TimeSpan[] array = new TimeSpan[testSampleCount];

            for (int i = 0; i < testSampleCount; i++)
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                var primes = FindPrimes(n).ToArray();
                var count = primes.Length;
                stopwatch.Stop();
                array[i] = stopwatch.Elapsed;
            }

            result["min"] = array.Min();
            result["average"] = TimeSpan.FromMilliseconds(array.Select(x => x.TotalMilliseconds).Average());
            result["max"] = array.Max();
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