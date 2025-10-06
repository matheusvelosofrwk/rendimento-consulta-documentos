using System.Net;

namespace ConsultaDocumentos.Application.Exceptions
{
    public class ExternalProviderException : Exception
    {
        public string? ProviderName { get; set; }
        public HttpStatusCode? StatusCode { get; set; }
        public string? ResponseBody { get; set; }
        public DateTime OccurredAt { get; set; }

        public ExternalProviderException(string message) : base(message)
        {
            OccurredAt = DateTime.UtcNow;
        }

        public ExternalProviderException(string message, Exception innerException)
            : base(message, innerException)
        {
            OccurredAt = DateTime.UtcNow;
        }

        public ExternalProviderException(
            string providerName,
            string message,
            HttpStatusCode? statusCode = null,
            string? responseBody = null)
            : base(message)
        {
            ProviderName = providerName;
            StatusCode = statusCode;
            ResponseBody = responseBody;
            OccurredAt = DateTime.UtcNow;
        }

        public ExternalProviderException(
            string providerName,
            string message,
            Exception innerException,
            HttpStatusCode? statusCode = null,
            string? responseBody = null)
            : base(message, innerException)
        {
            ProviderName = providerName;
            StatusCode = statusCode;
            ResponseBody = responseBody;
            OccurredAt = DateTime.UtcNow;
        }

        public override string ToString()
        {
            var details = $"Provider: {ProviderName ?? "Unknown"}, " +
                         $"StatusCode: {StatusCode?.ToString() ?? "N/A"}, " +
                         $"Occurred: {OccurredAt:yyyy-MM-dd HH:mm:ss}";

            if (!string.IsNullOrWhiteSpace(ResponseBody))
            {
                details += $", Response: {ResponseBody}";
            }

            return $"{base.ToString()}\n{details}";
        }
    }
}
