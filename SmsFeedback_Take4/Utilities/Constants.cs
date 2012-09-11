using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmsFeedback_Take4.Utilities
{
    public class Constants
    {

        public const String NULL_CONVID_ERROR_MESSAGE = "NullConvId";
        public const String NULL_STRING = "null";
        public const String INVALID_CONVID_ERROR_MESSAGE = "InvalidConvId";
        public const String NO_CONVID_ERROR_MESSAGE = "noConvIdPassed";
        public const String NULL_STARRED_STATUS_ERROR_MESSAGE = "nullStarredStatus";
        public const String DAY_GRANULARITY = "day";
        public const String WEEK_GRANULARITY = "week";
        public const String MONTH_GRANULARITY = "month";
        public const string GLOBAL_SCOPE = "Global";
        public const string DEFAULT_CHART_STYLE = "area";
        public const string BARS_CHART_STYLE = "bars";
        public const string NUMBER_COLUMN_TYPE = "number";
        public const string STRING_COLUMN_TYPE = "string";
        public const string POS_ADD_EVENT = "posAdd";
        public const string NEG_ADD_EVENT = "negAdd";
        public const string POS_TO_NEG_EVENT = "posToNeg";
        public const string NEG_TO_POS_EVENT = "negToPos";
        public const string POS_REMOVE_EVENT = "posRemove";
        public const string NEG_REMOVE_EVENT = "negRemove";
    }
}