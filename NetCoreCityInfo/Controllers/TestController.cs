using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace NetCoreCityInfo.Controllers
{
    [Route("api/test")]
    public class TestController : Controller
    {


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