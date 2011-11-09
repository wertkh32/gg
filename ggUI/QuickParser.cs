using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace gg
{
    /// <summary>
    /// Parses input for the quick add function
    /// </summary>
    class QuickParser
    {

        public static GGItem Parse(string input)
        {
            if (IsInvalidInput(input))
            {
                throw new Exception("Invalid input passed to QuickParser");
            }

            string description = string.Empty;
            string tag = string.Empty;
            DateTime endDate = GGItem.DEFAULT_ENDDATE;
            GGItem result = null;
            if (InputContainsQuotes(input))
            {
                string restOfInput = string.Empty;
                description = GetDescriptionFromWithinQuotes(input, ref restOfInput);
                if (restOfInput == null || restOfInput.Trim().Equals(string.Empty))
                {
                    result = new GGItem(description);
                    return result;
                }
                GetDateAndRestOfInputFromInput(restOfInput, ref tag, ref endDate);
                tag = GetTagIfTagExist(restOfInput);
                result = new GGItem(description, endDate, tag);
                return result;
            }
            description = input;
            GetDateAndRestOfInputFromInput(input, ref description, ref endDate);
            GetDescriptionAndTagFromInput(input, ref description, ref tag);
            description = RemoveTrailingPrepositionsFromDescription(description);
            result = new GGItem(description, endDate, tag);
            return result;
        }

        private static string GetDescriptionFromWithinQuotes(string input, ref string restOfInput)
        {
            Match quotesMatch = Regex.Match(input, "(\"|\')(?<desc>[^\"\']+)(\"|\')");
            string description = quotesMatch.Groups["desc"].Value;
            restOfInput = Regex.Replace(input, "(\"|\')" + description + "(\"|\')", "").Trim();
            return description;
        }

        private static bool InputContainsQuotes(string input)
        {
            return Regex.IsMatch(input, "(\"|\')[^\"\']+(\"|\')");
        }

        private static string GetTagIfTagExist(string restOfInput)
        {
            Match tagMatch = null;
            if (MatchesTag(restOfInput, out tagMatch))
            {
                return GetTagFromMatch(tagMatch);
            }
            else
                return "misc";
        }

        private static void GetDescriptionAndTagFromInput(string input, ref string description, ref string tag)
        {
            Match tagMatch = null;
            if (MatchesTag(input, out tagMatch))
            {
                description = RemoveTagFromDescription(description);
                tag = GetTagFromMatch(tagMatch);
            }
            else
                tag = "misc";
        }

        private static void GetDateAndRestOfInputFromInput(string input, ref string description, ref DateTime endDate)
        {
            DateTimeParser dateTimeParser = new DateTimeParser(input);
            endDate = dateTimeParser.GetDateTimeResult();
            if (endDate != GGItem.DEFAULT_ENDDATE)
            {
                description = dateTimeParser.GetRestOfInput();
            }
        }

        private static bool IsInvalidInput(string input)
        {
            return input == null || input.Equals(string.Empty);
        }

        private static string RemoveTagFromDescription(string input)
        {
            string tagPattern = @"(\s+|^)#\s*(\w+)(\s+|$)";
            string result = Regex.Replace(input, tagPattern, " ");
            return result;
        }

        private static string GetTagFromMatch(Match tagMatch)
        {
            Group tag = tagMatch.Groups["tag"];
            return tag.ToString();
        }

        private static bool MatchesTag(string input, out Match tagMatch)
        {
            string tagPattern = @"\s+#\s*(?<tag>\w+)\b";
            tagMatch = Regex.Match(input, tagPattern);
            return tagMatch.Success;
        }

        private static string RemoveTrailingPrepositionsFromDescription(string description)
        {
            if (!IsInvalidInput(description))
                return Regex.Replace(description, @"\s+(@|at|by|on)(\s+)?$", "").Trim();
            return description.Trim();
        }
    }
}
