using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using PipelineDataFlow.Models;

namespace PipelineDataFlow.Dataflow
{
    public class PipelineDataflow
    {
        // Declare blocks
        private readonly ISourceBlock<TargetPipeline> _sourceBlock;
        private readonly IPropagatorBlock<TargetPipeline, TargetPipeline> _transformBlock;
        private readonly ITargetBlock<TargetPipeline> _targetBlock;

        public PipelineDataflow()
        {
            // Source Block: Produces PipelineData
            _sourceBlock = new BufferBlock<TargetPipeline>();

            // Transform Block: Example transformation
            _transformBlock = new TransformBlock<TargetPipeline, TargetPipeline>(data => // Entity must be source and output
            {
                data.Transformed = true; // If already transformed
                return data;
            });

            // Target Block: Consumes the transformed data
            _targetBlock = new ActionBlock<TargetPipeline>(data =>
            {
                Console.WriteLine($"Processed Data: {data.Id}, Transformed: {data.Transformed}");
                // Logic for saving data into database supposed to be below
            });

            // Linking blocks
            _sourceBlock.LinkTo(
                _transformBlock,
                new DataflowLinkOptions { PropagateCompletion = true }
            );
            _transformBlock.LinkTo(
                _targetBlock,
                new DataflowLinkOptions { PropagateCompletion = true }
            );
        }

        // Method to post data to the source block
        public void PostData(PipelineData data)
        {
            (_sourceBlock as ITargetBlock<PipelineData>)?.Post(data);
        }

        // Method to complete the source block
        public void Complete()
        {
            _sourceBlock.Complete();
        }

        // Method to await completion of the target block
        public Task Completion => _targetBlock.Completion;
    }
}
