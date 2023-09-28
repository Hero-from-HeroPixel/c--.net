using System.Globalization;
using System.Text.Json;
using HelloWorld.Data;
using HelloWorld.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;


internal class Program
{
    private static void Main(string[] args)
    {

        IConfiguration configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build();

        DataContextDapper dapper = new(configuration);
        DataContextEF entityFramework = new(configuration);

        // string sqlCommand = "SELECT GETDATE()";

        // DateTime rightNow = dapper.LoadDataSingle<DateTime>(sqlCommand);

        // Console.WriteLine(rightNow);

        // Computer myComputer = new Computer()
        // {
        //     Motherboard = "B450",
        //     HasWifi = true,
        //     HasLTE = false,
        //     ReleaseDate = DateTime.Now,
        //     Price = 15850.00m,
        //     VideoCard = "RTX 2060"
        // };

        // entityFramework.Add(myComputer);
        // entityFramework.SaveChanges();




        // File.WriteAllText("log.txt", sql);

        // using StreamWriter openFile = new("log.txt", append : true);

        // openFile.WriteLine(sql);

        // openFile.Close();

        string computersJson = File.ReadAllText("computers.json");
        //Console.WriteLine(computersJson);

        JsonSerializerOptions options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        // IEnumerable<Computer>? computers = JsonSerializer.Deserialize<IEnumerable<Computer>>(computersJson,options);

        IEnumerable<Computer>? computers = JsonConvert.DeserializeObject<IEnumerable<Computer>>(computersJson);

        if (computers != null)
        {
            foreach (Computer computer in computers)
            {
                // Console.WriteLine(computer.Motherboard);
                string sql = "\n" + @"INSERT INTO TutorialAppSchema.Computer (
            Motherboard,
            HasWifi,
            HasLTE,
            ReleaseDate,
            Price,
            VideoCard  
        ) Values ('" + EscapeSingleQuote(computer.Motherboard)
                + "','" + computer.HasWifi
                + "','" + computer.HasLTE
                + "','" + computer.ReleaseDate
                + "','" + computer.Price.ToString("0.00", CultureInfo.InvariantCulture)
                + "','" + EscapeSingleQuote(computer.VideoCard)
            + "') \n";

                dapper.ExecuteSql(sql);
            }

        }

        JsonSerializerSettings settings = new()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        string computersCopy = JsonConvert.SerializeObject(computers, settings);
        File.WriteAllText("computersCopyNewtonsoft.txt", computersCopy);

        string computersCopySystem = System.Text.Json.JsonSerializer.Serialize(computers, options);
        File.WriteAllText("computersCopySystem.txt", computersCopy);
        // Console.WriteLine(sql);

        // bool res = dapper.ExecuteSql(sql);

        // Console.WriteLine(res);

        // string sqlSelect = @"
        // select 
        //     Computer.Motherboard,
        //     Computer.HasWifi,
        //     Computer.HasLTE,
        //     Computer.ReleaseDate,
        //     Computer.Price,
        //     Computer.VideoCard  
        // from TutorialAppSchema.Computer";

        // IEnumerable<Computer> computers = dapper.LoadData<Computer>(sqlSelect);
        // IEnumerable<Computer>? computersEf = entityFramework.Computer?.ToList<Computer>();

        // if (computersEf != null)
        // {

        //     foreach (Computer computer in computersEf)
        //     {
        //         Console.WriteLine("'" + computer.ComputerId
        //             + "','" + computer.Motherboard
        //             + "','" + computer.HasWifi
        //             + "','" + computer.HasLTE
        //             + "','" + computer.ReleaseDate
        //             + "','" + computer.Price.ToString("0.00", CultureInfo.InvariantCulture)
        //             + "','" + computer.VideoCard
        //         + "'");
        //     }
        // }
        // else
        // {
        //     foreach (Computer computer in computers)
        //     {
        //         Console.WriteLine("'" + computer.ComputerId
        //             + "','" + computer.Motherboard
        //             + "','" + computer.HasWifi
        //             + "','" + computer.HasLTE
        //             + "','" + computer.ReleaseDate
        //             + "','" + computer.Price.ToString("0.00", CultureInfo.InvariantCulture)
        //             + "','" + computer.VideoCard
        //         + "'");
        //     }
        // }


    }

    static string EscapeSingleQuote(string input)
    {
        string output = input.Replace("'", "''");
        return output;
    }

}