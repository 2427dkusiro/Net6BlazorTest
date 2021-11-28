using Microsoft.JSInterop;

namespace TestImplement
{
    public static class SerializerTest
    {
        public static async Task RunSerializeTest(this Tester tester, int size, IJSRuntime jSRuntime)
        {
            var result = tester.CreateNewCondition($"N={size}");

            TestObject[] objects = Enumerable.Range(1, size).Select(x => TestObject.CreateDummy()).ToArray();
            var module = await jSRuntime.InvokeAsync<IJSObjectReference>("import", "./TestScript.js")
                as Microsoft.JSInterop.Implementation.JSObjectReference;
            var module2 = module as IJSInProcessObjectReference;
            var moduleId = module.GetId();
            var resolver = Utf8Json.Resolvers.StandardResolver.AllowPrivateCamelCase;

            var watch = System.Diagnostics.Stopwatch.StartNew();
            foreach (var obj in objects)
            {
                JSHelper.InvokeJSUTF8<TestObject, object>("UTF8JsonTest", obj, resolver, moduleId);
            }
            watch.Stop();
            result["utf8json"] = watch.Elapsed;

            watch = System.Diagnostics.Stopwatch.StartNew();
            foreach (var obj in objects)
            {
                module2.InvokeVoid("JsonTest", obj);
            }
            watch.Stop();
            result["InProcess"] = watch.Elapsed;
        }
    }

    public sealed class TestObject
    {
        public TestObject()
        {

        }

        private readonly static Random random = new Random();
        public static TestObject CreateDummy()
        {
            return new TestObject()
            {
                Id = Guid.NewGuid(),
                Name = "Hoge",
                IsActive = (random.Next() & 1) == 1,
                Address = random.NextInt64(),
            };
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public long Address { get; set; }
    }
}
