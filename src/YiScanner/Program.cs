﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.YiScanner.Commands;

namespace Wikiled.YiScanner
{
    public class Program
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            log.Info("Starting {0} version utility...", Assembly.GetExecutingAssembly().GetName().Version);
            List<Command> commandsList = new List<Command>();
            commandsList.Add(new MonitorCommand());
            commandsList.Add(new DownloadCommand());
            var commands = commandsList.ToDictionary(item => item.Name, item => item, StringComparer.OrdinalIgnoreCase);

            try
            {
                if (args.Length == 0)
                {
                    log.Warn("Please specify arguments");
                    CommandLineParser.PrintCommands(commands.Values);
                    return;
                }

                if (!commands.TryGetValue(args[0], out var command))
                {
                    log.Error("Unknown Command");
                    return;
                }

                command.ParseArguments(args.Skip(1));
                command.Execute();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
    }
}