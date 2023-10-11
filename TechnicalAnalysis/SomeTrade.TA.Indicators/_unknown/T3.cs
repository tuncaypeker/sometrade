namespace SomeTrade.TA.Indicators
{
    using CuttingEdge.Conditions;
    using SomeTrade.TA.Indicators.Dto;
    using System.Linq;

    /// <summary>
    /// description:
    ///     Yatay piyasalarda etkili degildir
    ///     TMA veya T3 olarak bilinir
    ///     Form�lasyonda EMA ve DEMA'lar kullan�l�r 
    ///     Hareketli ortalaman�n gecikmesini engellemeye �al���r
    ///  parameters:
    ///     period: bar sayisi
    ///     vFactor: hacim olarak d���n�lmemeli, 0 yap�nca �ssel'in 3.derecesini al�r, 1 yapinca �ift �ssel'in 3. derecesini al�r gibi 
    ///     d���n�lebilir, standart degeri 0.7'dir ve �ift �ssel'e biraz daha yak�nd�r
    ///  strateji:
    ///     1: Fiyat T3� yukar� keserse AL, Fiyat T3� a�a�� keserse sat
    ///     2: T3 K�rm�z�dan Ye�ile d�nerse al, Ye�ilden k�rm�z�ya d�nerse sat
    ///     3: �ki farkl� parametreli T3 ekleyip, k�sa T3, uzunu yukar� keserse al, a�a�� keserse sat
    ///     4: Destek, Diren� olarak kullan�labilir
    ///  �rnek backtest:
    ///     1: period:8, vFactor: 0.7, piyasa:BIST 100, zaman: 2 Y�l periyot: g�nl�k, getiri:36%
    ///  pinescript:
    ///     //@version=3
    ///     //Copyright (c) 2018-present, Alex Orekhov (everget)
    ///     //T3 script may be freely distributed under the MIT license.
    ///     study(title="T3", shorttitle="T3", overlay=true)
    /// </summary>
    public class T3
    {
        public static SuperTrendResultDto Calculate(double[] inReal, int period, double vFactor)
        {
            Condition.Requires(inReal, "inHigh").IsNotNull().IsNotEmpty();
            Condition.Requires(vFactor, "vFactor").IsGreaterThan(0.0).IsLessThan(1.0);

            int i;
            int startIdx = 0;
            int endIdx = inReal.Length - 1;

            var result = new SuperTrendResultDto()
            {
                Color = new string[inReal.Length],
                Result = new double[inReal.Length]
            };

            int lookbackTotal = (period - 1) * 6;
            if (startIdx <= lookbackTotal)
                startIdx = lookbackTotal;

            Condition.Requires(startIdx, "startIdx").IsLessThan(endIdx);

            int today = startIdx - lookbackTotal;
            double k = 2.0 / (period + 1.0);
            double one_minus_k = 1.0 - k;
            double tempReal = inReal[today];
            today++;
            for (i = period - 1; i > 0; i--)
            {
                tempReal += inReal[today];
                today++;
            }

            double e1 = tempReal / period;
            tempReal = e1;
            for (i = period - 1; i > 0; i--)
            {
                e1 = k * inReal[today] + one_minus_k * e1;
                today++;
                tempReal += e1;
            }

            double e2 = tempReal / period;
            tempReal = e2;
            for (i = period - 1; i > 0; i--)
            {
                e1 = k * inReal[today] + one_minus_k * e1;
                today++;
                e2 = k * e1 + one_minus_k * e2;
                tempReal += e2;
            }

            double e3 = tempReal / period;
            tempReal = e3;
            for (i = period - 1; i > 0; i--)
            {
                e1 = k * inReal[today] + one_minus_k * e1;
                today++;
                e2 = k * e1 + one_minus_k * e2;
                e3 = k * e2 + one_minus_k * e3;
                tempReal += e3;
            }

            double e4 = tempReal / period;
            tempReal = e4;
            for (i = period - 1; i > 0; i--)
            {
                e1 = k * inReal[today] + one_minus_k * e1;
                today++;
                e2 = k * e1 + one_minus_k * e2;
                e3 = k * e2 + one_minus_k * e3;
                e4 = k * e3 + one_minus_k * e4;
                tempReal += e4;
            }

            double e5 = tempReal / period;
            tempReal = e5;
            for (i = period - 1; i > 0; i--)
            {
                e1 = k * inReal[today] + one_minus_k * e1;
                today++;
                e2 = k * e1 + one_minus_k * e2;
                e3 = k * e2 + one_minus_k * e3;
                e4 = k * e3 + one_minus_k * e4;
                e5 = k * e4 + one_minus_k * e5;
                tempReal += e5;
            }

            double e6 = tempReal / period;
            while (true)
            {
                if (today > startIdx)
                {
                    break;
                }

                e1 = k * inReal[today] + one_minus_k * e1;
                today++;
                e2 = k * e1 + one_minus_k * e2;
                e3 = k * e2 + one_minus_k * e3;
                e4 = k * e3 + one_minus_k * e4;
                e5 = k * e4 + one_minus_k * e5;
                e6 = k * e5 + one_minus_k * e6;
            }

            tempReal = vFactor * vFactor;
            double c1 = -(tempReal * vFactor);
            double c2 = 3.0 * (tempReal - c1);
            double c3 = -6.0 * tempReal - 3.0 * (vFactor - c1);
            double c4 = 1.0 + 3.0 * vFactor - c1 + 3.0 * tempReal;
            int outIdx = startIdx;

            result.Result[outIdx] = c1 * e6 + c2 * e5 + c3 * e4 + c4 * e3;

            outIdx++;

            while (true)
            {
                if (today > endIdx)
                    break;

                e1 = k * inReal[today] + one_minus_k * e1;
                today++;
                e2 = k * e1 + one_minus_k * e2;
                e3 = k * e2 + one_minus_k * e3;
                e4 = k * e3 + one_minus_k * e4;
                e5 = k * e4 + one_minus_k * e5;
                e6 = k * e5 + one_minus_k * e6;
                result.Result[outIdx] = c1 * e6 + c2 * e5 + c3 * e4 + c4 * e3;
                result.Color[outIdx] = result.Result[outIdx] > result.Result[outIdx - 1]
                    ? "green"
                    : result.Result[outIdx] < result.Result[outIdx - 1]
                        ? "red"
                        : "yellow";

                outIdx++;
            }

            return result;
        }

        public static SuperTrendOneResultDto Last(double[] inReal, int period, double vFactor)
        {
            var result = Calculate(inReal, period, vFactor);
            return new SuperTrendOneResultDto()
            {
                Color = result.Color.Last(),
                Result = result.Result.Last()
            };
        }
        public static SuperTrendOneResultDto Prev(double[] inReal, int period, double vFactor)
        {
            var result = Calculate(inReal, period, vFactor);
            return new SuperTrendOneResultDto()
            {
                Color = result.Color.TakeLast(2).First(),
                Result = result.Result.TakeLast(2).First()
            };
        }
    }
}
