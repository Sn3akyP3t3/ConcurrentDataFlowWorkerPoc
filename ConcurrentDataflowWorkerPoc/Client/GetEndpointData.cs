using ConcurrentDataflowWorkerPoc.Repo;

namespace ConcurrentDataflowWorkerPoc.Client;

public interface IGetEndpointData
{
    Task<Data> CallFirstEndpointAsync(Data inputData, string urlPath = "/posts");
    Task<List<Data>> CallLastEndpointAsync(IEnumerable<Data> inputData, string urlPath = "/posts");
}

public class GetEndpointData : IGetEndpointData
{
    private readonly HttpClient _client;
    private readonly Random _random;
    private int _iterator;
    private int _callLastEndpointAsyncCountTracker = 0;

    public GetEndpointData(HttpClient client)
    {
        _client = client;
        _iterator = 0;
        _random = new Random();
    }

    public async Task<Data> CallFirstEndpointAsync(Data inputData, string urlPath = "/posts")
    {
        Data data = new();
        var response = await _client.GetAsync(urlPath);
        if (response != null && response.IsSuccessStatusCode)
        {
            data.Payload = await response.Content.ReadAsStringAsync();
            data.MasterFakedId = _iterator++;
            Console.WriteLine($"CallFirstEndpointAsync");
        }

        return data;
    }

    public async Task<List<Data>> CallLastEndpointAsync(IEnumerable<Data> inputDataBundle, string urlPath = "/posts")
    {
        List<Data> dataList = [];
        int ct = 0;
        foreach (Data data in inputDataBundle)
        {
            var response = await _client.GetAsync(urlPath);
            if (response != null && response.IsSuccessStatusCode)
            {
                data.Days = _random.Next(1, 500);
                dataList.Add(data);
                Console.WriteLine($"CallLastEndpointAsync - Current Call: {ct++}; Total Call: {_callLastEndpointAsyncCountTracker++}");
            }
        }

        return dataList;
    }

    public void ResetIterator()
    {
        _iterator = 0;
    }
}
