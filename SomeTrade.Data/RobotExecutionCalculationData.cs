namespace SomeTrade.Data
{
    using Microsoft.Extensions.DependencyInjection;
    using SomeTrade.Data.Infrastructure;
    using SomeTrade.Infrastructure.Interfaces;

    public class RobotExecutionCalculationData : EntityBaseData<Model.RobotExecutionCalculation>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RobotExecutionCalculationData(ILogger<object> logger, IServiceScopeFactory serviceScopeFactory) : base(logger, serviceScopeFactory) { 
            _serviceScopeFactory = serviceScopeFactory;
        }
    }
}
