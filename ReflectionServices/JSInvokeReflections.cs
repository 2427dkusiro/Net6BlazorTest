using System.Reflection;

namespace ReflectionServices
{
    public static class JSInvokeReflections
    {
        private readonly static MethodInfo baseMethodInfo;
        private readonly static Func<Microsoft.JSInterop.Implementation.JSObjectReference, long> getId;
        static JSInvokeReflections()
        {
            Assembly asm = typeof(Microsoft.JSInterop.WebAssembly.WebAssemblyJSRuntime).Assembly;
            var method = asm.GetType("WebAssembly.JSInterop.InternalCalls")?.GetMethod("InvokeJS", BindingFlags.Public | BindingFlags.Static);
            if (method is null) { throw new InvalidOperationException(); }
            baseMethodInfo = method;

            var idProp = typeof(Microsoft.JSInterop.Implementation.JSObjectReference).GetProperty("Id", BindingFlags.NonPublic | BindingFlags.Instance);
            if (idProp is null) { throw new InvalidOperationException(); }

            var func = (Func<Microsoft.JSInterop.Implementation.JSObjectReference, long>)Delegate.CreateDelegate(typeof(Func<Microsoft.JSInterop.Implementation.JSObjectReference, long>), idProp.GetMethod);
            getId = func;
        }

        public static int GetIdDelegate()
        {

        }
    }
}