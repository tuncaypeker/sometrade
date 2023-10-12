namespace SomeTrade.Data
{
    using Microsoft.Extensions.DependencyInjection;
    using SomeTrade.Data.Infrastructure;
    using SomeTrade.Infrastructure.Interfaces;

    public class RobotPositionPnlData : EntityBaseData<Model.RobotPositionPnl>
    {
        public RobotPositionPnlData(ILogger<object> logger, IServiceScopeFactory serviceScopeFactory) : base(logger, serviceScopeFactory) { }
    }
}
