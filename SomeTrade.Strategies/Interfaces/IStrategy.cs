using SomeTrade.Strategies.Dto;
using System.Collections.Generic;

namespace SomeTrade.Strategies.Interfaces
{
    public interface IStrategy
    {
        CheckResult ShouldOpenLong();
        CheckResult ShouldOpenShort();
        CheckResult ShouldCloseLong();
        CheckResult ShouldCloseShort();
        TrailingStopResult UpdateTrailingStop(decimal entryPrice, int side, decimal quantity);
        bool ShouldStop(decimal entryPrice, int side);
        Dictionary<string,string> GetCalculations();
    }
}
