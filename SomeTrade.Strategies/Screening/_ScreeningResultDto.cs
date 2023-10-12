using System.Collections.Generic;

namespace SomeTrade.Strategies.Screening
{
    public class ScreeningResultDto
    {
        public ScreeningResultDto(bool hasAlert, string message, Dictionary<string,object> values)
        {
            HasAlert = hasAlert;
            Message = message;
            Values = values;
        }

        public bool HasAlert { get; set; }
        public string Message { get; set; }
        public Dictionary<string, object> Values { get; set; }
    }
}
