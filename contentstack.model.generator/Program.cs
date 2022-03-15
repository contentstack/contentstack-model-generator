using System;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace contentstack.model.generator
{
    class Program
    {
        public const int EXCEPTION = 2;
        public const int ERROR = 1;
        public const int OK = 0;

        protected Program()
        {
        }

        static async Task<int> Main(string[] args)
        {
            try
            {
                return await CommandLineApplication.ExecuteAsync<ModelGenerator>(args);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("Unexpected error: " + ex.ToString());
                Console.ResetColor();
                return EXCEPTION;
            }
        }
    }
}
