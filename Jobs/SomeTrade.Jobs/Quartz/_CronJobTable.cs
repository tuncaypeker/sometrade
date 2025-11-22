namespace SomeTrade.Jobs.Quartz
{
    public static class CronExpressionHelper
    {
        /// <summary>
        ///https://www.freeformatter.com/cron-expression-generator-quartz.html#
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static string GetExpressionBySecond(int seconds)
        {
            switch (seconds)
            {
                //binance mum'u veremiyor bu yuzden 5.-10. sn de calısmayı deniyorum
                case 10: return "5/10 * * * * ?";//her 10 sn'nin 5. sn sinde
                case 20: return "5/20 * * * * ?";//her 20 sn'nin 5. sn sinde
                case 30: return "5/30 * * * * ?";//her 30 sn'nin 5. sn sinde
                case 45: return "5/45 * * * * ?";//her 45 sn'nin 5. sn sinde
                case 60: return "10 * * ? * * *";//her dk'nin 50. sn sinde
                case 120: return "10 */2 * ? * * *";
                case 180: return "10 */3 * ? * * *";
                case 300: return "10 */5 * ? * * *";
                case 600: return "10 */10 * ? * * *";
                case 900: return "10 */15 * ? * * *";
                case 1200: return "10 */20 * ? * * *";
                default: return "10 */20 * ? * * *";
            }
        }
    }
}
