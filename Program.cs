// See https://aka.ms/new-console-template for more information
using System.Text;
using Newtonsoft.Json;

var endpoint = "https://archtechoai.openai.azure.com/openai/deployments/ArchTechChat/chat/completions?api-version=2023-03-15-preview";
var client = new HttpClient();

Console.WriteLine("Hi, how may I help you?");

while(true)
{
    var input = Console.ReadLine();

    if(input == "exit")
    {
        break;
    }

    var message = PrepareMessage(input);

    var response = client.Send(message);

    if(response.IsSuccessStatusCode)
    {
        Console.WriteLine(UnpackResponseMessage(response));
        Console.WriteLine("-------------------------------------------------------------");
        Console.WriteLine("What more can I help you with?");
    }
    else
    {
        Console.WriteLine($"Request failed: {response.StatusCode}, {response.Content}");
    }
}

string UnpackResponseMessage(HttpResponseMessage response)
{
    var responseBody = JsonConvert.DeserializeObject<AzureOpenAIResponse>(response.Content.ReadAsStringAsync().Result);
    return responseBody.choices.First().message.content;
}

HttpRequestMessage PrepareMessage(string input)
{
    var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
    request.Headers.Add("api-key", "a513d467147f4a808c7b60f033897c1d");

    var payload = new 
    {
        messages = new []
        {
            new 
            {
                role = "system",
                content = "You are a helpful assistant with an overly courteous tone"
            },
            new
            {
                role = "user",
                content = input
            }
        }
    };

    var json = JsonConvert.SerializeObject(payload);
    var content = new StringContent(json, Encoding.UTF8, "application/json");

    request.Content = content;

    return request;
}

public class AzureOpenAIResponse
{
    public Choice[] choices { get; set; }
}

public class Choice
{
    public Message message { get; set; }
}

public class Message
{
    public string content { get; set; }
}
