using Rocket.Unturned.Player;
using Rocket.Unturned.Chat;
using Rocket.Unturned;
using Rocket.Core.Plugins;
using Rocket.API.Collections;
using UnityEngine;
using Rocket.API;
using System.Collections.Generic;
using Rocket.Core;

namespace EFG.Duty
{
    public class Duty : RocketPlugin<DutyConfiguration>
    {
        public static Duty Instance;

        protected override void Load()
        {
            Instance = this;

            Rocket.Core.Logging.Logger.LogWarning("Loading event \"Player Connected\"...");
            U.Events.OnPlayerConnected += PlayerConnected;
            Rocket.Core.Logging.Logger.LogWarning("Loading event \"Player Disconnected\"...");
            U.Events.OnPlayerDisconnected += PlayerDisconnected;

            Rocket.Core.Logging.Logger.LogWarning("");
            Rocket.Core.Logging.Logger.LogWarning("Duty has been successfully loaded!");
        }
        
        protected override void Unload()
        {
            Instance = null;

            Rocket.Core.Logging.Logger.LogWarning("Unloading on player connect event...");
            U.Events.OnPlayerConnected -= PlayerConnected;
            Rocket.Core.Logging.Logger.LogWarning("Unloading on player disconnect event...");
            U.Events.OnPlayerConnected -= PlayerDisconnected;

            Rocket.Core.Logging.Logger.LogWarning("");
            Rocket.Core.Logging.Logger.LogWarning("Duty has been unloaded!");
        }

        public class onDuty
        {
            public UnturnedPlayer Player;
            public Reason Reason;
            public DutyGroup DutyGroup;
        }

        public List<onDuty> OnDuty_Players = new List<onDuty> { };

        public void duty(UnturnedPlayer caller, Reason r, DutyGroup dg)
        {
            bool c = true;
            foreach(var pod in OnDuty_Players)
            {
                if (pod.Player.CSteamID == caller.CSteamID)
                {
                    c = false;
                    if(pod.DutyGroup.Group == null)
                    {
                        caller.Admin(false);
                    } else
                    {
                        R.Permissions.RemovePlayerFromGroup(pod.DutyGroup.Group, caller);
                    }
                    OnDuty_Players.Remove(pod);
                    if (Configuration.Instance.EnableServerAnnouncer)
                    {
                        UnturnedChat.Say(Translate("off_duty_message", caller.DisplayName), UnturnedChat.GetColorFromName(Configuration.Instance.MessageColor, Color.blue));
                    }
                }
                break;
            }
            if(c)
            {
                if(r!=null&&dg!=null)
                {
                    if(dg.Group == null)
                    {
                        caller.Admin(true);
                        if (Configuration.Instance.EnableServerAnnouncer)
                        {
                            UnturnedChat.Say(Translate("on_duty_message", caller.DisplayName), UnturnedChat.GetColorFromName(Configuration.Instance.MessageColor, Color.blue));
                        }
                        
                    }
                    else
                    {
                        R.Permissions.AddPlayerToGroup(dg.Group, caller);
                        if (Configuration.Instance.EnableServerAnnouncer)
                        {
                            UnturnedChat.Say(Translate(r.Translate_Id, caller.DisplayName), UnturnedChat.GetColorFromName(Configuration.Instance.MessageColor, Color.blue));
                        }
                        
                    }
                    
                    OnDuty_Players.Add(new onDuty
                    {
                        Player = caller,
                        DutyGroup = dg,
                        Reason = r
                    });
                }
            }
        }

