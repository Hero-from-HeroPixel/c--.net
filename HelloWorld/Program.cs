using HelloWorld.Data;
using HelloWorld.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;


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

        string computersJson = File.ReadAllText("ComputersSnake.json");
        //Console.WriteLine(computersJson);

        // Mapper mappper = new(new MapperConfiguration((cfg) =>
        // {
        //     cfg.CreateMap<ComputerSnake, Computer>()
        //     .ForMember(destination => destination.ComputerId, options => options.MapFrom(source => source.computer_id))
        //     .ForMember(destination => destination.CPUCores, options => options.MapFrom(source => source.cpu_cores))
        //     .ForMember(destination => destination.HasLTE, options => options.MapFrom(source => source.has_lte))
        //     .ForMember(destination => destination.HasWifi, options => options.MapFrom(source => source.has_wifi))
        //     .ForMember(destination => destination.Motherboard, options => options.MapFrom(source => source.motherboard))
        //     .ForMember(destination => destination.VideoCard, options => options.MapFrom(source => source.video_card))
        //     .ForMember(destination => destination.ReleaseDate, options => options.MapFrom(source => source.release_date))
        //     .ForMember(destination => destination.Price, options => options.MapFrom(source => source.price));

        // }));
        IEnumerable<Computer>? computersSystem = JsonConvert.DeserializeObject<IEnumerable<Computer>>(computersJson);

        if (computersSystem != null)
        {
            // IEnumerable<Computer> computerResult = mappper.Map<IEnumerable<Computer>>(computersSystem);

            foreach (Computer computer in computersSystem)
            {
                Console.WriteLine(computer.Motherboard);
            }
        }

        // JsonSerializerOptions options = new()
        // {
        //     PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        // };

        // // IEnumerable<Computer>? computers = JsonSerializer.Deserialize<IEnumerable<Computer>>(computersJson,options);

        // IEnumerable<Computer>? computers = JsonConvert.DeserializeObject<IEnumerable<Computer>>(computersJson);

        // if (computers != null)
        // {
        //     foreach (Computer computer in computers)
        //     {
        //         // Console.WriteLine(computer.Motherboard);
        //         string sql = "\n" + @"INSERT INTO TutorialAppSchema.Computer (
        //     Motherboard,
        //     HasWifi,
        //     HasLTE,
        //     ReleaseDate,
        //     Price,
        //     VideoCard  
        // ) Values ('" + EscapeSingleQuote(computer.Motherboard)
        //         + "','" + computer.HasWifi
        //         + "','" + computer.HasLTE
        //         + "','" + computer.ReleaseDate
        //         + "','" + computer.Price.ToString("0.00", CultureInfo.InvariantCulture)
        //         + "','" + EscapeSingleQuote(computer.VideoCard)
        //     + "') \n";

        //         dapper.ExecuteSql(sql);
        //     }
        // }

        // JsonSerializerSettings settings = new()
        // {
        //     ContractResolver = new CamelCasePropertyNamesContractResolver()
        // };

        // string computersCopy = JsonConvert.SerializeObject(computers, settings);
        // File.WriteAllText("computersCopyNewtonsoft.txt", computersCopy);

        // string computersSystem = System.Text.Json.JsonSerializer.Serialize(computers);
        // File.WriteAllText("computersCopySystem.txt", computersCopy);
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