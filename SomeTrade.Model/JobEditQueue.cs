using System;

namespace SomeTrade.Model
{
    public class JobEditQueue : Core.ModelBase
    {
        /// <summary>
        /// Alert
        /// Livetest
        /// Robot
        /// </summary>
        public string JobType { get; set; }

        /// <summary>
        /// Id of Alert, Livetest or robot
        /// </summary>
        public int JobId { get; set; }

        /// <summary>
        /// Upsert
        /// Delete
        /// </summary>
        public string JobAction { get; set; }

        /// <summary>
        /// Bu islem robotlara yansitildi mi
        /// </summary>
        public bool HasProcessed { get; set; }

        public DateTime Created { get; set; }
        public DateTime Processed { get; set; }
        public string Reason { get; set; }
    }
}