        public void cduty(UnturnedPlayer cplayer, IRocketPlayer caller)
        {
            if (cplayer != null)
            {
                bool od = false;

                foreach(var pod in OnDuty_Players)
                {
                    if(pod.Player.CSteamID == cplayer.CSteamID)
                    {
                        od = true;
                    }
                }

                if (od)
                {
                    if (Configuration.Instance.AllowDutyCheck)
                    {
                        if (caller is ConsolePlayer)
                        {
                            UnturnedChat.Say(Instance.Translate("check_on_duty_message", "Console", cplayer.DisplayName), UnturnedChat.GetColorFromName(Instance.Configuration.Instance.MessageColor, Color.red));
                        }
                        else if (caller is UnturnedPlayer)
                        {
                            UnturnedChat.Say(Instance.Translate("check_on_duty_message", caller.DisplayName, cplayer.DisplayName), UnturnedChat.GetColorFromName(Instance.Configuration.Instance.MessageColor, Color.red));
                        }
                    }
                    else if (Configuration.Instance.AllowDutyCheck == false)
                    {
                        if (caller is UnturnedPlayer)
                        {
                            UnturnedChat.Say(caller, "Unable To Check Duty. Configuration Is Set To Be Disabled.");
                        }
                    }
                }
                else
                {
                    if (Configuration.Instance.AllowDutyCheck)
                    {
                        if (caller is ConsolePlayer)
                        {
                            UnturnedChat.Say(Instance.Translate("check_off_duty_message", "Console", cplayer.DisplayName), UnturnedChat.GetColorFromName(Instance.Configuration.Instance.MessageColor, Color.red));
                        }
                        else if (caller is UnturnedPlayer)
                        {
                            UnturnedChat.Say(Instance.Translate("check_off_duty_message", caller.DisplayName, cplayer.DisplayName), UnturnedChat.GetColorFromName(Instance.Configuration.Instance.MessageColor, Color.red));
                        }
                    }
                    else if (Configuration.Instance.AllowDutyCheck == false)
                    {
                        if (caller is UnturnedPlayer)
                        {
                            UnturnedChat.Say(caller, "Unable To Check Duty. Configuration Is Set To Be Disabled.");
                        }
                    }
                }
            }
            else if (cplayer == null)
            {
                Rocket.Core.Logging.Logger.LogWarning("Duty Debug: Player is not online or his name is invalid.");
                if (caller is UnturnedPlayer)
                {
                    UnturnedChat.Say(caller, "Player is not online or his name is invalid.");
                }
            }
        }

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList {
                    {"admin_login_message", "{0} has logged on and is now on duty."},
                    {"admin_logoff_message", "{0} has logged off and is now off duty."},
                    {"on_duty_message", "{0} is now on duty."},
                    {"off_duty_message", "{0} is now off duty."},
                    {"check_on_duty_message", "{0} has confirmed that {1} is on duty."},
                    {"check_off_duty_message", "{0} has confirmed that {1} is not on duty."},
                    {"cannot_be_in_duty", "{0} is not staff member and do not have permissions to go on duty!" },
                    {"reasons", "Reasons for Group {0} are: [ {1} ]" },
                    {"no_reason", "You must specify a reason! [ {0} ]" },
                    {"no_dutygroup", "You do not have permission for any duty group!" },
                    {"empty_reason", "{0} is now on duty for no reason!" },
                    {"g_base", "{0} is now on duty for punishing some glitchers :P" },
                    {"hacker", "{0} is now on duty for punishing some hackers :P" }
                };
                    
            }
        }
        void PlayerConnected(UnturnedPlayer player)
        {
            foreach(var pod in OnDuty_Players)
            {
                if(pod.Player.CSteamID == player.CSteamID)
                {
                    pod.Player = player;
                    if(pod.DutyGroup == null)
                    {
                        player.Admin(true);
                        if (Configuration.Instance.EnableServerAnnouncer)
                        {
                            UnturnedChat.Say(Translate("admin_login_message", player.DisplayName), UnturnedChat.GetColorFromName(Configuration.Instance.MessageColor, Color.blue));
                        }
                    }
                }
            }
        }
        void PlayerDisconnected(UnturnedPlayer player)
        {
            player.Admin(false);
            foreach (var pod in OnDuty_Players)
            {
                if (pod.Player.CSteamID == player.CSteamID)
                {
                    if (pod.DutyGroup == null)
                    {
                        if(Configuration.Instance.RemoveAdminDutyOnlogout == true)
                        {
                            OnDuty_Players.Remove(pod);
                            if (Configuration.Instance.EnableServerAnnouncer)
                            {
                                UnturnedChat.Say(Translate("admin_logoff_message", player.DisplayName), UnturnedChat.GetColorFromName(Configuration.Instance.MessageColor, Color.blue));
                            }
                        }
                    }
                    else
                    {
                        R.Permissions.RemovePlayerFromGroup(pod.DutyGroup.Group, player);
                        if(Configuration.Instance.EnableServerAnnouncer)
                        {
                            UnturnedChat.Say(Translate("admin_logoff_message", player.DisplayName), UnturnedChat.GetColorFromName(Configuration.Instance.MessageColor, Color.blue));
                        }
                        
                        OnDuty_Players.Remove(pod);
                    }
                }
            }
        }
    }
}
