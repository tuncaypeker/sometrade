namespace SomeTrade.Data
{
	using Microsoft.EntityFrameworkCore;
	using SomeTrade.Data.Infrastructure;

	public class DataContext : DbContext
	{
		public DataContext(string connectionString)
			: base(new DbContextOptionsBuilder().UseMySQL(connectionString).Options)
		{
		}

		public DbSet<Model.Symbol> Symbols { get; set; }
		public DbSet<Model.SymbolPair> SymbolPairs { get; set; }
		public DbSet<Model.Exchange> Markets { get; set; }

		public DbSet<Model.Strategy> Strategies { get; set; }
		public DbSet<Model.StrategyMeta> StrategyMetas { get; set; }
		public DbSet<Model.StrategyVersion> StrategyVersions { get; set; }

		public DbSet<Model.Role> Roles { get; set; }
		public DbSet<Model.RolePage> RolePages { get; set; }
		public DbSet<Model.User> Users { get; set; }

		public DbSet<Model.Robot> Robots { get; set; }

		public DbSet<Model.RobotExecution> RobotExecutions { get; set; }
		public DbSet<Model.RobotExecutionCalculation> RobotExecutionCalculations { get; set; }
		public DbSet<Model.RobotExecutionCandle> RobotExecutionCandles { get; set; }

		public DbSet<Model.RobotPosition> RobotPositions { get; set; }
		public DbSet<Model.RobotPositionCalculation> RobotPositionCalculations { get; set; }
		public DbSet<Model.RobotPositionCandle> RobotPositionCandles { get; set; }
		public DbSet<Model.RobotPositionPnl> RobotPositionPnls { get; set; }
		public DbSet<Model.RobotPositionStopLog> RobotPositionStopLogs { get; set; }

		public DbSet<Model.RobotMetaValue> RobotMetaValues { get; set; }
		public DbSet<Model.RobotSymbolPair> RobotSymbolPairs { get; set; }

		public DbSet<Model.Screening> Screenings { get; set; }
		public DbSet<Model.ScreeningMeta> ScreeningMetas { get; set; }
		public DbSet<Model.ScreeningVersion> ScreeningVersions { get; set; }

		public DbSet<Model.Alert> Alerts { get; set; }
		public DbSet<Model.AlertSymbolPair> AlertSymbolPairs { get; set; }
		public DbSet<Model.AlertMetaValue> AlertMetaValues { get; set; }
		public DbSet<Model.AlertLog> AlertLogs { get; set; }

		public DbSet<Model.JobEditQueue> JobEditQueues { get; set; }

		public DbSet<Model.Backtest> Backtests { get; set; }
		public DbSet<Model.BacktestPosition> BacktestPositions { get; set; }
		public DbSet<Model.BacktestMetaValue> BacktestMetaValues { get; set; }
		public DbSet<Model.DataFile> DataFiles { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.Entity<Model.Symbol>(entity => entity.ToTable("tb_symbols"));
			builder.Entity<Model.SymbolPair>(entity => entity.ToTable("tb_symbol_pairs"));
			builder.Entity<Model.Exchange>(entity => entity.ToTable("tb_exchanges"));

			builder.Entity<Model.Strategy>(entity => entity.ToTable("tb_strategies"));
			builder.Entity<Model.StrategyMeta>(entity => entity.ToTable("tb_strategy_metas"));
			builder.Entity<Model.StrategyVersion>(entity => entity.ToTable("tb_strategy_versions"));

			builder.Entity<Model.Screening>(entity => entity.ToTable("tb_screenings"));
			builder.Entity<Model.ScreeningMeta>(entity => entity.ToTable("tb_screening_metas"));
			builder.Entity<Model.ScreeningVersion>(entity => entity.ToTable("tb_screening_versions"));

			builder.Entity<Model.Role>(entity => entity.ToTable("tb_roles"));
			builder.Entity<Model.RolePage>(entity => entity.ToTable("tb_role_pages"));
			builder.Entity<Model.User>(entity => entity.ToTable("tb_users"));

			builder.Entity<Model.Robot>(entity => entity.ToTable("tb_robots"));

			builder.Entity<Model.RobotExecution>(entity => entity.ToTable("tb_robot_executions"));
			builder.Entity<Model.RobotExecutionCandle>(entity => entity.ToTable("tb_robot_execution_candles"));
			builder.Entity<Model.RobotExecutionCalculation>(entity => entity.ToTable("tb_robot_execution_calculations"));

			builder.Entity<Model.RobotPosition>(entity => entity.ToTable("tb_robot_positions"));
			builder.Entity<Model.RobotPositionCalculation>(entity => entity.ToTable("tb_robot_position_calculations"));
			builder.Entity<Model.RobotPositionCandle>(entity => entity.ToTable("tb_robot_position_candles"));
			builder.Entity<Model.RobotPositionPnl>(entity => entity.ToTable("tb_robot_position_pnls"));
			builder.Entity<Model.RobotPositionStopLog>(entity => entity.ToTable("tb_robot_position_stop_logs"));
			builder.Entity<Model.RobotMetaValue>(entity => entity.ToTable("tb_robot_meta_values"));
			builder.Entity<Model.RobotSymbolPair>(entity => entity.ToTable("tb_robot_symbol_pairs"));

			builder.Entity<Model.Alert>(entity => entity.ToTable("tb_alerts"));
			builder.Entity<Model.AlertMetaValue>(entity => entity.ToTable("tb_alert_meta_values"));
			builder.Entity<Model.AlertSymbolPair>(entity => entity.ToTable("tb_alert_symbol_pairs"));
			builder.Entity<Model.AlertLog>(entity => entity.ToTable("tb_alert_logs"));

			builder.Entity<Model.JobEditQueue>(entity => entity.ToTable("tb_job_edit_queues"));

			builder.Entity<Model.Backtest>(entity => entity.ToTable("tb_backtests"));
			builder.Entity<Model.BacktestPosition>(entity => entity.ToTable("tb_backtest_positions"));
			builder.Entity<Model.BacktestMetaValue>(entity => entity.ToTable("tb_backtest_meta_values"));
			builder.Entity<Model.DataFile>(entity => entity.ToTable("tb_data_files"));

			foreach (var entityType in builder.Model.GetEntityTypes())
			{
				foreach (var property in entityType.GetProperties())
				{
					if (property.ClrType == typeof(bool))
					{
						property.SetValueConverter(new BoolToIntConverter());
					}
				}
			}
		}
	}
}