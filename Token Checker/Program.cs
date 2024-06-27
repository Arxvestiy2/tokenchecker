using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace DiscordTokenChecker
{
    class Program
    {
        static async Task<bool> CheckTokenAsync(string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", token);

                try
                {
                    HttpResponseMessage response = await client.GetAsync("https://discord.com/api/v9/users/@me");

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return true;
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        return false;
                    }
                    else
                    {
                        throw new Exception($"Unexpected status code: {response.StatusCode}");
                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request error: {e.Message}");
                    return false;
                }
            }
        }

        static void PrintAsciiArt()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(@"
  ___                            _   _       
 / _ \                          | | (_)      
/ /_\ \_ ____  ____   _____  ___| |_ _ _   _ 
|  _  | '__\ \/ /\ \ / / _ \/ __| __| | | | |
| | | | |   >  <  \ V /  __/\__ \ |_| | |_| |
\_| |_/_|  /_/\_\  \_/ \___||___/\__|_|\__, |
                                        __/ |
                                       |___/ 
");
        }

        static async Task Main(string[] args)
        {
            Console.Title = "Made by Arxvestiy | discord.gg/arxvestiy";
            PrintAsciiArt();

            string[] tokens = File.ReadAllLines("input.txt");
            using StreamWriter validWriter = new StreamWriter("valid.txt");
            using StreamWriter invalidWriter = new StreamWriter("invalid.txt");

            int validCount = 0;
            int invalidCount = 0;

            Console.WriteLine("Token checking started.");
            Console.WriteLine("------------------------------------------");

            
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            foreach (string token in tokens)
            {
                try
                {
                    // Record timestamp for each token
                    string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");


                    bool isValid = await CheckTokenAsync(token);
                    if (isValid)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"[{timestamp}] {token} is valid.");
                        validWriter.WriteLine(token);
                        validCount++;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[{timestamp}] {token} is invalid.");
                        invalidWriter.WriteLine(token);
                        invalidCount++;
                    }
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"An error occurred while checking token {token}: {e.Message}");
                    invalidWriter.WriteLine(token);
                    invalidCount++;
                }
                finally
                {
                    Console.ResetColor();
                }
            }

            // Stop stopwatch and calculate elapsed time
            stopwatch.Stop();
            TimeSpan elapsedTime = stopwatch.Elapsed;

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\nSummary:");
            Console.WriteLine($"Total valid tokens: {validCount}");
            Console.WriteLine($"Total invalid tokens: {invalidCount}");
            Console.WriteLine($"Time taken: {elapsedTime.TotalSeconds} seconds");

            Console.WriteLine("Token checking completed.");
            Console.WriteLine("Press any key to close...");

            Console.ReadKey(); //close
        }
    }
}
