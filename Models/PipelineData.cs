using Microsoft.EntityFrameworkCore;

namespace PipelineDataFlow.Models;

[Keyless]
public class PipelineData
{
    public Dictionary<string, object?> Data { get; set; } = new Dictionary<string, object?>();
}
