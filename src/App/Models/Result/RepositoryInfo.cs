namespace App.Models.Result
{
    public class RepositoryInfo
    {
        public string Name { get; set; }
        public long RepositoryId { get; set; }
        public string Url { get; set; }
        public string Branch { get; set; }
        public int BranchId { get; set; }
    }
}
