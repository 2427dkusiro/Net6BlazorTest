using System.Reflection;
using System.Reflection.Emit;

using static System.Reflection.Emit.OpCodes;

namespace ReflectionService
{
    /// <summary>
    /// JS相互運用のためにリフレクションによる動的型情報を提供します。
    /// </summary>
    public static class JSInteropReflections
    {
        private readonly static MethodInfo baseMethodInfo;
        private readonly static Module module;

        /// <summary>
        /// <see cref="Microsoft.JSInterop.Implementation.JSObjectReference" /> のIDを取得する関数を取得します。
        /// </summary>
        public static Func<Microsoft.JSInterop.Implementation.JSObjectReference, long> GetId { get; }

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
            GetId = func;
        }

        /// <summary>
        /// ジェネリック型引数を利用して生成結果をキャッシュします。
        /// </summary>
        /// <typeparam name="T0"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="TRes"></typeparam>
        public static unsafe class GenericTypeCache<T0, T1, T2, TRes>
        {
#if AOT
            /// <summary>
            /// JSを実行する関数。
            /// </summary>
            public static readonly Func<IntPtr, IntPtr, T0, T1, T2, TRes> del;
#else
            /// <summary>
            /// JSを実行する関数。
            /// </summary>
            public static readonly delegate*<IntPtr, IntPtr, T0, T1, T2, TRes> del;
#endif

            static GenericTypeCache()
            {
#if AOT
                var info = baseMethodInfo.MakeGenericMethod(typeof(T0), typeof(T1), typeof(T2), typeof(TRes));
                del = MakeFunctionByILEmit(info);
#else
                del = (delegate*<IntPtr, IntPtr, T0, T1, T2, TRes>)baseMethodInfo.MakeGenericMethod(typeof(T0), typeof(T1), typeof(T2), typeof(TRes)).MethodHandle.GetFunctionPointer();
#endif
            }

            private static Func<IntPtr, IntPtr, T0, T1, T2, TRes> MakeFunctionByILEmit(MethodInfo target)
            {
                DynamicMethod dynamicMethod = new("<>InvokeJS", typeof(TRes), new[] { typeof(IntPtr), typeof(IntPtr), typeof(T0), typeof(T1), typeof(T2) }, module, true);
                var ilGen = dynamicMethod.GetILGenerator();
                ilGen.Emit(Ldarg_0);
                ilGen.Emit(Ldarg_1);
                ilGen.Emit(Ldarg_2);
                ilGen.Emit(Ldarg_3);
                ilGen.Emit(Ldarg_S, 4);
                ilGen.Emit(Call, target);
                ilGen.Emit(Ret);
                var func = dynamicMethod.CreateDelegate<Func<IntPtr, IntPtr, T0, T1, T2, TRes>>();
                return func;
            }
        }
    }
}
