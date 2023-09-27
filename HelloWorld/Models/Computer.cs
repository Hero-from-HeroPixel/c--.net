namespace HelloWorld.Models
{
    public class Computer
    {

        public string Motherboard { get; set; }
        /************ Under the hood */
        // private string _motherboard;
        // private string Motherboard {get{ return _motherboard; } set{ _motherboard = value; }}

        public int CPUCores { get; set; }

        public bool HasWifi { get; set; }

        public bool HasLTE { get; set; }

        public DateTime ReleaseDate { get; set; }

        public decimal Price { get; set; }

        public string VideoCard { set; get; } = "";

        /***** To ways of default values */
        public Computer()
        {
            Motherboard ??= "";
        }
    }
}


