using OxyPlot.Axes;
using OxyPlot.ImageSharp;
using OxyPlot.Series;
using OxyPlot;
using System.Collections.Generic;
using System;
using System.IO;
using SomeTrade.TA.Dto;
using System.Linq;

namespace SomeTrade.TA.Harmonics
{
	public class Drawing
	{
		public void Draw(HarmonicItem harmonicItem,
			List<CandleDto> candles, string outputPath, string symbol, string name)
		{
			var plotModel = new PlotModel
			{
				Title = $"{symbol}-{name}-{harmonicItem.X.Date}",
				Background = OxyColors.White
			};
			string subtitle = "";

			var candleSeries = new CandleStickSeries
			{
				IncreasingColor = OxyColors.White,
				DecreasingColor = OxyColors.Black,
			};

			//mumların tamamı yerine öncesi 10 ve sonrası 10 kullanalim
			var candleXIndex = candles.FindIndex(x => x.OpenTime == harmonicItem.X.Date);
			var candleDIndex = candles.FindIndex(x => x.OpenTime == harmonicItem.D.Date);

			var startCandleIndex = candleXIndex > 10 ? candleXIndex - 10 : 0;
			var candleTakeCount = candleDIndex - startCandleIndex + 10;
			foreach (var candle in candles.Skip(startCandleIndex).Take(candleTakeCount))
			{
				candleSeries.Items.Add(new HighLowItem
				{
					X = DateTimeAxis.ToDouble(candle.OpenTime),
					Open = (double)candle.OpenPrice,
					High = (double)candle.HighPrice,
					Low = (double)candle.LowPrice,
					Close = (double)candle.ClosePrice
				});
			}

			plotModel.Series.Add(candleSeries);
			plotModel.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, StringFormat = "HH:mm" });
			plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left });

			// Swing çizgisi için LineSeries
			var swingLine = new LineSeries
			{
				Color = OxyColors.Blue,
				StrokeThickness = 2
			};

			// Başlangıç ve bitiş noktalarını ekle
			swingLine.Points.Add(new DataPoint(DateTimeAxis.ToDouble(harmonicItem.X.Date), Convert.ToDouble(harmonicItem.X.Price)));
			swingLine.Points.Add(new DataPoint(DateTimeAxis.ToDouble(harmonicItem.A.Date), Convert.ToDouble(harmonicItem.A.Price)));

			if (harmonicItem.B != null)
			{
				swingLine.Points.Add(new DataPoint(DateTimeAxis.ToDouble(harmonicItem.B.Date), Convert.ToDouble(harmonicItem.B.Price)));
				subtitle += $"B: {harmonicItem.B.Percentage} ";
			}
			if (harmonicItem.C != null)
			{
				swingLine.Points.Add(new DataPoint(DateTimeAxis.ToDouble(harmonicItem.C.Date), Convert.ToDouble(harmonicItem.C.Price)));
				subtitle += $"C: {harmonicItem.C.Percentage} ";
			}
			if (harmonicItem.D != null)
			{
				swingLine.Points.Add(new DataPoint(DateTimeAxis.ToDouble(harmonicItem.D.Date), Convert.ToDouble(harmonicItem.D.Price)));
				subtitle += $"D: {harmonicItem.D.Percentage} ";
			}
			plotModel.Subtitle = subtitle;


			//A, B, D yi de çizelim
			plotModel.Series.Add(swingLine);

			var swingLine2 = new LineSeries
			{
				Color = OxyColors.Blue,
				StrokeThickness = 2
			};
			swingLine2.Points.Add(new DataPoint(DateTimeAxis.ToDouble(harmonicItem.X.Date), Convert.ToDouble(harmonicItem.X.Price)));
			if (harmonicItem.B != null)
				swingLine2.Points.Add(new DataPoint(DateTimeAxis.ToDouble(harmonicItem.B.Date), Convert.ToDouble(harmonicItem.B.Price)));

			if (harmonicItem.D != null)
				swingLine2.Points.Add(new DataPoint(DateTimeAxis.ToDouble(harmonicItem.D.Date), Convert.ToDouble(harmonicItem.D.Price)));

			plotModel.Series.Add(swingLine2);

			// Ekseni ekleyin
			plotModel.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, StringFormat = "HH:mm" });
			plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left });

			// JPG dosyasını kaydet
			using var stream = File.Create(outputPath);
			var pngExporter = new PngExporter(width: 1200, height: 400);
			pngExporter.Export(plotModel, stream);
		}
	}
}
