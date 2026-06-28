using System;
using System.Collections.Generic;
using System.Reflection;
using Audio;
using BepInEx;
using BepInEx.Configuration;
using FMOD.Studio;
using HarmonyLib;
using Radio;

[BepInPlugin("ifz.squadvoicevolume", "Infection Free Zone - Squad Voice Volume", "1.0.0")]
public class IFZSquadVoiceVolumePlugin : BaseUnityPlugin
{
    private static ConfigEntry<bool> Enabled;
    private static ConfigEntry<float> ChooseVolume;
    private static ConfigEntry<float> GoVolume;
    private static ConfigEntry<float> AttackVolume;
    private static ConfigEntry<float> BuildingClearVolume;
    private static ConfigEntry<bool> DebugLog;

    private static BepInEx.Logging.ManualLogSource Log;
    private static readonly Random Rng = new Random();

    private static FieldInfo SoundsDictField;
    private static FieldInfo CallbackField;
    private static FieldInfo RadioMessageIdField;
    private static PropertyInfo RadioMessageIdProperty;

    private void Awake()
    {
        Log = Logger;

        Enabled = Config.Bind("General", "Enabled", true, "Enable/disable the plugin.");
        ChooseVolume = Config.Bind("Volume", "ChooseVolume", 0.10f, "Volume for rvoice_squad_choose. 1.0 = original, 0.10 = 10%.");
        GoVolume = Config.Bind("Volume", "GoVolume", 0.10f, "Volume for rvoice_squad_go. 1.0 = original, 0.10 = 10%.");
        AttackVolume = Config.Bind("Volume", "AttackVolume", 1.00f, "Volume for rvoice_squad_attack.");
        BuildingClearVolume = Config.Bind("Volume", "BuildingClearVolume", 1.00f, "Volume for rvoice_squad_building_clear.");
        DebugLog = Config.Bind("Debug", "DebugLog", false, "Keep false for best performance.");

        SoundsDictField = typeof(FmodRvoicePlayer).GetField("_helpfulDictWithRvoicesDividedByRadioMsg", BindingFlags.Instance | BindingFlags.NonPublic);
        CallbackField = typeof(FmodRvoicePlayer).GetField("_rvoiceCallback", BindingFlags.Instance | BindingFlags.NonPublic);
        RadioMessageIdField = typeof(RadioMessage).GetField("id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        RadioMessageIdProperty = typeof(RadioMessage).GetProperty("id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (SoundsDictField == null || CallbackField == null)
        {
            Logger.LogError("Required FmodRvoicePlayer private fields not found. Mod disabled.");
            return;
        }

        Harmony harmony = new Harmony("ifz.squadvoicevolume");
        MethodInfo target = AccessTools.Method(typeof(FmodRvoicePlayer), "PlayRvoice");
        MethodInfo prefix = AccessTools.Method(typeof(IFZSquadVoiceVolumePlugin), "PlayRvoicePrefix");

        if (target == null || prefix == null)
        {
            Logger.LogError("PlayRvoice patch target/prefix not found. Mod disabled.");
            return;
        }

        harmony.Patch(target, new HarmonyMethod(prefix));
        Logger.LogInfo("Infection Free Zone - Squad Voice Volume v1.0.0 loaded. Direct PlayRvoice hook active.");
        Logger.LogInfo("Volumes: choose=" + ChooseVolume.Value + ", go=" + GoVolume.Value + ", attack=" + AttackVolume.Value + ", clear=" + BuildingClearVolume.Value);
    }

    private static bool PlayRvoicePrefix(FmodRvoicePlayer __instance, RadioMessage radioMessage, ref bool silent, ref bool shouldPlayFirstSound, ref FmodSoundInfo __result)
    {
        if (!Enabled.Value || radioMessage == null)
        {
            return true;
        }

        string msgId = GetRadioMessageId(radioMessage);
        float volume = GetVolumeForMessage(msgId);

        if (volume >= 0.999f)
        {
            return true;
        }

        try
        {
            Dictionary<RadioMessage, List<FmodSoundInfo>> soundsByMessage = SoundsDictField.GetValue(__instance) as Dictionary<RadioMessage, List<FmodSoundInfo>>;
            if (soundsByMessage == null)
            {
                return true;
            }

            List<FmodSoundInfo> sounds;
            if (!soundsByMessage.TryGetValue(radioMessage, out sounds) || sounds == null || sounds.Count == 0)
            {
                return true;
            }

            FmodSoundInfo selectedSound = shouldPlayFirstSound ? sounds[0] : sounds[Rng.Next(sounds.Count)];

            if (!silent)
            {
                EventInstance eventInstance = FmodHelper.CreateEventInstance("Transmissions/Rvoice");
                eventInstance.setVolume(volume);

                EVENT_CALLBACK callback = (EVENT_CALLBACK)CallbackField.GetValue(__instance);
                FmodHelper.PlayFromAudioTable(selectedSound.Id, eventInstance, callback);

                if (DebugLog.Value)
                {
                    Log.LogInfo("Adjusted " + msgId + " / " + selectedSound.Id + " -> volume " + volume);
                }
            }

            __result = selectedSound;
            return false;
        }
        catch (Exception ex)
        {
            if (DebugLog.Value)
            {
                Log.LogWarning("Patch failed for " + msgId + ": " + ex.GetType().Name + " - " + ex.Message + ". Falling back to original.");
            }
            return true;
        }
    }

    private static string GetRadioMessageId(RadioMessage radioMessage)
    {
        try
        {
            if (RadioMessageIdField != null)
            {
                object value = RadioMessageIdField.GetValue(radioMessage);
                return value as string;
            }

            if (RadioMessageIdProperty != null)
            {
                object value = RadioMessageIdProperty.GetValue(radioMessage, null);
                return value as string;
            }
        }
        catch
        {
        }

        return null;
    }

    private static float GetVolumeForMessage(string id)
    {
        if (string.IsNullOrEmpty(id)) return 1.0f;
        if (id == "rvoice_squad_choose") return ChooseVolume.Value;
        if (id == "rvoice_squad_go") return GoVolume.Value;
        if (id == "rvoice_squad_attack") return AttackVolume.Value;
        if (id == "rvoice_squad_building_clear") return BuildingClearVolume.Value;
        return 1.0f;
    }
}
