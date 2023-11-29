using Newtonsoft.Json;
using System.Text;

namespace AmigoSecreto.Desktop
{
    public class ApiHelper
    {
        private readonly HttpClient _httpClient;
        public ApiHelper()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(Configuration.GetApiUrl());
            _httpClient.Timeout = TimeSpan.FromSeconds(40);
        }

        public async Task<string> GetAsync(string uri)
        {
            try
            {
                using (var response = await _httpClient.GetAsync(uri))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        MessageBox
                            .Show(@$"Erro ao realizar chamada na API.
                                     StatusCode: {(int)response.StatusCode}
                                     {response.Headers}");

                        return string.Empty;
                    }

                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
        }
        public async Task<int> PostAync(string uri, Object obj)
        {
            try
            {
                HttpContent content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(uri, content);

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show(@$"Erro ao salvar amigo.
                                       StatusCode: {(int)response.StatusCode}");
                    return (int)response.StatusCode;
                }
                else
                {
                    MessageBox.Show(@$"Amigo cadastrado com sucesso.");
                    return (int)response.StatusCode;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
    }
}