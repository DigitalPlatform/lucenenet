﻿using Lucene.Net.Support;
using System;
using System.Globalization;

namespace Lucene.Net.QueryParsers.Flexible.Standard.Config
{
    /// <summary>
    /// LUCENENET specific enum for mimicking the Java DateFormat
    /// </summary>
    public enum DateFormat
    {
        FULL, LONG,
        MEDIUM, SHORT
    }

    /// <summary>
    /// This {@link Format} parses {@link Long} into date strings and vice-versa. It
    /// uses the given {@link DateFormat} to parse and format dates, but before, it
    /// converts {@link Long} to {@link Date} objects or vice-versa.
    /// </summary>
    public class NumberDateFormat : NumberFormat
    {
        //private static readonly long serialVersionUID = 964823936071308283L;

        // The .NET ticks representing January 1, 1970 0:00:00, also known as the "epoch".
        public const long EPOCH = 621355968000000000;

        private readonly string dateFormat;
        private readonly DateFormat dateStyle;
        private readonly DateFormat timeStyle;
        private readonly TimeZoneInfo timeZone;

        /**
         * Constructs a {@link NumberDateFormat} object using the given {@link DateFormat}.
         * 
         * @param dateFormat {@link DateFormat} used to parse and format dates
         */
        public NumberDateFormat(string dateFormat, CultureInfo locale)
            : base(locale)
        {
            this.dateFormat = dateFormat;
        }

        public NumberDateFormat(DateFormat dateStyle, DateFormat timeStyle, CultureInfo locale, TimeZoneInfo timeZone)
            : base(locale)
        {
            this.dateStyle = dateStyle;
            this.timeStyle = timeStyle;
            this.timeZone = timeZone;
        }

        public override string Format(double number)
        {
            return new DateTime(EPOCH).AddMilliseconds(number).ToString(GetDateFormat(), this.locale);
        }

        public override string Format(long number)
        {
            return new DateTime(EPOCH).AddMilliseconds(number).ToString(GetDateFormat(), this.locale);
        }

        public override object Parse(string source)
        {
            return (DateTime.Parse(source, this.locale) - new DateTime(EPOCH)).TotalMilliseconds;
        }

        public override string Format(object number)
        {
            return new DateTime(EPOCH).AddMilliseconds(Convert.ToInt64(number)).ToString(GetDateFormat(), this.locale);
        }

        internal string GetDateFormat()
        {
            if (dateFormat != null) return dateFormat;

            string datePattern = "", timePattern = "";

            switch (dateStyle)
            {
                case DateFormat.SHORT:
                    datePattern = locale.DateTimeFormat.ShortDatePattern;
                    break;
                case DateFormat.MEDIUM:
                    datePattern = locale.DateTimeFormat.LongDatePattern
                        .Replace("dddd,", "").Replace(", dddd", "") // Remove the day of the week
                        .Replace("MMMM", "MMM"); // Replace month with abbreviated month
                    break;
                case DateFormat.LONG:
                    datePattern = locale.DateTimeFormat.LongDatePattern
                        .Replace("dddd,", "").Replace(", dddd", ""); // Remove the day of the week
                    break;
                case DateFormat.FULL:
                    datePattern = locale.DateTimeFormat.LongDatePattern;
                    break;
            }

            switch (timeStyle)
            {
                case DateFormat.SHORT:
                    timePattern = locale.DateTimeFormat.ShortTimePattern;
                    break;
                case DateFormat.MEDIUM:
                    timePattern = locale.DateTimeFormat.LongTimePattern;
                    break;
                case DateFormat.LONG:
                    timePattern = locale.DateTimeFormat.LongTimePattern; // LUCENENET TODO: Time zone info not being added
                    break;
                case DateFormat.FULL:
                    timePattern = locale.DateTimeFormat.LongTimePattern; // LUCENENET TODO: Time zone info not being added, but Java doc is unclear on what the difference is between this and LONG
                    break;
            }

            return string.Concat(datePattern, " ", timePattern);
        }
    }
}