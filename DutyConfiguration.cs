using Rocket.API;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace EFG.Duty
{
    public class DutyConfiguration : IRocketPluginConfiguration
    {
        public bool EnableServerAnnouncer;
        public bool RemoveAdminDutyOnlogout;
        public bool AllowDutyCheck;
        public string MessageColor;
        public string SuperAdminPermission;

        public List<DutyGroup> DutyGroups;

        public void LoadDefaults()
        {
            EnableServerAnnouncer = true;
            RemoveAdminDutyOnlogout = true;
            AllowDutyCheck = true;
            MessageColor = "red";
            SuperAdminPermission = "duty.superadmin";

            DutyGroups = new List<DutyGroup>
            {
                new DutyGroup
                {
                    Group = "helper",
                    Permission = "duty.helper",
                    Reasons = new List<Reason>
                    {
                        new Reason
                        {
                            Name = "",
                            Translate_Id = "empty_reason"
                        },
                        new Reason
                        {
                            Name = "glitch_base",
                            Translate_Id = "g_base"
                        },
                        new Reason
                        {
                            Name = "hacker",
                            Translate_Id = "hacker"
                        }
                    }
                },
                new DutyGroup
                {
                    Group = "mod",
                    Permission = "duty.mod",
                    Reasons = new List<Reason>
                    {
                        new Reason
                        {
                            Name = "",
                            Translate_Id = "empty_reason"
                        },
                        new Reason
                        {
                            Name = "glitch_base",
                            Translate_Id = "g_base"
                        },
                        new Reason
                        {
                            Name = "hacker",
                            Translate_Id = "hacker"
                        }
                    }
                }
            };


        }
    }

    public class DutyGroup
    {
        public string Group;
        public string Permission;
        public List<Reason> Reasons;
    }

    public class Reason
    {
        [XmlAttribute]
        public string Name;
        [XmlAttribute]
        public string Translate_Id;
    }
}
