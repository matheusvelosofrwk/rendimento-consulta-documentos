using System.Globalization;
using System.Text.RegularExpressions;

namespace ConsultaDocumentos.Application.Helpers
{
    public static class DecimalConversionHelper
    {
        public static decimal? ConvertToDecimal(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            // Remove caracteres não numéricos exceto vírgula e ponto
            var cleanStr = Regex.Replace(value, @"[^\d,.]", "");

            if (string.IsNullOrWhiteSpace(cleanStr))
                return null;

            // Substitui vírgula por ponto
            cleanStr = cleanStr.Replace(',', '.');

            if (decimal.TryParse(cleanStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }

            return null;
        }

        public static decimal ConvertToDecimalOrDefault(string? value, decimal defaultValue = 0)
        {
            return ConvertToDecimal(value) ?? defaultValue;
        }
    }
}
