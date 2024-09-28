using StarredSeaMUON.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON.Commands
{
    internal class CommandParser
    {
        public static List<Command> commands = new List<Command>();

        static CommandParser()
        {
            //order matters here! affects name matching priority
            commands.Add(new CommandLook());
        }

        private static (Command?, CommandParam[]?) findApplicableCommand(ClientConnection caller, string fullInput)
        {
            string lastError = "";
            foreach(Command c in commands)
            {
                int cL = c.DoesNameMatch(fullInput);
                if (cL != 0)
                {
                    string paramsString = fullInput.Substring(cL).TrimStart();
                    foreach (CommandParamType[] paramTypes in c.commandParamLists)
                    {
                        bool isOk = true;
                        string paramsRemaining = paramsString;
                        List<CommandParam> pars = new List<CommandParam>();
                        foreach(CommandParamType p in paramTypes)
                        {
                            (bool ok, paramsRemaining, CommandParam? outParam) = ParseNextParam(caller, paramsRemaining, p);
                            if (!ok) { isOk = false; break; }
                            Logger.Log("Got OK parse: " + outParam.value + ". Remaining: '" + paramsRemaining + "'");
                            pars.Add(outParam);
                        }
                        if (paramsRemaining.Length > 0 || !isOk)
                        {
                            lastError = "Incorrect parameters for command " + c.commandName + ".\n Usage: "+c.commandUsage;
                            continue;
                        }
                        Logger.Log("Got OK command with " + paramTypes.Length + " params.");
                        return (c, pars.ToArray());
                    }
                }
            }
            MessageSender.SendError(caller, lastError, false, true);
            return (null, null);
        }

        public static void TryRunCommand(ClientConnection caller, string fullInput)
        {
            (Command? c, CommandParam[]? pars) = findApplicableCommand(caller, fullInput);
            if(c == null)
            {
                MessageSender.SendError(caller, "Unknown Command.");
                return;
            }
            MessageSender.SendText(caller.writer, "Detected Command: " + c.ToString());
            MessageSender.SendText(caller.writer, "Args: ");
            foreach(CommandParam par in pars)
            {
                MessageSender.SendText(caller.writer, par.type.ToString() + " - " + par.value);
            }
            c.Call(caller, pars);
        }

        private static (bool, string, CommandParam?) ParseNextParam(ClientConnection caller, string _paramString, CommandParamType toParse)
        {
            string paramString = _paramString.Trim();
            Logger.Log("Attempting parse of " + toParse.ToString() + " on '" + paramString + "'");
            if (toParse == CommandParamType.INT)
            {
                int endOfTerm = paramString.IndexOf(' ');
                if (endOfTerm == -1) endOfTerm = paramString.Length;
                string target = paramString.Substring(0, endOfTerm);
                int val;
                if (int.TryParse(target, out val))
                {
                    return (true, paramString.Substring(endOfTerm), new CommandParam(target, CommandParamType.INT));
                }
            }
            else if (toParse == CommandParamType.DOUBLE)
            {
                int endOfTerm = paramString.IndexOf(' ');
                if (endOfTerm == -1) endOfTerm = paramString.Length;
                string target = paramString.Substring(0, endOfTerm);
                double val;
                if (double.TryParse(target, out val))
                {
                    return (true, paramString.Substring(endOfTerm), new CommandParam(target, CommandParamType.DOUBLE));
                }
            }
            else if (toParse == CommandParamType.STRING)
            {
                int startOfTerm = paramString.IndexOf('\"');
                int endOfTerm = paramString.IndexOf('\"', startOfTerm + 1);
                if (startOfTerm == -1) startOfTerm = 0;
                if (endOfTerm == -1) endOfTerm = paramString.Length;
                string target = paramString.Substring(startOfTerm, endOfTerm - startOfTerm);
                return (true, paramString.Substring(endOfTerm), new CommandParam(target, CommandParamType.STRING));
            }
            else if (toParse == CommandParamType.TARGET)
            {
                int endOfTerm = paramString.IndexOf(' ');
                if (endOfTerm == -1) endOfTerm = paramString.Length;
                string target = paramString.Substring(0, endOfTerm);
                //TODO: target searching
                return (true, paramString.Substring(endOfTerm), new CommandParam(target, CommandParamType.TARGET));
            }
            return (false, paramString, null);
        }
    }
}
