using System.Net.Http.Json;

namespace Client.Services;

public class GenericServiceResult<ReturnType> : ServiceResult
{
    public ReturnType? Value { get; set; }
    public GenericServiceResult(HttpResponseMessage responseMessage) : base(responseMessage)
    {
        if (responseMessage.IsSuccessStatusCode)
        {
            // Cause ReadAsAsync<>() cannot read a plain string result.
            if (typeof(ReturnType) == typeof(string))
            {
                string result = responseMessage.Content.ReadAsStringAsync().Result;
                Value = (ReturnType)Convert.ChangeType(result, typeof(ReturnType));
            }
            else
            {
                Value = responseMessage.Content.ReadFromJsonAsync<ReturnType>().Result;
            }
        }
    }
}
