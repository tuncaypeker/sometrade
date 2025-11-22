using SomeTrade.Data;
using System.Collections.Generic;
using System.Linq;

namespace SomeTrade.Jobs.Quartz
{
    public class SymbolDataHelpers
    {
        SymbolPairData symbolPairData;
        public SymbolDataHelpers(SymbolPairData symbolPairData)
        {
            this.symbolPairData = symbolPairData;
        }

        public List<string> GenerateSymbols(List<Model.Symbol> symbolsFromDb, int exchangeId, int[] livetestSymbolPairIds)
        {
            var symbols = new List<string>();
            var symbolPairs = symbolPairData.GetBy(x => x.ExchangeId == exchangeId && livetestSymbolPairIds.Contains(x.Id));

            foreach (var symbolPair in symbolPairs)
            {
                var fromSymbol = symbolsFromDb.FirstOrDefault(x => x.Id == symbolPair.FromSymbolId);
                var toSymbol = symbolsFromDb.FirstOrDefault(x => x.Id == symbolPair.ToSymbolId);

                if (fromSymbol == null || toSymbol == null)
                    continue;

                symbols.Add($"{toSymbol.Short}{fromSymbol.Short}");
            }

            return symbols;
        }


    }
}
