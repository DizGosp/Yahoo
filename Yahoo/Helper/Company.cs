namespace Yahoo.Controllers
{
    public class Company
    {
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string Sector { get; set; }

        internal static Company ParseRow(string row)
        {
            var columns = row.Split(',');
            return new Company()
            {
                Symbol = columns[0],
                Name = columns[1]
            };
        }
    }
}