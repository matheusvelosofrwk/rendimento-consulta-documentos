using ConsultaDocumentos.Web.Models;
using Refit;

namespace ConsultaDocumentos.Web.Clients
{
    public interface IDocumentoApi
    {
        [Get("/Documento")]
        Task<Result<IList<DocumentoViewModel>>> GetAllAsync();

        [Get("/Documento/{id}")]
        Task<Result<DocumentoViewModel>> GetByIdAsync(Guid id);

        [Post("/Documento")]
        Task<Result<DocumentoViewModel>> CreateAsync([Body] DocumentoViewModel model);

        [Put("/Documento/{id}")]
        Task<Result<DocumentoViewModel>> UpdateAsync(Guid id, [Body] DocumentoViewModel model);

        [Delete("/Documento/{id}")]
        Task<Result<bool>> DeleteAsync(Guid id);
    }
}
