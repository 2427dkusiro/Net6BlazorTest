using Microsoft.JSInterop;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TestImplement
{
    public static class WorkerCallTest
    {
        public static async Task RunWorkerCallTest(this Tester tester, int size, IJSRuntime jSRuntime)
        {
            var result = tester.CreateNewCondition($"N={size}");
            const string binaryTestMethod = "BinaryTest";

            TestObject[] objects = Enumerable.Range(1, size).Select(x => TestObject.CreateDummy()).ToArray();
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
                var ret = JSHelper.InvokeJSUTF8<TestObject, IntPtr>(binaryTestMethod, obj, resolver, moduleId);
                var retBin = Unsafe.As<IntPtr, byte[]>(ref ret);

                var _obj = Utf8Json.JsonSerializer.Deserialize<TestObject>(retBin);
                System.Diagnostics.Debug.Assert(obj.Equals(_obj));
            }
            watch.Stop();
            result["utf8json"] = watch.Elapsed;

            watch = System.Diagnostics.Stopwatch.StartNew();
            foreach (var obj in objects)
            {
                var bin = MessagePack.MessagePackSerializer.Serialize(obj, resolver2);
                var ret = JSHelper.InvokeUnmarshalled<byte[], int, object?, IntPtr>(binaryTestMethod, bin, bin.Length, null, moduleId);
                var retBin = Unsafe.As<IntPtr, byte[]>(ref ret);

                var _obj = MessagePack.MessagePackSerializer.Deserialize<TestObject>(retBin, resolver2);
                System.Diagnostics.Debug.Assert(obj.Equals(_obj));
            }
            watch.Stop();
            result["MessagePack"] = watch.Elapsed;

            watch = System.Diagnostics.Stopwatch.StartNew();
            foreach (var obj in objects)
            {
                var bin = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(obj);
                var ret = JSHelper.InvokeUnmarshalled<byte[], int, object?, IntPtr>(binaryTestMethod, bin, bin.Length, null, moduleId);
                var _obj = System.Text.Json.JsonSerializer.Deserialize<TestObject>(Unsafe.As<IntPtr, byte[]>(ref ret));
                System.Diagnostics.Debug.Assert(obj.Equals(_obj));
            }
            watch.Stop();
            result["System.Text.Json/utf8"] = watch.Elapsed;

        }
    }
}
