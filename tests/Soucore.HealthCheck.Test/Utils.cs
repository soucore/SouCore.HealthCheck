namespace Soucore.HealthCheck.Test
{
    public static class Utils
    {
        public static async Task<(string content, HttpResponseMessage response)> QueryHealthCheckEndpoint(string url)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(new Uri(url));
            var responseObject = await response.Content.ReadAsStringAsync();
            if (responseObject == null) throw new Exception("Response was null. Make sure the target project is running before executing tests");
            return (responseObject, response);
        }
    }
}
