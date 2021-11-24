using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Utf8Json;

namespace Net6BlazorTest.Pages
{
    public static class JSHelper
    {
        private readonly static MethodInfo baseMethodInfo;
        private readonly static Func<Microsoft.JSInterop.Implementation.JSObjectReference, long> getId;

        static JSHelper()
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

        public static long GetId(this Microsoft.JSInterop.Implementation.JSObjectReference jSObjectReference)
        {
            return getId.Invoke(jSObjectReference);
        }

        public unsafe static TResult InvokeJSUTF8<TArg, TResult>(string method, TArg arg0, IJsonFormatterResolver resolver, long objId = 0l)
        {
            byte[] json = JsonSerializer.Serialize(arg0, resolver);
            return InvokeUnmarshalled<byte[], int, object?, TResult>(method, json, json.Length, null, objId);
        }

        public static string InvokeJS(string P_0, string P_1, JSCallResultType P_2, long P_3)
        {
            JSCallInfo jSCallInfo = default;
            jSCallInfo.FunctionIdentifier = P_0;
            jSCallInfo.TargetInstanceId = P_3;
            jSCallInfo.ResultType = P_2;
            jSCallInfo.MarshalledCallArgsJson = P_1 ?? "[]";
            jSCallInfo.MarshalledCallAsyncHandle = 0L;
            JSCallInfo jSCallInfo2 = jSCallInfo;
            string? text;
            string result = InvokeJS<object?, object?, object?, string>(out text, ref jSCallInfo2, null, null, null);
            if (text == null)
            {
                return result;
            }
            throw new Exception(text);
        }

        public static TResult InvokeUnmarshalled<T0, T1, T2, TResult>(string P_0, T0 P_1, T1 P_2, T2 P_3, long P_4)
        {
            JSCallResultType jSCallResultType = JSCallResultType.Default;
            JSCallInfo jSCallInfo = default;
            jSCallInfo.FunctionIdentifier = P_0;
            jSCallInfo.TargetInstanceId = P_4;
            jSCallInfo.ResultType = jSCallResultType;
            JSCallInfo jSCallInfo2 = jSCallInfo;
            switch (jSCallResultType)
            {
                case JSCallResultType.Default:
                case JSCallResultType.JSVoidResult:
                    {
                        TResult result = InvokeJS<T0, T1, T2, TResult>(out string? text, ref jSCallInfo2, P_1, P_2, P_3);
                        if (text == null)
                        {
                            return result;
                        }
                        throw new Exception(text);
                    }
                case JSCallResultType.JSObjectReference:
                    {
                        throw new NotSupportedException();
                        /*
                        int num = InvokeJS<T0, T1, T2, int>(out text, ref jSCallInfo2, P_1, P_2, P_3);
                        if (text == null)
                        {
                            return (TResult)(object)new WebAssemblyJSObjectReference(this, num);
                        }
                        throw new Exception(text);
                        */
                    }
                case JSCallResultType.JSStreamReference:
                    {
                        throw new NotSupportedException();
                        /*
                        string text2 = InvokeJS<T0, T1, T2, string>(out text, ref jSCallInfo2, P_1, P_2, P_3);
                        if (text == null)
                        {
                            return (TResult)DeserializeJSStreamReference(text2);
                        }
                        throw new Exception(text);
                        */
                    }
                default:
                    {
                        DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(23, 1);
                        defaultInterpolatedStringHandler.AppendLiteral("Invalid result type '");
                        defaultInterpolatedStringHandler.AppendFormatted(jSCallResultType);
                        defaultInterpolatedStringHandler.AppendLiteral("'.");
                        throw new InvalidOperationException(defaultInterpolatedStringHandler.ToStringAndClear());
                    }
            }
        }

        public static unsafe TRes InvokeJS<T0, T1, T2, TRes>(out string? P_0, ref JSCallInfo P_1, T0 P_2, T1 P_3, T2 P_4)
        {
            var del = GenericTypeCache<T0, T1, T2, TRes>.del;
            P_0 = null;
            void* arg0Ptr = Unsafe.AsPointer(ref P_0);
            void* arg1Ptr = Unsafe.AsPointer(ref P_1);
            TRes result = del((nint)arg0Ptr, (nint)arg1Ptr, P_2, P_3, P_4);
            return result;
        }

        public static unsafe class GenericTypeCache<T0, T1, T2, TRes>
        {
#if true
            public static readonly delegate*<IntPtr, IntPtr, T0, T1, T2, TRes> del;
#else
            public static readonly Func<IntPtr, IntPtr, T0, T1, T2, TRes> del;
#endif
            static GenericTypeCache()
            {
#if true
                Console.WriteLine("関数ポインタ作成開始");
                var generic = baseMethodInfo.MakeGenericMethod(typeof(T0), typeof(T1), typeof(T2), typeof(TRes));
                Console.WriteLine("ジェネリックメソッド作成");
                var ptr = generic.MethodHandle.GetFunctionPointer();
                Console.WriteLine($"関数アドレス:{ptr}");
                del = (delegate*<IntPtr, IntPtr, T0, T1, T2, TRes>)ptr;
                Console.WriteLine("関数ポインタ作成完了");
                    
#else
                del = (Func<IntPtr, IntPtr, T0, T1, T2, TRes>)Delegate.CreateDelegate(typeof(Func<IntPtr, IntPtr, T0, T1, T2, TRes>), baseMethodInfo.MakeGenericMethod(typeof(T0), typeof(T1), typeof(T2), typeof(TRes)));
#endif
            }
        }

        [StructLayout(LayoutKind.Explicit, Pack = 4)]
        public struct JSCallInfo
        {
            [FieldOffset(0)]
            public string FunctionIdentifier;

            [FieldOffset(4)]
            public JSCallResultType ResultType;

            [FieldOffset(8)]
            public string MarshalledCallArgsJson;

            [FieldOffset(12)]
            public long MarshalledCallAsyncHandle;

            [FieldOffset(20)]
            public long TargetInstanceId;
        }

        public enum JSCallResultType
        {
            /// <summary>
            /// Indicates that the returned value is not treated in a special way.
            /// </summary>
            Default,
            /// <summary>
            /// Indicates that the returned value is to be treated as a JS object reference.
            /// </summary>
            JSObjectReference,
            /// <summary>
            /// Indicates that the returned value is to be treated as a JS data reference.
            /// </summary>
            JSStreamReference,
            /// <summary>
            /// Indicates a void result type.
            /// </summary>
            JSVoidResult
        }
    }
}
