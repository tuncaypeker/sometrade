namespace SomeTrade.TA
{
    using CuttingEdge.Conditions;
    using System;

    /// <summary>
    /// Parabolic SAR
    /// </summary>
    public class SAR
	{
		/// <summary>
		/// //@version=5
		/// indicator("ta.sar")
		/// plot(ta.sar(0.02, 0.02, 0.2), style=plot.style_cross, linewidth=3)
		/// 
		/// // The same on Pine Script™
		/// pine_sar(start, inc, max) =>
		/// 	var float result = na
		/// 	var float maxMin = na
		/// 	var float acceleration = na
		/// 	var bool isBelow = na
		/// 	bool isFirstTrendBar = false
		/// 
		/// 	if bar_index == 1
		/// 		if close > close[1]
		/// 			isBelow := true
		/// 			maxMin := high
		/// 			result := low[1]
		/// 		else
		/// 			isBelow := false
		/// 			maxMin := low
		/// 			result := high[1]
		/// 		isFirstTrendBar := true
		/// 		acceleration := start
		/// 
		/// 	result := result + acceleration * (maxMin - result)
		/// 
		/// 	if isBelow
		/// 		if result > low
		/// 			isFirstTrendBar := true
		/// 			isBelow := false
		/// 			result := math.max(high, maxMin)
		/// 			maxMin := low
		/// 			acceleration := start
		/// 	else
		/// 		if result < high
		/// 			isFirstTrendBar := true
		/// 			isBelow := true
		/// 			result := math.min(low, maxMin)
		/// 			maxMin := high
		/// 			acceleration := start
		/// 
		/// 	if not isFirstTrendBar
		/// 		if isBelow
		/// 			if high > maxMin
		/// 				maxMin := high
		/// 				acceleration := math.min(acceleration + inc, max)
		/// 		else
		/// 			if low < maxMin
		/// 				maxMin := low
		/// 				acceleration := math.min(acceleration + inc, max)
		/// 
		/// 	if isBelow
		/// 		result := math.min(result, low[1])
		/// 		if bar_index > 1
		/// 			result := math.min(result, low[2])
		/// 
		/// 	else
		/// 		result := math.max(result, high[1])
		/// 		if bar_index > 1
		/// 			result := math.max(result, high[2])
		/// 
		/// 	result
		/// 
		/// plot(pine_sar(0.02, 0.02, 0.2), style=plot.style_cross, linewidth=3)
		/// </summary>
		/// <param name="inReal"></param>
		/// <param name="period"></param>
		/// <returns></returns>
		public static double[] Calculate(double[] close, double[] high, double[] low,double start, double inc, double max)
		{
			Condition.Requires(close, "close").IsNotEmpty();
			Condition.Requires(start, "start").IsGreaterThan(0);

			double maxMin = 0;
			double acceleration = 0;
			bool isBelow = false;
			bool isFirstTrendBar = false;

			double[] result = new double[close.Length];

			for (int i = 1; i < close.Length; i++)
			{
				if (i == 1)
				{
					isFirstTrendBar = true;
					acceleration = start;
				}

				if (close[i] > close[i - 1])
					{
						isBelow = true;
						maxMin = high[i];
						result[i] = low[i - 1];
					}
					else
					{
						isBelow = false;
						maxMin = low[i];
						result[i] = high[i-1];
					}

				result[i] = result[i] + acceleration * (maxMin - result[i]);
				if (isBelow)
				{
					if (result[i] > low[i])
					{
						isFirstTrendBar = true;
						isBelow = false;
						result[i] = System.Math.Max(high[i], maxMin);
						maxMin = low[i];
						acceleration = start;
					}
				}
				else
				{
					if (result[i] < high[i])
					{
						isFirstTrendBar = true;
						isBelow = true;
						result[i] = System.Math.Min(low[i], maxMin);
						maxMin = high[i];
						acceleration = start;
					}
				}

				if (!isFirstTrendBar)
				{
					if (isBelow)
					{
						if (high[i] > maxMin)
						{
							maxMin = high[i];
							acceleration = System.Math.Min(acceleration + inc, max);
						}
					}
					else {
						if (low[i] < maxMin)
						{
							maxMin= low[i];
							acceleration = System.Math.Min(acceleration + inc, max);
						}
					}
				}

				if (isBelow)
				{
					result[i] = System.Math.Min(result[i], low[i - 1]);
					if (i > 1)
						result[i] = System.Math.Min(result[i], low[i - 2]);
				}
				else
				{
					result[i] = System.Math.Max(result[i], high[i-1]);
					if (i > 1)
						result[i] = System.Math.Max(result[i], high[i-2]);
				}

				//result[i] = maxMin;
			}

			return result;
		}
	}
}
