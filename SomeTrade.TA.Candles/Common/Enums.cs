namespace SomeTrade.Candles.Common
{
    public enum RangeType
    {
        RealBody,
        HighLow,
        Shadows
    }

    public enum RetCode
    {
        AllocErr = 3,
        BadObject = 15,
        BadParam = 2,
        FuncNotFound = 5,
        GroupNotFound = 4,
        InputNotAllInitialize = 10,
        InternalError = 5000,
        InvalidHandle = 6,
        InvalidListType = 14,
        InvalidParamFunction = 9,
        InvalidParamHolder = 7,
        InvalidParamHolderType = 8,
        LibNotInitialize = 1,
        NotSupported = 16,
        OutOfRangeEndIndex = 13,
        OutOfRangeStartIndex = 12,
        OutputNotAllInitialize = 11,
        Success = 0,
        UnknownErr = 65535
    }

     public enum CandleSettingType
    {
        BodyLong,
        BodyVeryLong,
        BodyShort,
        BodyDoji,
        ShadowLong,
        ShadowVeryLong,
        ShadowShort,
        ShadowVeryShort,
        Near,
        Far,
        Equal,
        AllCandleSettings
    }
}
