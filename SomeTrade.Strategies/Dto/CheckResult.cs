using System.Collections.Generic;

namespace SomeTrade.Strategies.Dto
{
    /// <summary>
    /// Stratejinin Kontrol sonrası gonderdigi model, 
    /// Kontrol sonrasi long'a girmesi gerekiyorsa ornek olarak true doner 
    /// PositionLogs altında da aslında gondermek istedigi bilgi amacli alanlari iletir
    /// </summary>
    public class CheckResult
    {
        public CheckResult()
        {
            
        }

        public CheckResult(bool result)
        {
            this.Result = result;
            this.PositionLogs = new Dictionary<string, object>();
        }

        public bool Result { get; set; }

        /// <summary>
        /// Indikator calculations and results
        /// </summary>
        public Dictionary<string,object> PositionLogs { get; set; }
    }
}
