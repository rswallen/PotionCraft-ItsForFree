using BepInEx.Configuration;
using QFSW.QC;

namespace ItsForFree
{
    [CommandPrefix("ItsForFree-")]
    internal static class Settings
    {
        internal static ConfigFile Config => ItsForFreePlugin.Config;

        private static bool initialised = false;

        private static ConfigEntry<float> multiplier;
        private static ConfigEntry<int> additive;

        public static float Multiplier
        {
            get => multiplier.Value;
            set
            {
                if (multiplier.Value != value)
                {
                    multiplier.Value = value;
                }
            }
        }

        public static int Additive
        {
            get => additive.Value;
            set
            {
                if (additive.Value != value)
                {
                    additive.Value = value;
                }
            }
        }

        public static void Initialize()
        {
            if (!initialised)
            {
                var multDesc = new ConfigDescription("Multipler for popularity when giving potion away", new AcceptableValueRange<float>(1f, float.MaxValue));
                multiplier = Config.Bind("General", "Popularity Multiplier", 2f, multDesc);

                var addDesc = new ConfigDescription("Additional popularity to add after multipler is applied", new AcceptableValueRange<int>(0, int.MaxValue));
                additive = Config.Bind("General", "Popularity Additive", 0, addDesc);

                Config.SettingChanged += DialogueBoxHook.ConfigUpdated;

                initialised = true;
                Config.Reload();
            }
        }

        [Command("Multiplier", "Updates the popularity multiplier", true, true, Platform.AllPlatforms, MonoTargetType.Single)]
        private static void SetMultiplier(float multiplier)
        {
            Multiplier = multiplier;
        }

        [Command("Additive", "Updates the popularity additive", true, true, Platform.AllPlatforms, MonoTargetType.Single)]
        private static void SetAdditive(int additive)
        {
            Additive = additive;
        }
    }
}
