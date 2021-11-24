using System.Reflection;
using System.Reflection.Emit;

using static System.Reflection.Emit.OpCodes;

namespace ReflectionService
{
    public static class JSInteropReflections
    {
        private readonly static MethodInfo baseMethodInfo;
        private readonly static Module module;
        public readonly static Func<Microsoft.JSInterop.Implementation.JSObjectReference, long> getId;

        static JSInteropReflections()
        {
            Assembly asm = Assembly.Load("Microsoft.JSInterop.WebAssembly.dll");
            var type = asm.GetType("WebAssembly.JSInterop.InternalCalls");
            module = type?.Module ?? throw new InvalidOperationException();
            var method = type?.GetMethod("InvokeJS", BindingFlags.Public | BindingFlags.Static);
            if (method is null) { throw new InvalidOperationException(); }
            baseMethodInfo = method;

            var idProp = typeof(Microsoft.JSInterop.Implementation.JSObjectReference).GetProperty("Id", BindingFlags.NonPublic | BindingFlags.Instance);
            if (idProp is null) { throw new InvalidOperationException(); }

            var func = (Func<Microsoft.JSInterop.Implementation.JSObjectReference, long>)Delegate.CreateDelegate(typeof(Func<Microsoft.JSInterop.Implementation.JSObjectReference, long>), idProp.GetMethod);
            getId = func;
        }

        public static unsafe class GenericTypeCache<T0, T1, T2, TRes>
        {
            public static readonly delegate*<IntPtr, IntPtr, T0, T1, T2, TRes> del;


            static GenericTypeCache()
            {
#if AOT
                var info = baseMethodInfo.MakeGenericMethod(typeof(T0), typeof(T1), typeof(T2), typeof(TRes));
                del = (delegate*<IntPtr, IntPtr, T0, T1, T2, TRes>)LoadFunctionPointer(info);
#else
                del = (delegate*<IntPtr, IntPtr, T0, T1, T2, TRes>)baseMethodInfo.MakeGenericMethod(typeof(T0), typeof(T1), typeof(T2), typeof(TRes)).MethodHandle.GetFunctionPointer();
#endif
            }

            private static IntPtr LoadFunctionPointer(MethodInfo target)
            {
                Console.WriteLine("AOT用 ldftn開始");
                DynamicMethod dynamicMethod = new("<>Ldftn", typeof(IntPtr), null, module, true);
                var ilGen = dynamicMethod.GetILGenerator();
                ilGen.Emit(Ldftn, target);
                ilGen.Emit(Ret);
                var func = dynamicMethod.CreateDelegate<Func<IntPtr>>();
                Console.WriteLine("ldftn関数作成完了");
                var ptr = func.Invoke();
                Console.WriteLine($"fptr={ptr}");
                return ptr;
            }
        }
    }
}
