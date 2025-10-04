namespace ConsultaDocumentos.Application.Results
{
    public class Result<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public List<string> Notifications { get; set; } = new List<string>();

        public static Result<T> SuccessResult(T data)
        {
            return new Result<T>
            {
                Success = true,
                Data = data,
                Notifications = new List<string>()
            };
        }

        public static Result<T> FailureResult(params string[] errors)
        {
            return new Result<T>
            {
                Success = false,
                Data = default,
                Notifications = errors.ToList()
            };
        }

        public static Result<T> FailureResult(List<string> errors)
        {
            return new Result<T>
            {
                Success = false,
                Data = default,
                Notifications = errors
            };
        }
    }
}
