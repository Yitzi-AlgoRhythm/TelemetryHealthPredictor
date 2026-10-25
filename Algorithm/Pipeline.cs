using System.Threading.Tasks.Dataflow;
using TelemetryHealthPredictor.Algorithm.Delegates.Abstractions;
using TelemetryHealthPredictor.Data;

namespace TelemetryHealthPredictor.Algorithm
{
    public class Pipeline : IPipeline
    {
        private readonly TransformManyBlock<IncomingFlightParameter[], KalmanFilterResult> _kalmanBlock;
        private readonly BroadcastBlock<KalmanFilterResult> _broadcastBlock;
        private readonly TransformBlock<KalmanFilterResult, MahalanobisResult> _mahalanobisBlock;
        private readonly TransformBlock<KalmanFilterResult, CusumResult> _cusumBlock;
        private readonly JoinBlock<MahalanobisResult, CusumResult> _joinBlock;
        private readonly TransformBlock<Tuple<MahalanobisResult, CusumResult>, AlgorithmResult> _fusionBlock;
        private readonly ActionBlock<AlgorithmResult> _senderBlock;

        public Pipeline
        (
            IKalmanFilter kalmanFilter,
            IMahalanobis mahalanobis, 
            ICusum cusum, 
            IFusion fusion, 
            IResultSender sender
        )
        {
            ExecutionDataflowBlockOptions blockOptions = new()
            {
                MaxDegreeOfParallelism = 1
            };

            GroupingDataflowBlockOptions groupingOptions = new()
            {
                EnsureOrdered = true
            };

            _kalmanBlock = new(kalmanFilter.KalmanFilterDelegate, blockOptions);
            _broadcastBlock = new(result => result);
            _mahalanobisBlock = new(mahalanobis.MahalanobisDelegate, blockOptions);
            _cusumBlock = new(cusum.CusumDelegate, blockOptions);
            _joinBlock = new(groupingOptions);
            _fusionBlock = new(fusion.FusionDelegate, blockOptions);
            _senderBlock = new(sender.ResultSenderDelegate, blockOptions);

            DataflowLinkOptions linkOptions = new()
            {
                PropagateCompletion = true
            };

            _kalmanBlock.LinkTo(_broadcastBlock, linkOptions);
            _broadcastBlock.LinkTo(_mahalanobisBlock, linkOptions);
            _broadcastBlock.LinkTo(_cusumBlock, linkOptions);
            _mahalanobisBlock.LinkTo(_joinBlock.Target1, linkOptions);
            _cusumBlock.LinkTo(_joinBlock.Target2, linkOptions);
            _joinBlock.LinkTo(_fusionBlock, linkOptions);
            _fusionBlock.LinkTo(_senderBlock, linkOptions);
        }
        public void BeginAlgorithm(IncomingFlightParameter[] parameters)
        {
            _kalmanBlock.Post(parameters);
        }

        public void Complete()
        {
            _kalmanBlock.Complete();
        }

        public void Fault(Exception ex)
        {
            ((IDataflowBlock)_kalmanBlock).Fault(ex);
        }

        public async Task Completion()
        {
            await _senderBlock.Completion;
        }
    }
}
