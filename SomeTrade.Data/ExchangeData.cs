namespace SomeTrade.Data
{
    using Microsoft.Extensions.DependencyInjection;
    using SomeTrade.Data.Infrastructure;
    using SomeTrade.Infrastructure.Interfaces;

    public class ExchangeData : EntityBaseData<Model.Exchange>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ExchangeData(ILogger<object> logger, IServiceScopeFactory serviceScopeFactory) : base(logger, serviceScopeFactory) { 
            _serviceScopeFactory = serviceScopeFactory;
        }
    }
}
