using HelloWorld.Models;


internal class Program
{
    private static void Main(string[] args)
    {
        Computer myComputer = new Computer()
        {
            Motherboard = "B450",
            HasWifi = true,
            HasLTE = false,
            ReleaseDate = DateTime.Now,
            Price = 15850.00m,
            VideoCard = "RTX 2060"
        };

    }

}