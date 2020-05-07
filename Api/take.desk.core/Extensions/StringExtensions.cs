using Lime.Messaging.Contents;
using Lime.Protocol;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace take.desk.core.Extensions
{
    public static class StringExtensions
    {
        private const string SMALLTALKS_CLEAN_PATTERN = @"(?i)\b(oi|ol(á|Á|a)|tchau|xau|at(e|é)\smais|adeus|f(alou+|lw+)(s)*|\s(é|É|eh|e|o|a)\s|meu|nome|aqui|me|tudo|todo|bem|chamo|(bo(m|a)\s(dia|tarde|noite))|eu|sou)\b";
        private const string SMALLTALKS_PUNCTUATION_PATTERN = @"[.,\/#!?$%\^&\*;:{}=\-_`~()]";
        private const string WHITE_SPACE = " ";

        /// <summary>
        /// Extension that removes the given RegEx pattern from the input.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static string RemoveRegexPattern(this string input, Regex pattern)
        {
            return Regex.Replace(input, pattern.ToString(), string.Empty).Trim();
        }

        /// <summary>
        ///  Extension that capitalizes each word from given input.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Capitalize(this string input)
        {
            try
            {
                var response = new StringBuilder();
                input = input.ToLower();

                if (string.IsNullOrEmpty(input))
                {
                    return response.ToString();
                }

                var words = input.Split(" ");

                foreach (var item in words)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        var capitalized = item.ToCharArray();
                        capitalized[0] = char.ToUpper(capitalized[0]);

                        response.Append($"{new string(capitalized)} ");
                    }
                }

                return response.ToString().Trim();
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Extension that encodes the given input to Base64.
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// Extension that decodes the given input from Base64.
        /// </summary>
        /// <param name="base64EncodedData"></param>
        /// <returns></returns>
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        /// <summary>
        /// Extension that checks if all elements from the given input are numbers.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNumber(this string value)
        {
            return value.All(char.IsNumber);
        }

        /// <summary>
        /// Extension that cleans the given input from any smalltalks and punctuations.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string CleanPunctuationsAndSmallTalks(this string input)
        {
            var _cleanNamePattern = new Regex(SMALLTALKS_CLEAN_PATTERN, RegexOptions.Compiled);

            return input.RemoveRegexPattern(_cleanNamePattern).CleanPunctuations(); ;
        }

        /// <summary>
        /// Extension that cleans the given input from any punctuations.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string CleanPunctuations(this string input)
        {
            var _ponctuationPattern = new Regex(SMALLTALKS_PUNCTUATION_PATTERN, RegexOptions.Compiled);

            return input.RemoveRegexPattern(_ponctuationPattern);
        }

        /// <summary>
        /// Extension that identifies and replaces any diatrictic characters for its unaccented correspondents.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RemoveDiacritics(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            text = text.Normalize(NormalizationForm.FormD);
            var chars = text.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();

            return new string(chars).Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// Extension that removes whitespaces from the entire input.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string GeneralTrim(this string text)
        {
            return text.Replace(WHITE_SPACE, string.Empty);
        }

        /// <summary>
        /// Extension that conforms the given input to a conventioned value to be used as payload for buttons from Blip mediatypes. 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ToPayloadValue(this string text)
        {
            return $"#{text.ToLower().CleanPunctuations().RemoveDiacritics().GeneralTrim()}";
        }

        /// <summary>
        /// Extension that wrappes a text string to a DocumentContainer containing a Document with the given text.
        /// </summary>
        /// <param name="payloadValue"></param>
        /// <returns></returns>
        public static DocumentContainer ToDocumentContainerPayload(this string text)
        {
            return text.ToPlainTextDocumentContainer();
        }

        /// <summary>
        /// Extension that wrappes a text string to DocumentContainer containing a PlainText document with the given text.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static DocumentContainer ToPlainTextDocumentContainer(this string text)
        {
            return PlainText.Parse(text).ToDocumentContainer();
        }

    }
}
