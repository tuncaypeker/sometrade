namespace SomeTrade.Data
{
    using Microsoft.Extensions.DependencyInjection;
    using SomeTrade.Data.Infrastructure;
    using SomeTrade.Infrastructure.Interfaces;

    public class RobotTickLogData : EntityBaseData<Model.RobotTickLog>
    {
        public RobotTickLogData(ILogger<object> logger, IServiceScopeFactory serviceScopeFactory) : base(logger, serviceScopeFactory) { }
    }
}
