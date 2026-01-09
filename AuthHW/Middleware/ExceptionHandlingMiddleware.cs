using System.Net;
using System.Text.Json;

namespace AuthHW.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Произошло необработанное исключение");

        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new ErrorResponse();

        switch (exception)
        {
            case KeyNotFoundException e:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.Message = e.Message ?? "Ресурс не найден";
                break;

            case UnauthorizedAccessException e:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.Message = e.Message ?? "Доступ запрещен";
                break;

            case ValidationException e:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = e.Message ?? "Ошибка валидации";
                errorResponse.Errors = e.Errors;
                break;

            case BusinessException e:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = e.Message ?? "Бизнес-ошибка";
                errorResponse.ErrorCode = e.ErrorCode;
                break;

            case BadHttpRequestException e:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = e.Message ?? "Неверный запрос";
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.Message = _environment.IsDevelopment() 
                    ? exception.Message 
                    : "Внутренняя ошибка сервера";
                break;
        }

        // Для разработки добавляем детали исключения
        if (_environment.IsDevelopment())
        {
            errorResponse.StackTrace = exception.StackTrace;
            errorResponse.InnerException = exception.InnerException?.Message;
            errorResponse.Type = exception.GetType().Name;
        }

        var result = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        await response.WriteAsync(result);
    }
}

// Класс для стандартизированного ответа об ошибке
public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public string? ErrorCode { get; set; }
    public Dictionary<string, string[]>? Errors { get; set; }
    
    // Только для разработки
    public string? StackTrace { get; set; }
    public string? Type { get; set; }
    public string? InnerException { get; set; }
}

// Примеры кастомных исключений
public class BusinessException : Exception
{
    public string ErrorCode { get; }

    public BusinessException(string message, string errorCode = "BUSINESS_ERROR") 
        : base(message)
    {
        ErrorCode = errorCode;
    }
}

public class ValidationException : Exception
{
    public Dictionary<string, string[]> Errors { get; }

    public ValidationException(Dictionary<string, string[]> errors) 
        : base("Произошла одна или несколько ошибок валидации")
    {
        Errors = errors;
    }

    public ValidationException(string propertyName, string errorMessage)
        : this(new Dictionary<string, string[]> 
        { 
            [propertyName] = new[] { errorMessage } 
        })
    {
    }
}