using Rocket.Unturned.Player;
using Rocket.API;
using System.Collections.Generic;
using Rocket.Unturned.Chat;
using UnityEngine;

namespace EFG.Duty
{
    public class CommandReasons : IRocketCommand
    {
        public void Execute(IRocketPlayer caller, string[] command)
        {
            bool c = true;
            UnturnedPlayer player = (UnturnedPlayer)caller;
            foreach (var pod in Duty.Instance.OnDuty_Players)
            {
                Rocket.Core.Logging.Logger.LogWarning(pod.Player.DisplayName);
                if (pod.Player.CSteamID == player.CSteamID)
                {
                    Rocket.Core.Logging.Logger.LogWarning(caller.DisplayName);
                    Duty.Instance.duty(player, null, null);
                    c = false;
                }
            }
            if (c)
            {
                if (command.Length == 0)
                {
                    command = new string[] { "" };
                }
                DutyGroup p_dg = null;
                foreach (var dg in Duty.Instance.Configuration.Instance.DutyGroups)
                {
                    if (player.HasPermission(dg.Permission))
                    {
                        p_dg = dg;
                    }
                }
                if (p_dg == null)
                {
                    if (player.HasPermission(Duty.Instance.Configuration.Instance.SuperAdminPermission))
                    {
                        p_dg = new DutyGroup
                        {
                            Group = null,
                            Permission = Duty.Instance.Configuration.Instance.SuperAdminPermission,
                            Reasons = null
                        };
                    }
                }
                if (p_dg == null)
                {
                    UnturnedChat.Say(caller, Duty.Instance.Translate("no_dutygroup"), Color.red);
                }

                string reasons = "";
                if (p_dg.Reasons != null)
                {
                    foreach (var r in p_dg.Reasons)
                    {
                        reasons += r.Name + " | ";
                    }
                    reasons += "||";
                    reasons = reasons.Replace(" | ||", string.Empty);
                }
                else
                {
                    reasons = "You do not need a reason to go on duty!";
                }
                if(p_dg.Group == null)
                {
                    UnturnedChat.Say(caller, Duty.Instance.Translate("reasons", "SUPER ADMIN" , reasons), Color.red);
                } else
                {
                    UnturnedChat.Say(caller, Duty.Instance.Translate("reasons", p_dg.Group, reasons), Color.red);
                }
            }
        }

        public string Help
        {
            get { return "Shows possible reasons for duty group that player can access."; }
        }

        public string Name
        {
            get { return "reasons"; }
        }

        public string Syntax
        {
            get { return ""; }
        }

        public bool AllowFromConsole
        {
            get { return false; }
        }

        public AllowedCaller AllowedCaller
        {
            get { return AllowedCaller.Player; }
        }

        public List<string> Aliases
        {
            get { return new List<string>() {  }; }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>() { "duty.reasons" };
            }
        }
    }
}
