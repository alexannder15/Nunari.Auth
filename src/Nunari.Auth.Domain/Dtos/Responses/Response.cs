namespace Nunari.Auth.Domain.Dtos.Responses;

public class ResponseDto<T>
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTimeOffset DateTime { get; set; } = DateTimeOffset.Now;
    public T? Data { get; set; }
}