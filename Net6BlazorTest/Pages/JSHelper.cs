using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using System.Reflection;

namespace Net6BlazorTest.Pages
{
    public class JSHelper
    {
        public static string InvokeJS(string P_0, string P_1, JSCallResultType P_2, long P_3)
        {
            JSCallInfo jSCallInfo = default(JSCallInfo);
            jSCallInfo.FunctionIdentifier = P_0;
            jSCallInfo.TargetInstanceId = P_3;
            jSCallInfo.ResultType = P_2;
            jSCallInfo.MarshalledCallArgsJson = P_1 ?? "[]";
            jSCallInfo.MarshalledCallAsyncHandle = 0L;
            JSCallInfo jSCallInfo2 = jSCallInfo;
            string text;
            string result = InvokeJS<object, object, object, string>(out text, ref jSCallInfo2, null, null, null);
            if (text == null)
            {
                return result;
            }
            throw new Exception(text);
        }

        public static TResult InvokeUnmarshalled<T0, T1, T2, TResult>(string P_0, T0 P_1, T1 P_2, T2 P_3, long P_4)
        {
            JSCallResultType jSCallResultType = JSCallResultType.Default;
            JSCallInfo jSCallInfo = default(JSCallInfo);
            jSCallInfo.FunctionIdentifier = P_0;
            jSCallInfo.TargetInstanceId = P_4;
            jSCallInfo.ResultType = jSCallResultType;
            JSCallInfo jSCallInfo2 = jSCallInfo;
            string text;
            switch (jSCallResultType)
            {
                case JSCallResultType.Default:
                case JSCallResultType.JSVoidResult:
                    {
                        TResult result = InvokeJS<T0, T1, T2, TResult>(out text, ref jSCallInfo2, P_1, P_2, P_3);
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

        public static unsafe TRes InvokeJS<T0, T1, T2, TRes>(out string P_0, ref JSCallInfo P_1, T0 P_2, T1 P_3, T2 P_4)
        {
            Assembly asm = typeof(Microsoft.JSInterop.WebAssembly.WebAssemblyJSRuntime).Assembly;
            var method = asm.GetType("WebAssembly.JSInterop.InternalCalls")?.GetMethod("InvokeJS", BindingFlags.Public | BindingFlags.Static)
                ?.MakeGenericMethod(new[] { typeof(T0), typeof(T1), typeof(T2), typeof(TRes) });
            if (method is null) { throw new InvalidOperationException(); }

            var type = asm.GetType("WebAssembly.JSInterop.JSCallInfo") ?? throw new InvalidOperationException();
            var asMethod = typeof(Unsafe).GetMethods().First(x => x.Name == "As" && x.GetGenericArguments().Length == 2)
                .MakeGenericMethod(new[] { typeof(JSCallInfo), type });
            var ins = asMethod.Invoke(null, new object[] { P_1 });

            P_0 = default;
            var res = method.Invoke(null, new object?[] { P_0, ins, P_2, P_3, P_4 });
            return res is null ? default(TRes) : (TRes)res;
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
