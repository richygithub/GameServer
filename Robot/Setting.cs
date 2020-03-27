using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
namespace Robot
{
    public static class Setting
    {

        public static IConfigurationRoot Configuration { get; }
        static Setting()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(ProcessDirectory)
                .AddJsonFile("res/config.json")
                .Build();


        }

        public static int Port => int.Parse(Configuration["port"]);
        public static string Host => Configuration["host"];
        public static List<string> TestCases => Configuration.GetSection("case").Get<List<string>>();  

        public static string ProcessDirectory
        {
            get
            {
#if NETSTANDARD1_3
                return AppContext.BaseDirectory;
#else
                return AppDomain.CurrentDomain.BaseDirectory;
#endif
            }
        }
    }
}
