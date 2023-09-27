using System.Data;
using System.Data.Common;
using System.Globalization;
using Dapper;
using HelloWorld.Data;
using HelloWorld.Models;
using Microsoft.Data.SqlClient;


internal class Program
{
    private static void Main(string[] args)
    {

        DataContextDapper dapper = new DataContextDapper();

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

        foreach (Computer computer in computers)
        {
            Console.WriteLine("'" + myComputer.Motherboard
                + "','" + myComputer.HasWifi
                + "','" + myComputer.HasLTE
                + "','" + myComputer.ReleaseDate
                + "','" + myComputer.Price.ToString("0.00", CultureInfo.InvariantCulture)
                + "','" + myComputer.VideoCard
            + "'");
        }

    }

}