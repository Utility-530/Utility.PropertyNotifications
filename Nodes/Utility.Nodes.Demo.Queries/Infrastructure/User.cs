namespace Utility.Nodes.Demo.Queries
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public IEnumerable<Car> Cars { get; set; }
    }

    public class Car
    {
        public int CarId { get; set; }
        public string Model { get; set; }
        public int MaxSpeed { get; set; }
    }
}