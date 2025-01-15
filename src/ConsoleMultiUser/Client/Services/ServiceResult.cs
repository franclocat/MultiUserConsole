using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
