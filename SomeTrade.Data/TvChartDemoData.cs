namespace SomeTrade.Data
{
    using Microsoft.Extensions.DependencyInjection;
    using SomeTrade.Data.Infrastructure;
    using SomeTrade.Infrastructure.Interfaces;

    public class TvChartDemoData : EntityBaseData<Model.TvChartDemo>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public TvChartDemoData(ILogger<object> logger, IServiceScopeFactory serviceScopeFactory) : base(logger, serviceScopeFactory) { 
            _serviceScopeFactory = serviceScopeFactory;
        }
    }
}
