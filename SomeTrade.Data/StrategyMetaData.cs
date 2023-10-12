namespace SomeTrade.Data
{
    using Microsoft.Extensions.DependencyInjection;
    using SomeTrade.Data.Infrastructure;
    using SomeTrade.Infrastructure.Interfaces;

    public class StrategyMetaData : EntityBaseData<Model.StrategyMeta>
    {
        public StrategyMetaData(ILogger<object> logger, IServiceScopeFactory serviceScopeFactory) : base(logger, serviceScopeFactory) { }
    }
}
