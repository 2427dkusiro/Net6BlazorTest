using TestImplement;

namespace Net6BlazorTest.Pages
{
    public partial class Index
    {
        private List<TestUI> TestUIs = new();

        private static string Display(TimeSpan ts)
        {
            return $"{ts.TotalMilliseconds.ToString("F1")}ms";
        }

        protected override void OnInitialized()
        {
            TestUIs.Add(new TestUI("処理速度/素数列挙(ふるい)", async tester =>
            {
                await tester.RunPrimeTest(5);
                await tester.RunPrimeTest(6);
            }));

            TestUIs.Add(new TestUI("JS呼び出し時間/単純呼び出し", async tester =>
            {
                await tester.RunJSCallTest(1, JSRuntime);
                await tester.RunJSCallTest(1 << 4, JSRuntime);
                await tester.RunJSCallTest(1 << 6, JSRuntime);
                await tester.RunJSCallTest(1 << 8, JSRuntime);
                await tester.RunJSCallTest(1 << 10, JSRuntime);
                await tester.RunJSCallTest(1 << 12, JSRuntime);
            }));

            TestUIs.Add(new TestUI("JS呼び出し時間/バイト配列転送", async tester =>
            {
                await tester.RunArrayTest(1 << 18, JSRuntime);
                await tester.RunArrayTest(1 << 20, JSRuntime);
                await tester.RunArrayTest(1 << 22, JSRuntime);
            }));

            TestUIs.Add(new TestUI("JS呼び出し時間/jsonシリアライズ-type1", async tester =>
            {
                await tester.RunSerializeTestType1(1, JSRuntime);
                await tester.RunSerializeTestType1(1 << 4, JSRuntime);
                await tester.RunSerializeTestType1(1 << 6, JSRuntime);
                await tester.RunSerializeTestType1(1 << 8, JSRuntime);
                await tester.RunSerializeTestType1(1 << 10, JSRuntime);
                await tester.RunSerializeTestType1(1 << 12, JSRuntime);
            }));

            TestUIs.Add(new TestUI("JS呼び出し時間/jsonシリアライズ-type2", async tester =>
            {
                await tester.RunSerializeTestType2(1, JSRuntime);
                await tester.RunSerializeTestType2(1 << 4, JSRuntime);
                await tester.RunSerializeTestType2(1 << 6, JSRuntime);
                await tester.RunSerializeTestType2(1 << 8, JSRuntime);
                await tester.RunSerializeTestType2(1 << 10, JSRuntime);
                await tester.RunSerializeTestType2(1 << 12, JSRuntime);
            }));

            TestUIs.Add(new TestUI("JS呼び出し時間/Worker想定シリアライズ", async tester =>
            {
                await tester.RunWorkerCallTest(1, JSRuntime);
                await tester.RunWorkerCallTest(1 << 4, JSRuntime);
                await tester.RunWorkerCallTest(1 << 6, JSRuntime);
                await tester.RunWorkerCallTest(1 << 8, JSRuntime);
                await tester.RunWorkerCallTest(1 << 10, JSRuntime);
                await tester.RunWorkerCallTest(1 << 12, JSRuntime);
            }));
        }
    }
}
