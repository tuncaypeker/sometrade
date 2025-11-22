using System;

namespace SomeTrade.Candles.Common
{
    public static partial class TACore
    {
        internal static readonly GlobalsType Globals = new GlobalsType();

        static TACore()
        {
            RestoreCandleDefaultSettings(CandleSettingType.AllCandleSettings);
        }

        public static Compatibility GetCompatibility()
        {
            return Globals.compatibility;
        }

        public static long GetUnstablePeriod(FuncUnstId id)
        {
            return id >= FuncUnstId.FuncUnstAll ? 0 : Globals.unstablePeriod[(int)id];
        }

        public static RetCode RestoreCandleDefaultSettings(CandleSettingType settingType)
        {
            switch (settingType)
            {
                case CandleSettingType.BodyLong:
                    SetCandleSettings(CandleSettingType.BodyLong, RangeType.RealBody, 10, 1.0);
                    break;

                case CandleSettingType.BodyVeryLong:
                    SetCandleSettings(CandleSettingType.BodyVeryLong, RangeType.RealBody, 10, 3.0);
                    break;

                case CandleSettingType.BodyShort:
                    SetCandleSettings(CandleSettingType.BodyShort, RangeType.RealBody, 10, 1.0);
                    break;

                case CandleSettingType.BodyDoji:
                    SetCandleSettings(CandleSettingType.BodyDoji, RangeType.HighLow, 10, 0.1);
                    break;

                case CandleSettingType.ShadowLong:
                    SetCandleSettings(CandleSettingType.ShadowLong, RangeType.RealBody, 0, 1.0);
                    break;

                case CandleSettingType.ShadowVeryLong:
                    SetCandleSettings(CandleSettingType.ShadowVeryLong, RangeType.RealBody, 0, 2.0);
                    break;

                case CandleSettingType.ShadowShort:
                    SetCandleSettings(CandleSettingType.ShadowShort, RangeType.Shadows, 10, 1.0);
                    break;

                case CandleSettingType.ShadowVeryShort:
                    SetCandleSettings(CandleSettingType.ShadowVeryShort, RangeType.HighLow, 10, 0.1);
                    break;

                case CandleSettingType.Near:
                    SetCandleSettings(CandleSettingType.Near, RangeType.HighLow, 5, 0.2);
                    break;

                case CandleSettingType.Far:
                    SetCandleSettings(CandleSettingType.Far, RangeType.HighLow, 5, 0.6);
                    break;

                case CandleSettingType.Equal:
                    SetCandleSettings(CandleSettingType.Equal, RangeType.HighLow, 5, 0.05);
                    break;

                case CandleSettingType.AllCandleSettings:
                    SetCandleSettings(CandleSettingType.BodyLong, RangeType.RealBody, 10, 1.0);
                    SetCandleSettings(CandleSettingType.BodyVeryLong, RangeType.RealBody, 10, 3.0);
                    SetCandleSettings(CandleSettingType.BodyShort, RangeType.RealBody, 10, 1.0);
                    SetCandleSettings(CandleSettingType.BodyDoji, RangeType.HighLow, 10, 0.1);
                    SetCandleSettings(CandleSettingType.ShadowLong, RangeType.RealBody, 0, 1.0);
                    SetCandleSettings(CandleSettingType.ShadowVeryLong, RangeType.RealBody, 0, 2.0);
                    SetCandleSettings(CandleSettingType.ShadowShort, RangeType.Shadows, 10, 1.0);
                    SetCandleSettings(CandleSettingType.ShadowVeryShort, RangeType.HighLow, 10, 0.1);
                    SetCandleSettings(CandleSettingType.Near, RangeType.HighLow, 5, 0.2);
                    SetCandleSettings(CandleSettingType.Far, RangeType.HighLow, 5, 0.6);
                    SetCandleSettings(CandleSettingType.Equal, RangeType.HighLow, 5, 0.05);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(settingType), settingType, null);
            }
            return RetCode.Success;
        }

        public static RetCode SetCandleSettings(CandleSettingType settingType, RangeType rangeType, int avgPeriod, double factor)
        {
            if (settingType >= CandleSettingType.AllCandleSettings)
            {
                return RetCode.BadParam;
            }

            Globals.candleSettings[(int)settingType].settingType = settingType;
            Globals.candleSettings[(int)settingType].rangeType = rangeType;
            Globals.candleSettings[(int)settingType].avgPeriod = avgPeriod;
            Globals.candleSettings[(int)settingType].factor = factor;

            return RetCode.Success;
        }

        public static RetCode SetCompatibility(Compatibility value)
        {
            Globals.compatibility = value;
            return RetCode.Success;
        }

        public static RetCode SetUnstablePeriod(FuncUnstId id, long unstablePeriod)
        {
            if (id > FuncUnstId.FuncUnstAll)
            {
                return RetCode.BadParam;
            }

            if (id != FuncUnstId.FuncUnstAll)
            {
                Globals.unstablePeriod[(int)id] = unstablePeriod;
            }
            else
            {
                for (int i = 0; i < 23; i++)
                {
                    Globals.unstablePeriod[i] = unstablePeriod;
                }
            }

            return RetCode.Success;
        }

        internal sealed class CandleSetting
        {
            public int avgPeriod;
            public double factor;
            public RangeType rangeType;
            public CandleSettingType settingType;
        }

        public enum Compatibility
        {
            Default,
            Metastock
        }

        public enum FuncUnstId
        {
            Adx = 0,
            Adxr = 1,
            Atr = 2,
            Cmo = 3,
            Dx = 4,
            Ema = 5,
            FuncUnstAll = 23,
            FuncUnstNone = -1,
            HtDcPeriod = 6,
            HtDcPhase = 7,
            HtPhasor = 8,
            HtSine = 9,
            HtTrendline = 10,
            HtTrendMode = 11,
            Kama = 12,
            Mama = 13,
            Mfi = 14,
            MinusDI = 15,
            MinusDM = 16,
            Natr = 17,
            PlusDI = 18,
            PlusDM = 19,
            Rsi = 20,
            StochRsi = 21,
            T3 = 22
        }

        internal sealed class GlobalsType
        {
            public CandleSetting[] candleSettings;
            public Compatibility compatibility = Compatibility.Default;
            public long[] unstablePeriod = new long[23];

            public GlobalsType()
            {
                for (int i = 0; i < 23; i++)
                {
                    unstablePeriod[i] = 0;
                }

                candleSettings = new CandleSetting[11];

                for (int j = 0; j < candleSettings.Length; j++)
                {
                    candleSettings[j] = new CandleSetting();
                }
            }
        }

        internal class MoneyFlow
        {
            public double negative;
            public double positive;
        }
    }
}
