using System.Threading;
using System.Threading.Tasks;

namespace Hangfire.Abstractions
{
    public interface IJob
    {
        string Id { get; }
        Task Run(CancellationToken cancellationToken = default);
    }

    public interface IJob<in T>
        where T : IJobArgs
    {
        string Id { get; }
        Task Run(T data, CancellationToken cancellationToken = default);
    }
}
