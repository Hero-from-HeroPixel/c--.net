namespace HelloWorld.Models
{
    public class ComputerSnake
    {

        public int computer_id { get; set; }
        public string motherboard { get; set; }
        /************ Under the hood */
        // private string _motherboard;
        // private string Motherboard {get{ return _motherboard; } set{ _motherboard = value; }}

        public int? cpu_cores { get; set; } = 0;

        public bool has_wifi { get; set; }

        public bool has_lte { get; set; }

        public DateTime? release_date { get; set; }

        public decimal price { get; set; }

        public string video_card { set; get; } = "";

        /***** To ways of default values */
        public ComputerSnake()
        {
            cpu_cores ??= 0;
            motherboard ??= "";
        }
    }
}


