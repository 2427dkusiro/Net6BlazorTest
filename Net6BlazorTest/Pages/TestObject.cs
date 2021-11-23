namespace Net6BlazorTest.Pages
{
    public class TestObject
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
