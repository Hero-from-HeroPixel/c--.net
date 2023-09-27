using System.Globalization;
using HelloWorld.Data;
using HelloWorld.Models;


internal class Program
{
    private static void Main(string[] args)
    {

        DataContextDapper dapper = new();
        DataContextEF entityFramework = new();

        string sqlCommand = "SELECT GETDATE()";

        DateTime rightNow = dapper.LoadDataSingle<DateTime>(sqlCommand);

        Console.WriteLine(rightNow);

        Computer myComputer = new Computer()
        {
            Motherboard = "B450",
            HasWifi = true,
            HasLTE = false,
            ReleaseDate = DateTime.Now,
            Price = 15850.00m,
            VideoCard = "RTX 2060"
        };

        entityFramework.Add(myComputer);
        entityFramework.SaveChanges();


        string sql = @"INSERT INTO TutorialAppSchema.Computer (
            Motherboard,
            HasWifi,
            HasLTE,
            ReleaseDate,
            Price,
            VideoCard  
        ) Values ('" + myComputer.Motherboard
                + "','" + myComputer.HasWifi
                + "','" + myComputer.HasLTE
                + "','" + myComputer.ReleaseDate
                + "','" + myComputer.Price.ToString("0.00", CultureInfo.InvariantCulture)
                + "','" + myComputer.VideoCard
            + "')";

        Console.WriteLine(sql);

        bool res = dapper.ExecuteSql(sql);

        Console.WriteLine(res);

        string sqlSelect = @"
        select 
            Computer.Motherboard,
            Computer.HasWifi,
            Computer.HasLTE,
            Computer.ReleaseDate,
            Computer.Price,
            Computer.VideoCard  
        from TutorialAppSchema.Computer";

        IEnumerable<Computer> computers = dapper.LoadData<Computer>(sqlSelect);
        IEnumerable<Computer>? computersEf = entityFramework.Computer?.ToList<Computer>();

        if (computersEf != null)
        {

            foreach (Computer computer in computersEf)
            {
                Console.WriteLine("'" + computer.ComputerId
                    + "','" + computer.Motherboard
                    + "','" + computer.HasWifi
                    + "','" + computer.HasLTE
                    + "','" + computer.ReleaseDate
                    + "','" + computer.Price.ToString("0.00", CultureInfo.InvariantCulture)
                    + "','" + computer.VideoCard
                + "'");
            }
        }
        else
        {
            foreach (Computer computer in computers)
            {
                Console.WriteLine("'" + computer.ComputerId
                    + "','" + computer.Motherboard
                    + "','" + computer.HasWifi
                    + "','" + computer.HasLTE
                    + "','" + computer.ReleaseDate
                    + "','" + computer.Price.ToString("0.00", CultureInfo.InvariantCulture)
                    + "','" + computer.VideoCard
                + "'");
            }
        }

    }

}