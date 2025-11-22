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

        /// <summary>
        /// Find an Entity
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public List<Model.RobotPosition> GetByWithCalculations(Expression<Func<Model.RobotPosition, bool>> predicate)
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                _context = scope.ServiceProvider.GetService<DataContext>();

                try
                {
                    return _context.Set<Model.RobotPosition>()
                        .Include(x => x.RobotPositionCalculations)
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
