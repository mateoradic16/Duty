using Rocket.Unturned.Player;
using Rocket.API;
using Rocket.Unturned.Chat;
using System.Collections.Generic;
using UnityEngine;

namespace EFG.Duty
{
    public class CommandDutycheck : IRocketCommand
    {
        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length == 0)
            {
                if (caller is ConsolePlayer)
                {
                    Rocket.Core.Logging.Logger.LogWarning("No argument was specified. Please use \"dc <playername>\" to check on a player.");
                }
                else if (caller is UnturnedPlayer)
                {
                    UnturnedChat.Say(caller, "No argument was given. Please use \"/dc <playername>\" to check a player.");
                }
            }
            else if (command.Length > 0)
            {
                UnturnedPlayer cplayer = UnturnedPlayer.FromName(command[0]);
                bool can = false;
                foreach(var dg in Duty.Instance.Configuration.Instance.DutyGroups)
                {
                    if(cplayer.HasPermission(dg.Permission))
                    {
                        can = true;
                    };
                }
                if(!can)
                {
                    can = cplayer.HasPermission(Duty.Instance.Configuration.Instance.SuperAdminPermission);
                }
                if (can)
                {
                    Duty.Instance.cduty(cplayer, caller);
                }
                else
                {
                    UnturnedChat.Say(caller, Duty.Instance.Translate("cannot_be_in_duty", cplayer.DisplayName),Color.red);
                }
            }
        }

        public string Help
        {
            get { return "Checks if a player has admin powers or not."; }
        }

        public string Name
        {
            get { return "DutyCheck"; }
        }

        public string Syntax
        {
            get { return "<playername>"; }
        }

        public bool AllowFromConsole
        {
            get { return true; }
        }

        public AllowedCaller AllowedCaller
        {
            get { return AllowedCaller.Both; }
        }
        public List<string> Aliases
        {
            get { return new List<string>() { "dc" }; }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>() { "duty.check" };
            }
        }
    }
}
