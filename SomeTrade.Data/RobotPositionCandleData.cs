namespace SomeTrade.Data
{
    using Microsoft.Extensions.DependencyInjection;
    using SomeTrade.Data.Infrastructure;
    using SomeTrade.Infrastructure.Interfaces;

    public class RobotPositionCandleData : EntityBaseData<Model.RobotPositionCandle>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RobotPositionCandleData(ILogger<object> logger, IServiceScopeFactory serviceScopeFactory) : base(logger, serviceScopeFactory) { 
            _serviceScopeFactory = serviceScopeFactory;
        }
    }
}
