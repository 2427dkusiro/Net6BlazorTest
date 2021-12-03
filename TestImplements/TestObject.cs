namespace TestImplement
{
    public class TestObject
    {
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

        public override bool Equals(object? obj)
        {
            return obj is TestObject @object &&
                   Id.Equals(@object.Id) &&
                   Name == @object.Name &&
                   IsActive == @object.IsActive &&
                   Address == @object.Address;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public long Address { get; set; }
    }

    public class TestObject2
    {
        private readonly static Random random = new Random();

        public static TestObject2 CreateDummy()
        {
            return new TestObject2()
            {
                Id = Guid.NewGuid(),
                Reference = Guid.NewGuid(),
                Name = "Hoge",
                Description = "Fuga",
                Address = random.Next(),
            };
        }

        public override bool Equals(object? obj)
        {
            return obj is TestObject2 @object &&
                   Id.Equals(@object.Id) &&
                   Reference.Equals(@object.Reference) &&
                   Name == @object.Name &&
                   Description == @object.Description &&
                   Address == @object.Address;
        }

        public Guid Id { get; set; }

        public Guid Reference { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Address { get; set; }
    }
}
