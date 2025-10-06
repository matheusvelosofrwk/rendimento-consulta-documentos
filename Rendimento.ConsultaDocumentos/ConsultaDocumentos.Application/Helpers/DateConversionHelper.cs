using System.Globalization;

namespace ConsultaDocumentos.Application.Helpers
{
    public static class DateConversionHelper
    {
        private static readonly string[] SerproFormats = { "dd/MM/yyyy", "ddMMyyyy", "yyyy-MM-ddTHH:mm:ss" };
        private static readonly string[] SerasaFormats = { "dd/MM/yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "yyyy-MM-ddTHH:mm:ss" };

        public static DateTime? ParseSerproDate(string? dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
                return null;

            foreach (var format in SerproFormats)
            {
                if (DateTime.TryParseExact(dateString, format, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var date))
                {
                    return date;
                }
            }

            // Tenta parse genérico como fallback
            if (DateTime.TryParse(dateString, out var genericDate))
            {
                return genericDate;
            }

            return null;
        }

        public static DateTime? ParseSerasaDate(string? dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
                return null;

            foreach (var format in SerasaFormats)
            {
                if (DateTime.TryParseExact(dateString, format, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var date))
                {
                    return date;
                }
            }

            // Tenta parse genérico como fallback
            if (DateTime.TryParse(dateString, out var genericDate))
            {
                return genericDate;
            }

            return null;
        }

        public static int? ParseAnoObito(string? anoString)
        {
            if (string.IsNullOrWhiteSpace(anoString))
                return null;

            if (int.TryParse(anoString, out var ano))
            {
                if (ano >= 1900 && ano <= DateTime.Now.Year)
                    return ano;
            }

            return null;
        }

        public static bool ParseResidenteExterior(string? valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return false;

            var valorUpper = valor.Trim().ToUpperInvariant();

            return valorUpper == "S" ||
                   valorUpper == "SIM" ||
                   valorUpper == "TRUE" ||
                   valorUpper == "1" ||
                   valorUpper == "YES" ||
                   valorUpper == "Y";
        }
    }
}
