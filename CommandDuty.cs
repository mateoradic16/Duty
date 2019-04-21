using Rocket.Unturned.Player;
using Rocket.API;
using System.Collections.Generic;
using Rocket.Unturned.Chat;
using UnityEngine;
using Rocket.Core.Logging;

namespace EFG.Duty
{
    public class CommandDuty : IRocketCommand
    {
        public void Execute(IRocketPlayer caller, string[] command)
        {
            bool c = true;
            UnturnedPlayer player = (UnturnedPlayer)caller;
            foreach(var pod in Duty.Instance.OnDuty_Players)
            {
                Rocket.Core.Logging.Logger.LogWarning(pod.Player.DisplayName);
                if(pod.Player.CSteamID == player.CSteamID)
                {
                    Rocket.Core.Logging.Logger.LogWarning(caller.DisplayName);
                    Duty.Instance.duty(player, null, null);
                    c = false;
                }
            }
            if(c)
            {
                if (command.Length == 0)
                {
                    command = new string[] { "" };
                }
                DutyGroup p_dg = null;
                Reason p_r = null;
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
                else
                {
                    if(p_dg.Reasons!=null)
                    {
                        p_dg.Reasons.ForEach((r) =>
                        {
                            if (r.Name == command[0])
                            {
                                p_r = r;
                            }
                        });
                    } else
                    {
                        p_r = new Reason
                        {
                            Name = "sadmin",
                            Translate_Id = "on_duty_message"
                        };
                    }
                }

                
                if (p_r == null)
                {
                    string reasons = "";
                    foreach (var r in p_dg.Reasons)
                    {
                        reasons += r.Name + " | ";
                    }
                    reasons += "||";
                    reasons = reasons.Replace(" | ||", string.Empty);
                    UnturnedChat.Say(caller, Duty.Instance.Translate("no_reason", reasons), Color.red);
                }
                else
                {
                    Duty.Instance.duty(player, p_r, p_dg);
                }
            }
        }

        public string Help
        {
            get { return "Gives admin powers to the player without the need of the console."; }
        }

        public string Name
        {
            get { return "duty"; }
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
            get { return new List<string>() { "d" }; }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>() { "duty.duty" };
            }
        }
    }
}
