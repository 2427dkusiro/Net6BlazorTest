using Microsoft.JSInterop;

namespace TestImplement
{
    public static class ArrayTest
    {
        public static async Task RunArrayTest(this Tester tester, int size, IJSRuntime jSRuntime)
        {
            var result = tester.CreateNewCondition($"{size >> 10}KB");

            var module = await jSRuntime.InvokeAsync<IJSObjectReference>("import", "./TestScript.js");
            var testData = Enumerable.Range(0, size).Select(x => (byte)x).ToArray();
            TimeSpan[] resArray = new TimeSpan[10];

            for (int i = 0; i < 10; i++)
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                await module.InvokeVoidAsync("ByteArrayTest", testData);
                stopwatch.Stop();
                resArray[i] = stopwatch.Elapsed;
            }
            result["average"] = TimeSpan.FromMilliseconds(resArray.Select(x => x.TotalMilliseconds).Average());
            result["max"] = resArray.Max();
        }
    }
}
