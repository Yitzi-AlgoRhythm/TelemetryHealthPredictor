using System.Threading.Tasks.Dataflow;
using MathNet.Numerics.LinearAlgebra;
using TelemetryHealthPredictor.Model.AlgorithmResultObjects;
using TelemetryHealthPredictor.Services.Algorithms.Abstractions;

namespace TelemetryHealthPredictor.Services.Dataflow
{
    public class Pipeline : IPipeline
    {
        private readonly TransformBlock<string, KeyValuePair<string, double>> _decoderBlock;
        private readonly TransformManyBlock<KeyValuePair<string, double>, Vector<double>> _kalmanBlock;
        private readonly BroadcastBlock<Vector<double>> _broadcastBlock;
        private readonly TransformBlock<Vector<double>, MahalanobisResult> _mahalanobisBlock;
        private readonly TransformBlock<Vector<double>, Dictionary<string, double>> _cusumBlock;
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
            ExecutionDataflowBlockOptions blockOptions = new()
            {
                MaxDegreeOfParallelism = 1
            };

            GroupingDataflowBlockOptions groupingOptions = new()
            {
                EnsureOrdered = true
            };

            _decoderBlock = new(decoder.DecoderDelegate, blockOptions);
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
