using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

using static System.Reflection.Emit.OpCodes;

namespace ReflectionService
{
    public static class RuntimeReflections
    {
        private static readonly Module module;

        static RuntimeReflections()
        {
            var asm = Assembly.Load("System.Private.Runtime.InteropServices.JavaScript, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
            var type = (asm.GetType("Interop") ?? throw new InvalidOperationException("Interopクラス発見失敗")).GetNestedType("Runtime", BindingFlags.NonPublic | BindingFlags.Static) ?? throw new InvalidOperationException("Runtimeクラス発見失敗");
            module = type?.Module ?? throw new InvalidOperationException();

            var invokeJS = type.GetMethod("InvokeJS", BindingFlags.NonPublic | BindingFlags.Static);
            _invokeJS = BuildInvokeJS(invokeJS ?? throw new InvalidOperationException());

            var invokeJSWithArgs = type.GetMethod("InvokeJSWithArgs", BindingFlags.NonPublic | BindingFlags.Static);
            _invokeJSWithArgs = BuildInvokeJSWithArgs(invokeJSWithArgs ?? throw new InvalidOperationException());

            var getGlobalObject = type.GetMethod("GetGlobalObject", BindingFlags.NonPublic | BindingFlags.Static);
            _getGlobalObject = BuildGetGlobalObject(getGlobalObject ?? throw new InvalidOperationException());

            var type2 = asm.GetType("System.Runtime.InteropServices.JavaScript.JSObject") ?? throw new InvalidCastException("JSObject発見失敗");
            var getJSHandle = type2.GetProperty("JSHandle")?.GetGetMethod() ?? throw new InvalidOperationException();
            _getJsHandle = BuildGetJSHandle(getJSHandle ?? throw new InvalidOperationException());
        }

        private static readonly Func<string, IntPtr, string> _invokeJS;

        public static unsafe string InvokeJS(string str, out int exceptionalResult)
        {
            exceptionalResult = 0;
            return _invokeJS(str, (IntPtr)Unsafe.AsPointer(ref exceptionalResult));
        }

        private static Func<string, IntPtr, string> BuildInvokeJS(MethodInfo methodInfo)
        {
            DynamicMethod dynamicMethod = new("<>InvokeJS", typeof(string), new[] { typeof(string), typeof(IntPtr) }, module, true);
            var ilGen = dynamicMethod.GetILGenerator();
            ilGen.Emit(Ldarg_0);
            ilGen.Emit(Ldarg_1);
            ilGen.Emit(Call, methodInfo);
            ilGen.Emit(Ret);
            var del = dynamicMethod.CreateDelegate<Func<string, IntPtr, string>>();
            return del;
        }

        private static readonly Func<int, string, object[], IntPtr, object> _invokeJSWithArgs;

        public static unsafe object InvokeJSWithArgs(int jsObjHandle, string method, object[] parms, out int exceptionalResult)
        {
            exceptionalResult = 0;
            return _invokeJSWithArgs(jsObjHandle, method, parms, (IntPtr)Unsafe.AsPointer(ref exceptionalResult));
        }

        private static Func<int, string, object[], IntPtr, object> BuildInvokeJSWithArgs(MethodInfo methodInfo)
        {
            DynamicMethod dynamicMethod = new("<>InvokeJSWithArgs", typeof(object), new[] { typeof(int), typeof(string), typeof(object[]), typeof(IntPtr) }, module, true);
            var ilGen = dynamicMethod.GetILGenerator();
            ilGen.Emit(Ldarg_0);
            ilGen.Emit(Ldarg_1);
            ilGen.Emit(Ldarg_2);
            ilGen.Emit(Ldarg_3);
            ilGen.Emit(Call, methodInfo);
            ilGen.Emit(Ret);
            var del = dynamicMethod.CreateDelegate<Func<int, string, object[], IntPtr, object>>();
            return del;
        }

        private static readonly Func<string, IntPtr, object> _getGlobalObject;

        public static unsafe object GetGlobalObject(string globalName, out int exceptionalResult)
        {
            exceptionalResult = 0;
            return _getGlobalObject(globalName, (IntPtr)Unsafe.AsPointer(ref exceptionalResult));
        }

        private static Func<string, IntPtr, object> BuildGetGlobalObject(MethodInfo methodInfo)
        {
            DynamicMethod dynamicMethod = new("<>GetGlobalObject", typeof(object), new[] { typeof(string), typeof(IntPtr) }, module, true);
            var ilGen = dynamicMethod.GetILGenerator();
            ilGen.Emit(Ldarg_0);
            ilGen.Emit(Ldarg_1);
            ilGen.Emit(Call, methodInfo);
            ilGen.Emit(Ret);
            var del = dynamicMethod.CreateDelegate<Func<string, IntPtr, object>>();
            return del;
        }

        private static readonly Func<object, int> _getJsHandle;

        public static int GetJsHandle(object jSObject)
        {
            return _getJsHandle(jSObject);
        }

        private static Func<object, int> BuildGetJSHandle(MethodInfo methodInfo)
        {
            DynamicMethod dynamicMethod = new("<>GetJSHandle", typeof(int), new[] { typeof(object) }, module, true);
            var ilGen = dynamicMethod.GetILGenerator();
            ilGen.Emit(Ldarg_0);
            ilGen.Emit(Call, methodInfo);
            ilGen.Emit(Ret);
            var del = dynamicMethod.CreateDelegate<Func<object, int>>();
            return del;
        }
    }
}
