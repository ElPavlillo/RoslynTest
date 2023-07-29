using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace RoslynTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string code2 = @"
                using System;
                using System.Net.Http;
                public class MyClass
                {
                    public int Add(int a, int b)
                    {
                        return a + b;
                    }

                    public static async Task MakeRequest(){
                        var httpClient = new HttpClient();
                        var ip = await httpClient.GetStringAsync(""https://api.ipify.org"");
                        Console.WriteLine($""My public IP address is: {ip}"");
                    }
                }

                MyClass myClass = new MyClass();
                MyClass.MakeRequest();
                //return myClass.Add(10, 20);
            ";

            string code = @"
                using System.Net.Http;

                HttpClientHandler handler = new HttpClientHandler();
                handler.AutomaticDecompression = DecompressionMethods.All;

                HttpClient client = new HttpClient(handler);

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, ""https://catfact.ninja/fact"");

                request.Headers.Add(""authority"", ""catfact.ninja"");
                request.Headers.Add(""accept"", ""text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7"");
                request.Headers.Add(""accept-language"", ""es-419,es;q=0.9,en;q=0.8,zh-CN;q=0.7,zh;q=0.6"");
                request.Headers.Add(""cache-control"", ""max-age=0"");
                request.Headers.Add(""cookie"", ""XSRF-TOKEN=eyJpdiI6InBZWkl6V3o1VkM0b0dxVGoweXErcHc9PSIsInZhbHVlIjoiTTZVNG5TbEIrYUg0RlNBdk1VeEU3ZDZLNFZyTVAzemdROXl4VWUvVnJYS1dCdlFHNjFia3VteDRxU0dTc0pILzcxSmtrWkhNeXkyUi9jWVlIZ1BaaWlsWHdOajhHVm1MYlhGTEI5Mmc4NVpjOFpCVGw2UmczQkZ2ckdiODA4NmQiLCJtYWMiOiJlNTBmYjMzZjlkM2M3ZmNjYjg2Mzc3NzBjYTU1ODA4MjE2OTA1YzRkNTRjMWRkNWYxZDQzZjI5ZjZlYmZlODFlIiwidGFnIjoiIn0%3D; catfacts_session=eyJpdiI6Ikt6N3V0VlQrSit1U0VTZjAyWTRQQVE9PSIsInZhbHVlIjoiMTgxTTYyT2ZabDRGdFV5ejlSWUlyS0hEYi84RUFqMlpSUGpxbXV0cnMwTnVhY016NC83eCtEQkpHWS9iaHJkZkh6TzhDMjg3T3hLS0ZydlR3NzYyWUkzVXlUcjFjeDd1d3hXVzZJVVFmQlNzRm43WTAvdjN6dEIzdWdsQjFnMlQiLCJtYWMiOiJkNmM0ZTVlNGY0NzMxMmNiOGUxMzg2NmQxMDg0YjBjNThiM2VlYzk0ODE1MjdkMTg1YjQwZGY3ZDMwODM2Nzk1IiwidGFnIjoiIn0%3D"");
                request.Headers.Add(""sec-ch-ua"", ""\""Not/A)Brand\"";v=\""99\"", \""Google Chrome\"";v=\""115\"", \""Chromium\"";v=\""115\"""");
                request.Headers.Add(""sec-ch-ua-mobile"", ""?0"");
                request.Headers.Add(""sec-ch-ua-platform"", ""\""Windows\"""");
                request.Headers.Add(""sec-fetch-dest"", ""document"");
                request.Headers.Add(""sec-fetch-mode"", ""navigate"");
                request.Headers.Add(""sec-fetch-site"", ""none"");
                request.Headers.Add(""sec-fetch-user"", ""?1"");
                request.Headers.Add(""upgrade-insecure-requests"", ""1"");
                request.Headers.Add(""user-agent"", ""Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/115.0.0.0 Safari/537.36"");

                HttpResponseMessage response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);
            ";

            ExecuteCodeAsync(code).Wait();
            Console.ReadLine();
        }

        public static async Task ExecuteCodeAsync(string code)
        {
            try
            {
                // Create a list of references to assemblies that the code might depend on.
                var references = new[]
                {
                typeof(object).Assembly,
                typeof(Console).Assembly,
                typeof(HttpClient).Assembly,
                typeof(Task).Assembly,
                typeof(DecompressionMethods).Assembly,
                // Add other assemblies if needed
            };

                // Create options with the necessary references.
                var options = ScriptOptions.Default
                    .AddReferences(references)
                    .AddImports("System", "System.Net", "System.Net.Http", "System.Threading.Tasks");

                // Evaluate the code and get the result.
                var result = await CSharpScript.EvaluateAsync(code, options);

                // Do something with the result (if required).
                Console.WriteLine("Result: " + result);
            }
            catch (CompilationErrorException compilationError)
            {
                // Handle compilation errors if any.
                foreach (var diagnostic in compilationError.Diagnostics)
                {
                    Console.WriteLine(diagnostic.ToString());
                }
            }
            catch (Exception ex)
            {
                // Handle other exceptions if any.
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}