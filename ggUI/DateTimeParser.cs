using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace gg
{
    class DateTimeParser
    {
        private DateTime dateTimeResult;
        private string restOfInput;
        Boolean containsTime = false;
        static CultureInfo usCulture = CultureInfo.CreateSpecificCulture("en-US");

        public DateTimeParser(string input)
        {
            dateTimeResult = GGItem.DEFAULT_ENDDATE;
            restOfInput = input;
            if (IsInvalidInput(input))
            {
                throw new Exception("Invalid Input passed to DateTimeParser");
            }
            else
            {
                string output = string.Empty;
                output = ConvertInputToTimeStampIfMatches(input);
                output = ConvertInputToDateStampIfMatches(input, output);
                dateTimeResult = ConvertStringOutputIntoDateTime(output);
            }
        }

        private bool IsInvalidInput(string input)
        {
            return input == null || input.Equals(string.Empty);
        }

        private string ConvertInputToTimeStampIfMatches(string input)
        {
            Match matchedTime = null;
            string output = string.Empty;
            if (MatchesTwelveHourTime(input, out matchedTime))
                output = DelimitHoursAndMinutes(matchedTime);
            else if (MatchesTwentyFourHourTime(input, out matchedTime))
                output = DelimitHoursAndMinutes(matchedTime);
            if (!output.Equals(string.Empty))
                containsTime = true;
            return output;
        }

        private string ConvertInputToDateStampIfMatches(string input, string output)
        {
            Match matchedDate = null;
            string dateStamp = string.Empty;
            int month = 0;
            int dateInMonth = 0;
            if (MatchesTomorrow(input))
                output = ConvertTomorrowToDate() + " " + output;
            else if (MatchesToday(input))
                output = ConvertTodayToDate() + " " + output;
            else if (MatchesDateStamp(input, out matchedDate))
                output = ConvertMatchedDateToProperDateStamp(matchedDate) + " " + output;
            else if (MatchesWeekday(input, out dateStamp))
                output = dateStamp + " " + output;
            else if (MatchesDateWithMonthNames(input, out month, out dateInMonth))
                output = ConvertDateAndMonthToProperDateStamp(dateInMonth, month) + " " + output;
            return output;
        }

        private DateTime ConvertStringOutputIntoDateTime(string output)
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            Debug.WriteLine("System culture: " + currentCulture.DisplayName);
            Debug.WriteLine("DateParser.ConvertStringOutputIntoDateTime: Got " + output);
            CultureInfo provider = new CultureInfo("en-US");
            DateTime result = DateTime.MinValue;
            DateTime.TryParse(output, provider, System.Globalization.DateTimeStyles.None, out result);
            Boolean containsDateOnly = !containsTime && result != DateTime.MinValue;
            if (containsDateOnly)
                result = result.AddHours(23.9833333333);
            Debug.WriteLine("DateParser.ConvertStringOutputIntoDateTime: Converted to " + result.ToString());
            return result;
        }

        private static string ConvertDateAndMonthToProperDateStamp(int date, int month)
        {
            Debug.Assert(month > 0 && month <= 12);
            Debug.Assert(date > 0 && date <= 32);
            string usDateFormat = month + "/" + date + "/" + DateTime.Now.Year;
            return usDateFormat;
        }

        private bool MatchesDateWithMonthNames(string input, out int month, out int date)
        {
            Dictionary<string, int> mapMonthPatternsToIntegers = GenerateMonthPatternToIntegerMapping();

            foreach (KeyValuePair<string, int> monthPatternToInteger in mapMonthPatternsToIntegers)
            {
                Match match = Regex.Match(input, monthPatternToInteger.Key, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    restOfInput = Regex.Replace(restOfInput, @"(\d\d?\s+)?" + match.ToString().Trim() + @"(\s+\d\d?)?", "").Trim();
                    month = monthPatternToInteger.Value;
                    date = GetDateFromInputWithMonthName(input, match);
                    return true;
                }
            }
            month = 0;
            date = 0;
            return false;
        }

        private static int GetDateFromInputWithMonthName(string input, Match match)
        {
            int date;
            string month = match.ToString().Trim();
            Match dateMatch = Regex.Match(input, @"((?<date>\d\d?\s+)" + month + "|" + month +  @"(?<date>\s+\d\d?))");
            if (dateMatch.Success)
                date = int.Parse(dateMatch.Groups["date"].Value);
            else
                date = 1;
            return date;
        }

        private static Dictionary<string, int> GenerateMonthPatternToIntegerMapping()
        {
            Dictionary<string, int> mapMonthPatternsToIntegers = new Dictionary<string, int>();
            mapMonthPatternsToIntegers.Add(@"(^|\s+)jan\.?(uary)?($|\s+)", 1);
            mapMonthPatternsToIntegers.Add(@"(^|\s+)feb\.?(ruary)?($|\s+)", 2);
            mapMonthPatternsToIntegers.Add(@"(^|\s+)mar\.?(ch)?($|\s+)", 3);
            mapMonthPatternsToIntegers.Add(@"(^|\s+)apr\.?(il)?($|\s+)", 4);
            mapMonthPatternsToIntegers.Add(@"(^|\s+)may", 5);
            mapMonthPatternsToIntegers.Add(@"(^|\s+)jun\.?e?($|\s+)", 6);
            mapMonthPatternsToIntegers.Add(@"(^|\s+)jul\.?y?($|\s+)", 7);
            mapMonthPatternsToIntegers.Add(@"(^|\s+)aug\.?(ust)?($|\s+)", 8);
            mapMonthPatternsToIntegers.Add(@"(^|\s+)sep\.?(t\.?|tember)?($|\s+)", 9);
            mapMonthPatternsToIntegers.Add(@"(^|\s+)oct\.?(ober)?($|\s+)", 10);
            mapMonthPatternsToIntegers.Add(@"(^|\s+)nov\.?(ember)?($|\s+)", 11);
            mapMonthPatternsToIntegers.Add(@"(^|\s+)dec\.?(ember)?($|\s+)", 12);
            return mapMonthPatternsToIntegers;
        }

        private bool MatchesWeekday(string input, out string dateStamp)
        {
            Dictionary<string, DateTime> mapDayPatternToDate = GenerateDayPatternToDateMapping();

            foreach (KeyValuePair<string, DateTime> dayPatternToDateMap in mapDayPatternToDate)
            {
                Match match = Regex.Match(input, dayPatternToDateMap.Key, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    restOfInput = Regex.Replace(restOfInput, @"(this)?\s+" + match.ToString().Trim(), "", RegexOptions.IgnoreCase).Trim();
                    dateStamp = ConvertToUsDateStampIfNotUsCulture(dayPatternToDateMap.Value);
                    return true;
                }
            }
            dateStamp = string.Empty;
            return false;
        }

        private static string ConvertToUsDateStampIfNotUsCulture(DateTime otherCultureDate)
        {
            string dateStamp;
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            if (currentCulture != usCulture)
                dateStamp = otherCultureDate.Month + "/" + otherCultureDate.Day + "/" + otherCultureDate.Year;
            else
                dateStamp = otherCultureDate.ToShortDateString().ToString();
            return dateStamp;
        }

        private static Dictionary<string, DateTime> GenerateDayPatternToDateMapping()
        {
            Dictionary<string, DateTime> mapDayPatternToDate = new Dictionary<string, DateTime>();
            mapDayPatternToDate.Add(@"(^|\s+)m[ou]n(day)?($|\s+)", ConvertDayToDateTime(DayOfWeek.Monday));
            mapDayPatternToDate.Add(@"(^|\s+)t(ue|eu|oo|u|)s?(day)?($|\s+)", ConvertDayToDateTime(DayOfWeek.Tuesday));
            mapDayPatternToDate.Add(@"(^|\s+)we(d|dnes|nds|nns)(day)?($|\s+)", ConvertDayToDateTime(DayOfWeek.Wednesday));
            mapDayPatternToDate.Add(@"(^|\s+)th(u(?:rs)?|ers)(day)?($|\s+)", ConvertDayToDateTime(DayOfWeek.Thursday));
            mapDayPatternToDate.Add(@"(^|\s+)fr[iy](day)?($|\s+)", ConvertDayToDateTime(DayOfWeek.Friday));
            mapDayPatternToDate.Add(@"(^|\s+)sat(t?[ue]rday)?($|\s+)", ConvertDayToDateTime(DayOfWeek.Saturday));
            mapDayPatternToDate.Add(@"(^|\s+)su[nm](day)?($|\s+)", ConvertDayToDateTime(DayOfWeek.Sunday));
            return mapDayPatternToDate;
        }

        private static DateTime ConvertDayToDateTime(DayOfWeek dayOfWeek)
        {
            int day = (int)dayOfWeek;
            Debug.Assert(day >= 0 && day < 7);
            int today = (int)DateTime.Now.DayOfWeek;
            Boolean weekdayIsBeforeToday = day < today;
            if (weekdayIsBeforeToday)
            {
                day = day + 7;
            }
            int daysToAdd = day - today;
            return DateTime.Now.AddDays(daysToAdd);
        }

        private static string ConvertMatchedDateToProperDateStamp(Match matchedDate)
        {
            string[] dateArray = Regex.Split(matchedDate.ToString(), "[-/]");
            string usDate = string.Empty;
            Debug.Assert(dateArray.Length <= 3);
            Boolean hasYear = dateArray.Length == 3;
            if (dateArray.Length < 3)
                usDate = dateArray[1] + "/" + dateArray[0] + "/" + DateTime.Now.Year.ToString();
            else
                usDate = dateArray[1] + "/" + dateArray[0] + "/" + dateArray[2];
            return usDate;
        }

        private bool MatchesDateStamp(string input, out Match matchedDate)
        {
            string dateStampPattern = @"[0-9]{1,2}[-/][0-9]{1,2}([-/][0-9]{1,4})?";
            matchedDate = Regex.Match(input, dateStampPattern);
            if (matchedDate.Success)
            {
                restOfInput = restOfInput.Replace(matchedDate.ToString(), "").Trim();
                return true;
            }
            return false;
        }

        private static string ConvertTodayToDate()
        {
            DateTime result = DateTime.Now.Date;
            return ConvertToUsDateStampIfNotUsCulture(result);
        }

        private bool MatchesToday(string input)
        {
            string todayOrTomorrowPattern = @"(to?da?y)";
            Match matchedDate = Regex.Match(input, todayOrTomorrowPattern, RegexOptions.IgnoreCase);
            if (matchedDate.Success)
            {
                restOfInput = restOfInput.Replace(matchedDate.ToString(), "").Trim();
                return true;
            }
            else
                return false;
        }

        private static string DelimitHoursAndMinutes(Match matchedTime)
        {
            string time = matchedTime.ToString().Trim();
            int temp;
            Boolean isTwentyFourHour = int.TryParse(time, out temp);
            if (isTwentyFourHour)
            {
                return time.Substring(0, 2) + ":" + time.Substring(2, 2);
            }
            else
            {
                Match pmAmMatch = Regex.Match(time, @"\d+", RegexOptions.IgnoreCase);
                if (pmAmMatch.Value.Length == 4)
                    return time.Substring(0, 2) + ":" + time.Substring(2);
                else if (pmAmMatch.Value.Length == 3)
                    return time.Substring(0, 1) + ":" + time.Substring(1);
                else
                    return time;
            }
        }

        private static string ConvertTomorrowToDate()
        {
            DateTime result = DateTime.Now.AddDays(1).Date;
            return ConvertToUsDateStampIfNotUsCulture(result);
        }

        private bool MatchesTomorrow(string input)
        {
            string todayOrTomorrowPattern = @"(to?mo?r?(r|l)(ow)?)";
            Match matchedDate = Regex.Match(input, todayOrTomorrowPattern, RegexOptions.IgnoreCase);
            if (matchedDate.Success)
            {
                restOfInput = restOfInput.Replace(matchedDate.ToString(), "").Trim();
                return true;
            }
            else
                return false;
        }

        private bool MatchesTwentyFourHourTime(string input, out Match matchedTime)
        {
            string twentyFourHourTimePattern = @"(\s+|^)[0-2]\d[0-5]\d\b(\s+|$)";
            matchedTime = Regex.Match(input, twentyFourHourTimePattern, RegexOptions.IgnoreCase); ;
            if (matchedTime.Success)
            {
                restOfInput = restOfInput.Replace(matchedTime.ToString(), "").Trim();
                return true;
            }
            else
                return false;
        }

        private bool MatchesTwelveHourTime(string input, out Match matchedTime)
        {
            string twelveHourTimePattern = @"\b[\d]{1,4}\s*(p|a)m\b";
            matchedTime = Regex.Match(input, twelveHourTimePattern, RegexOptions.IgnoreCase);
            if (matchedTime.Success)
            {
                restOfInput = restOfInput.Replace(matchedTime.ToString(), "").Trim();
                return true;
            }
            else
                return false;
        }

        public DateTime GetDateTimeResult()
        {
            return dateTimeResult;
        }

        public string GetRestOfInput()
        {
            return restOfInput;
        }
    }
}
