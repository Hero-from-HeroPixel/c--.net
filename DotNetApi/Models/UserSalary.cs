namespace DotNetApi
{
    public partial class UserSalary
    {
        public int UserId { get; set; }
        public string Salary { get; set; } = "";
        public string AvgSalary { get; set; } = "";
    }
}