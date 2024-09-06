using ConcurrentDataflowWorkerPoc.Client;
using ConcurrentDataflowWorkerPoc.Repo;
using System.Threading.Tasks.Dataflow;

namespace ConcurrentDataflowWorkerPoc;

public interface IDataWorkflowService
{
    Task DoProcess();
}

public class DataWorkflowService : IDataWorkflowService
{
    private readonly IGetEndpointData _clientCaller;
    private readonly TransformBlock<Data, Data> _getFirstDataBlock;
    private readonly BatchBlock<Data> _batchBlock;
    private readonly TransformBlock<IEnumerable<Data>, List<Data>> _getLastDataBlock;

    public DataWorkflowService(IGetEndpointData clientCaller)
    {
        _clientCaller = clientCaller;

        var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

        GroupingDataflowBlockOptions groupingDataflowBlockOptions = new()
        {
            Greedy = true
        };

        ExecutionDataflowBlockOptions executionDataflowBlockOptions = new()
        {
            MaxDegreeOfParallelism = 4
        };

        _getFirstDataBlock = new(async input => await _clientCaller.CallFirstEndpointAsync(input) ,executionDataflowBlockOptions);
        _batchBlock = new BatchBlock<Data>(4, groupingDataflowBlockOptions);
        _getLastDataBlock = new(async input => await _clientCaller.CallLastEndpointAsync(input) ,executionDataflowBlockOptions);

        _getFirstDataBlock.LinkTo(_batchBlock, linkOptions);
        _batchBlock.LinkTo(_getLastDataBlock, linkOptions);
        _batchBlock.Completion.ContinueWith(_ => _getLastDataBlock.Complete());
    }

    public async Task DoProcess()
    {
        var originalData = GenerateData();

        foreach (var dataSet in originalData)
        {
            await _getFirstDataBlock.SendAsync(dataSet);
        }

        _getFirstDataBlock.Complete();
        IAsyncEnumerable<List<Data>> dataSetsBundle = _getLastDataBlock.ReceiveAllAsync();

        await foreach (var dataSets in dataSetsBundle)
        {
            foreach (var dataSet in dataSets)
            {
                Console.WriteLine($"FakedId: {dataSet.FakedId}; MasterFakedId: {dataSet.MasterFakedId}; Days: {dataSet.Days}; Payload: {dataSet.Payload};");
            }
        }

        // Although _getLastDataBlock is completed when the async foreach loop above concludes, it
        // might have been completed in a cancelled or faulty state. To ensure exceptions and such
        // related to cancellation or faulty completion are propagated by the async state machine of the
        // async DoProcess method, _getLastDataBlock.Completion is still being awaited after the loop.
        await _getLastDataBlock.Completion;

        string validate = string.Empty;
    }

    private static List<Data> GenerateData()
    {
        var dataList = new List<Data>();

        List<int> fakeIdList = [90, 10, 80, 20, 70, 30, 60, 40, 50, 100];

        Console.WriteLine("GetAllData");
        foreach (int fakeId in fakeIdList)
        {
            Console.WriteLine(fakeId);
            var data = new Data
            {
                FakedId = fakeId,
            };

            dataList.Add(data);
        }

        return dataList;
    }
}
