using System.Threading.Tasks.Dataflow;
using ParameterDataLib;
using TelemetryHealthPredictor.Model.AlgorithmResultObjects;
using TelemetryHealthPredictor.Services.Algorithms.Abstractions;

namespace TelemetryHealthPredictor.Services.Dataflow
{
    public class Pipeline : IPipeline
    {
        private readonly TransformBlock<string, KeyValuePair<string, double>> _decoderBlock;
        private readonly TransformBlock<KeyValuePair<string, double>, FilteredResult> _kalmanBlock;
        private readonly BroadcastBlock<FilteredResult> _broadcastBlock;
        private readonly TransformBlock<FilteredResult, MahalanobisResult> _mahalanobisBlock;
        private readonly TransformBlock<FilteredResult, Dictionary<string, double>> _cusumBlock;
        private readonly JoinBlock<MahalanobisResult, Dictionary<string, double>> _joinBlock;
        private readonly TransformBlock<Tuple<MahalanobisResult, Dictionary<string, double>>, AlgorithmResult> _fusionBlock;
        private readonly ActionBlock<AlgorithmResult> _senderBlock;

        public Pipeline
        (
            IDecoder decoder, 
            IKalmanFilter kalmanFilter, 
            IMahalanobis mahalanobis, 
            ICusum cusum, 
            IFusion fusion, 
            IResultSender sender
        )
        {
            ExecutionDataflowBlockOptions algorithmBlockOptions = new()
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount,
                EnsureOrdered = true
            };

            GroupingDataflowBlockOptions groupingOptions = new()
            {
                EnsureOrdered = true
            };

            ExecutionDataflowBlockOptions senderOptions = new()
            {
                MaxDegreeOfParallelism = 1
            };

            _decoderBlock = new(decoder.DecoderDelegate, algorithmBlockOptions);
            _kalmanBlock = new(kalmanFilter.KalmanFilterDelegate, algorithmBlockOptions);
            _broadcastBlock = new(result => result);
            _mahalanobisBlock = new(mahalanobis.MahalanobisDelegate, algorithmBlockOptions);
            _cusumBlock = new(cusum.CusumDelegate, algorithmBlockOptions);
            _joinBlock = new(groupingOptions);
            _fusionBlock = new(fusion.FusionDelegate, algorithmBlockOptions);
            _senderBlock = new(sender.ResultSenderDelegate, senderOptions);

            DataflowLinkOptions linkOptions = new()
            {
                PropagateCompletion = true
            };

            _decoderBlock.LinkTo(_kalmanBlock, linkOptions);
            _kalmanBlock.LinkTo(_broadcastBlock, linkOptions);
            _broadcastBlock.LinkTo(_mahalanobisBlock, linkOptions);
            _broadcastBlock.LinkTo(_cusumBlock, linkOptions);
            _mahalanobisBlock.LinkTo(_joinBlock.Target1, linkOptions);
            _cusumBlock.LinkTo(_joinBlock.Target2, linkOptions);
            _joinBlock.LinkTo(_fusionBlock, linkOptions);
            _fusionBlock.LinkTo(_senderBlock, linkOptions);
        }
        public void BeginAlgorithm(string json)
        {
            _decoderBlock.Post(json);
        }

        public void Complete()
        {
            _decoderBlock.Complete();
        }

        public void Fault(Exception ex)
        {
            ((IDataflowBlock)_decoderBlock).Fault(ex);
        }

        public async Task Completion()
        {
            await _senderBlock.Completion;
        }
    }
}
