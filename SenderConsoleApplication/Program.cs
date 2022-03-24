using APShared.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SenderConsoleApplication
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Employee emp = new Employee();
            Console.WriteLine("Enter First Name");
            emp.FirstName = Console.ReadLine();
            Console.WriteLine("Enter Last Name");
            emp.LastName = Console.ReadLine();
            string empJson = JsonConvert.SerializeObject(emp);
            using (var client = new HttpClient())
            {
                var res = await client.PostAsync("http://localhost:7071/api/Function1", new StringContent(empJson,Encoding.UTF8,"application/json"));
                                                //Endpoints will be saved in config files. For demo purposes it is hardcoded
                if (res.IsSuccessStatusCode)
                {
                    Console.WriteLine("Data Pushed");
                }
                else
                {
                    Console.WriteLine("Error");
                }
            }    
        }
    }
}
