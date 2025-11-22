using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.ImageSharp;
using OxyPlot.Series;
using SomeTrade.TA.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SomeTrade.TA
{
	public class Drawing
	{
		public void Draw(string symbol, string interval, SwingResultDto swingResultDto, string outputPath, List<CandleDto> candles)
		{
			var plotModel = new PlotModel
			{
				Title = $"{symbol} - {interval}",
				Subtitle = swingResultDto.CurrentTrend == 1
					? $"${swingResultDto.SwingHigh.Price} - ${swingResultDto.SwingLow.Price}"
					: $"${swingResultDto.SwingLow.Price} - ${swingResultDto.SwingHigh.Price}",
				Background = OxyColors.White
			};

			var candleSeries = new CandleStickSeries
			{
				IncreasingColor = OxyColors.White,
				DecreasingColor = OxyColors.Black,
			};

			//kac veri gelirse gelsin, max 750 mum cizerim
			foreach (var candle in candles)
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
			foreach (var swingHistory in swingResultDto.SwingHistory)
			{
				swingLine.Points.Add(new DataPoint(DateTimeAxis.ToDouble(swingHistory.SwingPoint.Date)
					, Convert.ToDouble(swingHistory.SwingPoint.Price)));
			}

			plotModel.Series.Add(swingLine);

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
