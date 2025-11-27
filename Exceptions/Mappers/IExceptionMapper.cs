using Microsoft.AspNetCore.Mvc;

namespace ECM_BE.Exceptions.Mappers
{
    public interface IExceptionMapper
    {
        bool CanMap(Exception ex);
        ProblemDetails ToProblemDetails(Exception ex);
        object ToBasicError(Exception ex);
        int GetStatusCode();
    }
}
