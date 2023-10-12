using SomeTrade.Strategies.Dto;

namespace SomeTrade.Strategies.Interfaces
{
    public interface IStrategy
    {
        CheckResult ShouldOpenLong();
        CheckResult ShouldOpenShort();
        CheckResult ShouldTakeProfitLong();
        CheckResult ShouldTakeProfitShort();
        bool UpdateTrailingStop();
        bool CheckAlarm();
        bool ShouldStop(double entryPrice, int side);
    }
}
