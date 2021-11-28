using Microsoft.JSInterop;

namespace TestImplement
{
    public static class JSCallTest
    {
        public static async Task RunJSCallTest(this Tester testResult, int size, IJSRuntime JSRuntime)
        {
            var result = testResult.CreateNewCondition($"N={size}");

            var runtime = (Microsoft.JSInterop.WebAssembly.WebAssemblyJSRuntime)JSRuntime;
            var module = await runtime.InvokeAsync<IJSUnmarshalledObjectReference>("import", "./TestScript.js");
            var moduleId = (module as Microsoft.JSInterop.Implementation.JSObjectReference ?? throw new InvalidOperationException()).GetId();

            var global = ReflectionService.RuntimeReflections.GetGlobalObject("globalThis", out var error);
            var globalId = ReflectionService.RuntimeReflections.GetJsHandle(global);

            var module2Id = (int)await (Task<object>)ReflectionService.RuntimeReflections.InvokeJSWithArgs(globalId, "_import", new[] { "./TestScript.js" }, out var error2);

            System.Diagnostics.Stopwatch watch;

            watch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < size; i++)
            {
                await module.InvokeVoidAsync("Hoge", i);
            }
            watch.Stop();
            result["async"] = watch.Elapsed;

            watch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < size; i++)
            {
                module.InvokeVoid("Hoge", i);
            }
            watch.Stop();
            result["InProcess"] = watch.Elapsed;

            watch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < size; i++)
            {
                module.InvokeUnmarshalled<int, bool>("Hoge", i);
            }
            watch.Stop();
            result["Unmarshalled"] = watch.Elapsed;

            watch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < size; i++)
            {
                JSHelper.InvokeUnmarshalled<int, object?, object?, object>("Hoge", i, null, null, moduleId);
            }
            watch.Stop();
            result["original"] = watch.Elapsed;

            watch = System.Diagnostics.Stopwatch.StartNew();
            var argObj1 = new object[1];
            for (int i = 0; i < size; i++)
            {
                argObj1[0] = i;
                ReflectionService.RuntimeReflections.InvokeJSWithArgs(globalId, "Hoge", argObj1, out _);
            }
            watch.Stop();
            result["worker/global"] = watch.Elapsed;

            watch = System.Diagnostics.Stopwatch.StartNew();
            var argObj = new object[] { module2Id, "Hoge", 0 };
            for (int i = 0; i < size; i++)
            {
                argObj[2] = i;
                ReflectionService.RuntimeReflections.InvokeJSWithArgs(globalId, "_invoke", argObj, out _);
            }
            watch.Stop();
            result["worker/module"] = watch.Elapsed;
        }
    }
}
