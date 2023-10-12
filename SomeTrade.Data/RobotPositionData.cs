namespace SomeTrade.Data
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using SomeTrade.Data.Infrastructure;
    using SomeTrade.Infrastructure.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public class RobotPositionData : EntityBaseData<Model.RobotPosition>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RobotPositionData(ILogger<object> logger, IServiceScopeFactory serviceScopeFactory) : base(logger, serviceScopeFactory) { 
            _serviceScopeFactory = serviceScopeFactory;
        }


        public Model.RobotPosition GetWithLogAndCandlesAndPnls(int id)
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                var dtNow = DateTime.Now;

                var _context = scope.ServiceProvider.GetService<DataContext>();

                return _context.Set<Model.RobotPosition>()
                    .Include(x => x.RobotPositionLogs)
                    .Include(x => x.RobotPositionCandles)
                    .Include(x => x.RobotPositionPnls)
                    .Where(x => x.Id == id)
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefault();
            }
        }

        /// <summary>
        /// Find an Entity
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public List<Model.RobotPosition> GetByWithLogs(Expression<Func<Model.RobotPosition, bool>> predicate)
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                _context = scope.ServiceProvider.GetService<DataContext>();

                try
                {
                    return _context.Set<Model.RobotPosition>()
                        .Include(x => x.RobotPositionLogs)
                        .Where(predicate)
                        .ToList();
                }
                catch (Exception exc)
                {
                    return null;
                }
            }
        }
    }
}
