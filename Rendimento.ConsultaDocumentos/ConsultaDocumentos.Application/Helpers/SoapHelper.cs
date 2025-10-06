using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace ConsultaDocumentos.Application.Helpers
{
    public static class SoapHelper
    {
        public static string CriarEnvelopeSerasa(string operacao, Dictionary<string, string> parametros, string namespaceUri = "http://www.experianmarketing.com.br/")
        {
            var sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine("<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"");
            sb.AppendLine("               xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"");
            sb.AppendLine("               xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">");
            sb.AppendLine("  <soap:Body>");
            sb.AppendLine($"    <{operacao} xmlns=\"{namespaceUri}\">");

            foreach (var param in parametros)
            {
                sb.AppendLine($"      <{param.Key}>{SecurityElement.Escape(param.Value)}</{param.Key}>");
            }

            sb.AppendLine($"    </{operacao}>");
            sb.AppendLine("  </soap:Body>");
            sb.AppendLine("</soap:Envelope>");

            return sb.ToString();
        }

        public static string CriarEnvelopeSerpro(string operacao, Dictionary<string, string> parametros, string namespaceUri = "https://acesso.infoconv.receita.fazenda.gov.br/ws/cnpj/")
        {
            var sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine("<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"");
            sb.AppendLine("               xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"");
            sb.AppendLine("               xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">");
            sb.AppendLine("  <soap:Body>");
            sb.AppendLine($"    <{operacao} xmlns=\"{namespaceUri}\">");

            foreach (var param in parametros)
            {
                sb.AppendLine($"      <{param.Key}>{SecurityElement.Escape(param.Value)}</{param.Key}>");
            }

            sb.AppendLine($"    </{operacao}>");
            sb.AppendLine("  </soap:Body>");
            sb.AppendLine("</soap:Envelope>");

            return sb.ToString();
        }

        public static XmlDocument ParsearRespostaSoap(string xmlResponse)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xmlResponse);
            return doc;
        }

        public static string? ExtrairValorXml(XmlDocument doc, string tagName)
        {
            var nodes = doc.GetElementsByTagName(tagName);
            if (nodes.Count > 0 && nodes[0]?.FirstChild != null)
            {
                return nodes[0].FirstChild.InnerText?.Trim();
            }
            return null;
        }

        public static XmlNodeList? ObterNosXml(XmlDocument doc, string tagName)
        {
            return doc.GetElementsByTagName(tagName);
        }

        public static XDocument ParsearRespostaSoapComoXDocument(string xmlResponse)
        {
            return XDocument.Parse(xmlResponse);
        }

        public static string? ExtrairErroSoap(XmlDocument doc)
        {
            // Tenta extrair tag <Erro>
            var nodes = doc.GetElementsByTagName("Erro");
            if (nodes.Count > 0 && nodes[0]?.FirstChild != null)
            {
                var erro = nodes[0].FirstChild.InnerText?.Trim();
                if (!string.IsNullOrEmpty(erro))
                {
                    return erro;
                }
            }
            return null;
        }
    }

    // Helper para escapar caracteres XML
    internal static class SecurityElement
    {
        public static string Escape(string? value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            return value
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;");
        }
    }
}
