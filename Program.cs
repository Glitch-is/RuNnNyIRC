using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace RuNnNy_IRC_Client
{
    class Program
    {
        public static string server, nick, name, auth;
        public static int port;
        static void Main(string[] args)
        {
            string ver = "A0.1";
            
            Console.WriteLine("RuNnNy IRC Client Version: "+ver);
            Console.WriteLine("Checking for Configuration");
            if (File.Exists("config.txt"))
            {
                Console.WriteLine("Configuration Found");
                config();
            }
            else
            {
                Console.WriteLine("Configuration Not Found");
                manConfig();
            }
            Connect();
            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }

        public static void config()
        {
            string[] con = File.ReadAllText("config.txt").Split();
            server = con[0];
            port = Convert.ToInt32(con[1]);
            nick = con[2];
            name = con[3];
            auth = con[4]; //encrypt l8r
        }

        public static void manConfig()
        {
            Console.Write("Server: ");
            server = Console.ReadLine();
            Console.Write("Port: ");
            port = Convert.ToInt32(Console.ReadLine());
            Console.Write("Nick: ");
            nick = Console.ReadLine();
            Console.Write("Name: ");
            name = Console.ReadLine();
            Console.Write("Auth: ");
            auth = Console.ReadLine();
            Console.Write("Save Configuration?(y/n): ");
            string opt = Console.ReadLine();
            if (opt.ToLower() == "y")
            {
                using (StreamWriter sw = new StreamWriter("config.txt"))
			    {
                    sw.Write(server+" "+port+" "+nick+" "+name+" "+auth);
			    }
            }
        }

        public static void Connect()
        {
            ConnectToIRC irc = new ConnectToIRC();
            Console.WriteLine("Connecting to " + server + ":" + port);
            irc.Config(server, port, nick, name, auth);
            irc.StartIRCThread();
        }
    }
}
