namespace SomeTrade.Data
{
    using Microsoft.Extensions.DependencyInjection;
    using SomeTrade.Data.Infrastructure;
    using SomeTrade.Infrastructure.Interfaces;

    public class RobotSymbolPairData : EntityBaseData<Model.RobotSymbolPair>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RobotSymbolPairData(ILogger<object> logger, IServiceScopeFactory serviceScopeFactory) : base(logger, serviceScopeFactory) { 
            _serviceScopeFactory = serviceScopeFactory;
        }
    }
}
