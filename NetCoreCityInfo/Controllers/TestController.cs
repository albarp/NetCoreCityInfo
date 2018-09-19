using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace NetCoreCityInfo.Controllers
{
    [Route("api/test")]
    public class TestController : Controller
    {

        // In sostanza:
        // Thread.CurrentThread.CurrentPrincipal non viene mantenuto nel passaggio tra un thread e un altro
        //    questo implica:
        //        - il Thread, che serve la parte dx di await, non ha il CurrentPrincipal
        //        - Nella fase di autenticazione va impostato lo User di HttpContext, che, invece, sopravvive al cambio del contesto di esecuzione
        //        - Curiosamente anche CultureInfo.CurrentCulture sopravvive al passaggio di contesto

        [HttpGet("BothAsync")]
        public async Task<IActionResult> GetBothAsync()
        {
            if (CultureInfo.CurrentCulture.Name == "it-IT")
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            else
                Thread.CurrentThread.CurrentCulture = new CultureInfo("it-IT");

            Console.WriteLine("Main is running on thread {0}",
                         Thread.CurrentThread.ManagedThreadId);

            Console.WriteLine("Main CurrentCulture is {0}",
                              CultureInfo.CurrentCulture.Name);

            List<string> result = new List<string>();

            var task1 = GetOneAsync(result, "https://www.google.it/");
            var task2 = GetOneAsync(result, "http://www.ansa.it");

            Console.WriteLine("Main method after GetOneAsync {0}",
                         Thread.CurrentThread.ManagedThreadId);

            Console.WriteLine("Main method before Task.WaitAll {0}",
                         Thread.CurrentThread.ManagedThreadId);

            await Task.WhenAll(task1, task2);

            // Il giro giusto sarebbe recuperare il valore dei task con .Result e aggiornare la collezione?

            Console.WriteLine("Main method after Task.WhenAll {0}",
                         Thread.CurrentThread.ManagedThreadId);

            Console.WriteLine("Main CurrentCulture after Task.WhenAll is {0}",
                              CultureInfo.CurrentCulture.Name);

            Console.WriteLine("Result.Count {0}",
                         result.Count);

            return Ok();
        }

        private async Task GetOneAsync(List<string> result, string url)
        {
            HttpClient client = new HttpClient();

            //string res = await client.GetStringAsync(url);

            await Task.Delay(5000);

            Console.WriteLine("GetOneAsync Continuation is running on thread {0}",
                         Thread.CurrentThread.ManagedThreadId);

            Console.WriteLine("GetOneAsync Continuation CurrentCulture is {0}",
                              CultureInfo.CurrentCulture.Name);

            result.Add("aaaaaa");
        }

        [HttpGet("Principal")]
        public async Task<IActionResult> PrincipalTestAsync()
        {

            // Setup principal
            const string Issuer = "https://gov.uk";

            var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, "Andrew", ClaimValueTypes.String, Issuer),
                    new Claim(ClaimTypes.Surname, "Lock", ClaimValueTypes.String, Issuer),
                    new Claim(ClaimTypes.Country, "UK", ClaimValueTypes.String, Issuer),
                    new Claim("ChildhoodHero", "Ronnie James Dio", ClaimValueTypes.String)
            };

            var userIdentity = new ClaimsIdentity(claims, "Passport");

            var userPrincipal = new ClaimsPrincipal(userIdentity);

            Console.WriteLine("The example is running on thread {0}",
                         Thread.CurrentThread.ManagedThreadId);

            //var username = userPrincipal?.FindFirstValue(ClaimTypes.Name);

            // Questo rimane
            HttpContext.User = userPrincipal;

            // Questo si perde
            Thread.CurrentPrincipal = userPrincipal;

            Console.WriteLine("Userame from HttpContext {0}", HttpContext.User.Identity.Name);

            Console.WriteLine("Userame from Thread.CurrentPrincipal {0}", Thread.CurrentPrincipal.Identity.Name);

            HttpClient httpClient = new HttpClient();

            var x = await httpClient.GetAsync("https://goo.gl/");//.ConfigureAwait(false);

            Console.WriteLine("Continuation is running on thread {0}",
                         Thread.CurrentThread.ManagedThreadId);

            // OK
            Console.WriteLine("Userame from HttpContext {0}", HttpContext.User.Identity.Name);

            // Crash
            Console.WriteLine("Userame {0}", Thread.CurrentPrincipal.Identity.Name);

            return Ok();
        }

        [HttpGet("CultureInfo")]
        public IActionResult CultureInfoTest()
        {
            //var logger = ApplicationLogging.CreateLogger<TestController>();

            //logger.LogError("test");

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine("The example is running on thread {0}",
                         Thread.CurrentThread.ManagedThreadId);
            // Make the current culture different from the system culture.
            Console.WriteLine("The current culture is {0}",
                              CultureInfo.CurrentCulture.Name);
            if (CultureInfo.CurrentCulture.Name == "it-IT")
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            else
                Thread.CurrentThread.CurrentCulture = new CultureInfo("it-IT");

            Console.WriteLine("Changed the current culture to {0}.\n",
                              CultureInfo.CurrentCulture.Name);


            // Execute the delegate synchronously.
            Console.WriteLine("Executing the delegate synchronously:");
            Console.WriteLine(formatDelegate());

            // Call an async delegate to format the values using one format string.
            Console.WriteLine("Executing a task asynchronously:");
            var t1 = Task.Run(formatDelegate);
            Console.WriteLine(t1.Result);

            Console.WriteLine("Executing a task synchronously:");
            var t2 = new Task<String>(formatDelegate);
            t2.RunSynchronously();
            Console.WriteLine(t2.Result);

            return Ok();

        }


        // Print culture
        static decimal[] values = { 163025412.32m, 18905365.59m };
        static string formatString = "C2";
        Func<String> formatDelegate = () => {
            string output = String.Format("Formatting using the {0} culture on thread {1}.\n",
                                          CultureInfo.CurrentCulture.Name,
                                          Thread.CurrentThread.ManagedThreadId);
            foreach (var value in values)
                output += String.Format("{0}   ", value.ToString(formatString));

            output += Environment.NewLine;
            return output;
        };
    }
}