using SomeTrade.Strategies.Dto;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SomeTrade.Strategies.Interfaces
{
    public abstract class ScreeningBase : IScreening
    {
        readonly List<Candle> _candles;
        readonly Dictionary<string, object> _parameters;

        protected double[] highArray { get { return _candles.Select(x => x.High).ToArray(); } }
        protected double[] lowArray { get { return _candles.Select(x => x.Low).ToArray(); } }
        protected double[] closeArray { get { return _candles.Select(x => x.Close).ToArray(); } }
        protected double[] openArray { get { return _candles.Select(x => x.Open).ToArray(); } }
        protected double[] volumeArray { get { return _candles.Select(x => x.Volume).ToArray(); } }

        readonly string _nameInDb;
        readonly int _idInDb;

        public ScreeningBase(List<Candle> candles, Dictionary<string, object> parameters, string nameInDb, int idInDb)
        {
            _candles = candles;
            _parameters = parameters;
            _nameInDb = nameInDb;
            _idInDb = idInDb;
        }

        public abstract Screening.ScreeningResultDto Execute();

        //# helper methods
        protected int GetParameterAsInt(string name, int defValue = -1)
        {
            var parameter = GetParameter(name);
            return parameter == null
                ? defValue
                : int.Parse(parameter);
        }

        protected double GetParameterAsDouble(string name, double defValue = -1)
        {
            var parameter = GetParameter(name);
            if (parameter == null)
                return defValue;

            parameter = parameter.Replace(',', '.');
            double value;
            double.TryParse(parameter, NumberStyles.Any, CultureInfo.InvariantCulture, out value);

            return value;
        }

        protected bool GetParameterAsBoolean(string name, bool defValue = false)
        {
            var parameter = GetParameter(name);
            return parameter == null
                ? defValue
                : bool.Parse(parameter);
        }

        public string GetParameter(string name) => _parameters.ContainsKey(name) ? _parameters[name].ToString() : null;
    }
}
