using Microsoft.Extensions.Logging;
using Quartz;
using System.Threading.Tasks;

namespace TVShowCast.Presentation
{
    [DisallowConcurrentExecution]
    public class TvMazeScrapperJob: IJob
    {
        private readonly TVMazeScapper _tvMazeScapper;

        public TvMazeScrapperJob(TVMazeScapper tvMazeScapper)
        {
            _tvMazeScapper = tvMazeScapper;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _tvMazeScapper.ExecuteAsync();
            return Task.CompletedTask;
        }
    }
}
