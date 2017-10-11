using System;
using System.Net.Http;
using System.Threading.Tasks;
using Refit;

namespace RefitGlobalTimeout
{
    public interface IHttpBinApi
    {
        /// <summary>
        /// Simulates a slow response
        /// </summary>
        /// <param name="seconds">The number of seconds to delay before responding.</param>
        /// <returns>The raw response JSON</returns>
        [Get("/delay/{seconds}")]
        Task<string> Delay(int seconds);
    }

    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var httpBinApi = RestService.For<IHttpBinApi>(new HttpClient
            {
                BaseAddress = new Uri("http://httpbin.org"),
                Timeout = TimeSpan.FromSeconds(2)
            });

            // Ok
            await httpBinApi.Delay(0);
            Console.WriteLine("Response within timeout worked as expected");

            // Boom
            try
            {
                await httpBinApi.Delay(10);
                Console.Error.WriteLine(@"¯\_(ツ)_/¯ Request didn't time out like it should have");
                return 1;
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Request timed out as expected");
            }
            return 0;
        }
    }
}