using TestImplement;

class Program
{
    static async Task Main(string[] args)
    {
        Tester tester = new Tester();
        await tester.RunPrimeTest(6);
        Console.WriteLine(tester.ExportAsMarkDown());
    }
}