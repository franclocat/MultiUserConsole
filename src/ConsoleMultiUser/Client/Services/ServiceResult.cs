namespace Client.Services;

public class ServiceResult
{
    public bool IsSuccessful { get; set; }

    public string ErrorMessage { get; set; } = string.Empty;

    public ServiceResult(HttpResponseMessage responseMessage)
    {
        IsSuccessful = responseMessage.IsSuccessStatusCode;

        if (!responseMessage.IsSuccessStatusCode)
        {
            ErrorMessage = $"{responseMessage.ReasonPhrase} - {responseMessage.Content.ReadAsStringAsync().Result}";
        }
    }
}
