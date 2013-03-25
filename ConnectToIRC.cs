using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace RuNnNy_IRC_Client
{
    class ConnectToIRC
    {
        public string server,nick,name,auth,chan;
        public int port;
        NetworkStream ns = null;
        StreamReader sr = null;
        StreamWriter sw = null;
        TcpClient ircClient = null;

        public void Config(string serv, int por, string nic, string nam, string aut)
        { 
            server = serv;
            port = por;
            nick = nic;
            name = nam;
            auth = aut;
        }

        public void StartIRCThread()
        {
            Thread ircThread = new Thread(new ThreadStart(this.ConnectToServer));
            ircThread.Start();
        }

        public void ConnectToServer()
        {
            try
            {
                ircClient = new TcpClient(server, port);
                ns = ircClient.GetStream();
                sr = new StreamReader(ns);
                sw = new StreamWriter(ns);

                sw.WriteLine("NICK " + nick + "\n\r");
                sw.WriteLine("USER " + name + " " + name + " " + name + " " + server + " :" + name + " RuNnNy's Client\n\r");
                sw.WriteLine("AUTH " + auth + "\n\r");
                sw.WriteLine(sr.ReadLine());
                sw.Flush();

                Console.WriteLine("Connected to " + server + ":" + port);
                if (ircClient.Connected)
                {
                    new Thread(delegate() { while (ircClient.Connected) { IRCOut(); } }).Start();
                    new Thread(delegate() { while (ircClient.Connected) { Console.Write("{"+server+"}"+" #"+chan+": "); IRCIn(Console.ReadLine()); } }).Start();
                }
            }
            catch
            {
                Console.WriteLine("Error Connecting to Server");
            }
        }

        public void IRCOut()
        {
            try
            {
                string[] ex;
                string data;

                data = sr.ReadLine();
                Console.WriteLine(data);
                char[] charSeparator = new char[] { ' ' };
                ex = data.Split(charSeparator, 5);

                if (ex[0] == "PING")
                {
                    sw.WriteLine("PONG");
                }
            }
            catch
            {
                Console.WriteLine("Error Communicating With Server");
            }
        }

        public void IRCIn(string cmd)
        {
                try
                {
                    if (cmd != "")
                    {
                        if (cmd[0] == '/')
                        {
                            string[] CMD = cmd.Split();
                            switch (CMD[0].ToLower())
                            {
                                case "/join":
                                    sw.WriteLine("JOIN #" + CMD[1]);
                                    chan = CMD[1];
                                    Console.WriteLine("Joining #" + CMD[1] + "...");
                                    break;
                                case "/quit":
                                    if(CMD.Length==1)
                                        sw.WriteLine("QUIT Quiting");
                                    else
                                        sw.WriteLine("QUIT " + RemCmd(cmd));
                                    Console.WriteLine("Disconnected From " + server + ":" + port);
                                    break;
                                case "/me":
                                    sw.WriteLine("PRIVMSG #" + chan + " :\u0001" + "ACTION " + RemCmd(cmd) + "\u0001");
                                    break;
                                case "/part":
                                    sw.WriteLine("PART #"+CMD[1]);
                                    break;
                                case "/kick":
                                    sw.WriteLine("KICK #"+chan+" "+CMD[1]);
                                    break;
                                case "/who":
                                    sw.WriteLine("WHO "+CMD[1]);
                                    break;
                                case "/whois":
                                    sw.WriteLine("WHOIS " + server+" "+CMD[1]);
                                    break;
                                case "/whowas":
                                    sw.WriteLine("WHOWAS "+CMD[1]);
                                    break;
                                case "/away":
                                    sw.WriteLine("AWAY " + RemCmd(cmd));
                                    break;
                                case "/users":
                                    sw.WriteLine("USERS "+server);
                                    break;
                                case "/ison":
                                    sw.WriteLine("ISON "+CMD[1]);
                                    break;
                                case "/topic":
                                    sw.WriteLine("TOPIC #"+chan+" "+RemCmd(cmd));
                                    break;
                                case "/names":
                                    sw.WriteLine("NAMES #"+chan);
                                    break;
                                case "/list":
                                    sw.WriteLine("LIST #" + chan);
                                    break;
                                case "/invite":
                                    sw.WriteLine("INVITE "+ CMD[1] + " #"+chan);
                                    break;
                                case "/privmsg":
                                    sw.WriteLine("PRIVMSG " + RemCmd(cmd));
                                    break;
                                case "/motd":
                                    sw.WriteLine("MOTD");
                                    break;
                                case "/lusers":
                                    sw.WriteLine("LUSERS");
                                    break;
                                case "/version":
                                    sw.WriteLine("VERSION");
                                    break;
                                case "/stats":
                                    sw.WriteLine("STATS");
                                    break;
                                case "/time":
                                    sw.WriteLine("TIME");
                                    break;
                                case "/admin":
                                    sw.WriteLine("ADMIN");
                                    break;
                                case "/info":
                                    sw.WriteLine("INFO");
                                    break;
                                case "/kill":
                                    sw.WriteLine("KILL "+CMD[1]);
                                    break;
                                case "/pass":
                                    sw.WriteLine("PASS "+CMD[1]);
                                    break;
                                default:
                                    Console.WriteLine("Command '" + CMD[0] + "' does not exist");
                                    break;
                            }
                        }
                        else
                        {
                            sw.WriteLine("PRIVMSG #" + chan + " :" + cmd);
                        }
                        sw.Flush();
                    }
                }
                catch
                {
                    Console.WriteLine("Error Communicating With Server");
                }
        }

        public string RemCmd(string cmd)
        {
            string[] CMD = cmd.Split();
            string msg = "";
            for (int i = 1; i < CMD.Length - 2; i++)
            {
                msg += cmd[i] + " ";
            }
            msg.Trim();
            return msg;
        }
    }
}
