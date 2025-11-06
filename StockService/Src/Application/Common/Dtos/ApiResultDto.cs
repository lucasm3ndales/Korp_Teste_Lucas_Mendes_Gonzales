namespace StockService.Application.Common.Dtos;

public record ApiResultDto<T>
{
    public bool IsSuccess { get; init; }
    public List<string> Messages { get; init; } = [];
    public T? Data { get; init; }
    
    private ApiResultDto() { }
    
    public static ApiResultDto<T> Success(string message, T data)
    {
        return new ApiResultDto<T>
        {
            IsSuccess = true,
            Data = data,
            Messages = [message]
        };
    }
    
    public static ApiResultDto<T> Success(T data)
    {
        return new ApiResultDto<T>
        {
            IsSuccess = true,
            Data = data
        };
    }
    
    public static ApiResultDto<T> Failure(string errorMessage)
    {
        return new ApiResultDto<T>
        {
            IsSuccess = false,
            Data = default,
            Messages = [errorMessage]
        };
    }
    
    public static ApiResultDto<T> Failure(List<string> errorMessages)
    {
        return new ApiResultDto<T>
        {
            IsSuccess = false,
            Data = default,
            Messages = errorMessages
        };
    }
}