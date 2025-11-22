namespace SomeTrade.Data
{
    using Microsoft.Extensions.DependencyInjection;
    using SomeTrade.Data.Infrastructure;
    using SomeTrade.Infrastructure.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class RobotMetaValueData : EntityBaseData<Model.RobotMetaValue>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RobotMetaValueData(ILogger<object> logger, IServiceScopeFactory serviceScopeFactory) : base(logger, serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public List<Model.RobotMetaValue> GetByMetaIds(int[] vs, int robotId)
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetService<DataContext>();

                return _context.Set<Model.RobotMetaValue>()
                    .Where(x => vs.Contains(x.StrategyMetaId) && x.RobotId == robotId)
                    .OrderByDescending(x => x.Id)
                    .ToList();
            }
        }
    }
}
