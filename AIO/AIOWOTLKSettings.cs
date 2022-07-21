using robotManager.Helpful;
using System;
using System.IO;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

namespace WholesomeWOTLKAIO
{
    [Serializable]
    public class AIOWOTLKSettings : Settings
    {
        public static AIOWOTLKSettings CurrentSetting { get; set; }

        private AIOWOTLKSettings()
        {
            LastUpdateDate = 0;
        }

        public double LastUpdateDate { get; set; }

        public bool Save()
        {
            try
            {
                return Save(AdviserFilePathAndName("AIOWOTLKSettings",
                    ObjectManager.Me.Name + "." + Usefuls.RealmName));
            }
            catch (Exception e)
            {
                Logging.Write("AIOWOTLKSettings > Save(): " + e);
                return false;
            }
        }

        public static bool Load()
        {
            try
            {
                if (File.Exists(AdviserFilePathAndName("AIOWOTLKSettings",
                    ObjectManager.Me.Name + "." + Usefuls.RealmName)))
                {
                    CurrentSetting = Load<AIOWOTLKSettings>(
                        AdviserFilePathAndName("AIOWOTLKSettings",
                        ObjectManager.Me.Name + "." + Usefuls.RealmName));
                    return true;
                }
                CurrentSetting = new AIOWOTLKSettings();
            }
            catch (Exception e)
            {
                Logging.WriteError("AIOWOTLKSettings > Load(): " + e);
            }
            return false;
        }
    }
}