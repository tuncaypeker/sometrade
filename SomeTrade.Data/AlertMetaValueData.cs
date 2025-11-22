namespace SomeTrade.Data
{
    using Microsoft.Extensions.DependencyInjection;
    using SomeTrade.Data.Infrastructure;
    using SomeTrade.Infrastructure.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class AlertMetaValueData : EntityBaseData<Model.AlertMetaValue>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public AlertMetaValueData(ILogger<object> logger, IServiceScopeFactory serviceScopeFactory) : base(logger, serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public List<Model.AlertMetaValue> GetByMetaIds(int alertId, int[] vs)
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetService<DataContext>();

                return _context.Set<Model.AlertMetaValue>()
                    .Where(x => vs.Contains(x.ScreeningMetaId) && x.AlertId == alertId)
                    .OrderByDescending(x => x.Id)
                    .ToList();
            }
        }
    }
}
