namespace ConcurrentDataflowWorkerPoc.Repo;

public class Data
{
    public string Payload { get; set; } = string.Empty;

    public int FakedId { get; set; }

    public int MasterFakedId { get; set; }

    public int Days { get; set; }
}

