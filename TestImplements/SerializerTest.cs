using Microsoft.JSInterop;

namespace TestImplement
{
    public static class SerializerTest
    {
        public static async Task RunSerializeTestType1(this Tester tester, int size, IJSRuntime jSRuntime)
        {
            TestObject[] objects = Enumerable.Range(1, size).Select(x => TestObject.CreateDummy()).ToArray();
            await RunSerializeTest(tester, size, jSRuntime, objects);
        }

        public static async Task RunSerializeTestType2(this Tester tester, int size, IJSRuntime jSRuntime)
        {
            TestObject2[] objects = Enumerable.Range(1, size).Select(x => TestObject2.CreateDummy()).ToArray();
            await RunSerializeTest(tester, size, jSRuntime, objects);
        }

        internal static async Task RunSerializeTest<T>(Tester tester, int size, IJSRuntime jSRuntime, T[] objects)
        {
            var result = tester.CreateNewCondition($"N={size}");
            const string utf8MethodName = "UTF8JsonTest";

            var module = await jSRuntime.InvokeAsync<IJSObjectReference>("import", "./TestScript.js")
                as Microsoft.JSInterop.Implementation.JSObjectReference;
            var module2 = module as IJSInProcessObjectReference;
            var moduleId = module.GetId();
            var resolver = Utf8Json.Resolvers.StandardResolver.Default;
            var resolver2 = MessagePack.Resolvers.ContractlessStandardResolver.Options;

            System.Diagnostics.Stopwatch watch;

            watch = System.Diagnostics.Stopwatch.StartNew();
            foreach (var obj in objects)
            {
                _ = JSHelper.InvokeJSUTF8Unsafe<T, object>(utf8MethodName, obj, resolver, moduleId);
            }
            watch.Stop();
            result["utf8json/unsafe"] = watch.Elapsed;

            watch = System.Diagnostics.Stopwatch.StartNew();
            foreach (var obj in objects)
            {
                _ = JSHelper.InvokeJSUTF8<T, object>(utf8MethodName, obj, resolver, moduleId);
            }
            watch.Stop();
            result["utf8json"] = watch.Elapsed;

            watch = System.Diagnostics.Stopwatch.StartNew();
            foreach (var obj in objects)
            {
                var bin = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(obj);
                _ = JSHelper.InvokeUnmarshalled<byte[], int, object?, object?>(utf8MethodName, bin, bin.Length, null, moduleId);
            }
            watch.Stop();
            result["System.Text.Json/utf8"] = watch.Elapsed;

            watch = System.Diagnostics.Stopwatch.StartNew();
            foreach (var obj in objects)
            {
                module2.InvokeVoid("JsonTest", obj);
            }
            watch.Stop();
            result["InProcess"] = watch.Elapsed;
        }
    }
}
