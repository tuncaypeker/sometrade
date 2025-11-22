namespace SomeTrade.Data
{
    using Microsoft.Extensions.DependencyInjection;
    using SomeTrade.Data.Infrastructure;
    using SomeTrade.Infrastructure.Interfaces;

    public class RobotExecutionCandleData : EntityBaseData<Model.RobotExecutionCandle>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RobotExecutionCandleData(ILogger<object> logger, IServiceScopeFactory serviceScopeFactory) : base(logger, serviceScopeFactory) { 
            _serviceScopeFactory = serviceScopeFactory;
        }
    }
}
