namespace API.ErrorResponse;

public class ApiResponse
{
    public ApiResponse(int statusCode, string errorMessage = null)
    {
        StatusCode = statusCode;
        ErrorMessage = errorMessage ?? DefaultErrorMessage(statusCode);
    }

    public int StatusCode { get; set; }
    public string ErrorMessage { get; set; }
    private string DefaultErrorMessage(int statusCode)
    {
        return statusCode switch
        {
            400 => "You have made a bad request",
            401 => "You are not authorized",
            404 => "Resource not found",
            500 => "Internal Server error",
            _ => "An error has occurred"
        };
    }
}