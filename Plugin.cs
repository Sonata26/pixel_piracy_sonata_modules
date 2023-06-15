using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;


namespace Sonata_Modules
{
  [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
  public class Plugin : BaseUnityPlugin
  {
    private const string PLUGIN_GUID = "com.SonataModules";
    private const string PLUGIN_NAME = "Sonata Modules";
    private const string PLUGIN_VERSION = "1.0.0.0";

    private static bool patched = false;
    private static readonly Harmony harmony = new Harmony(PLUGIN_GUID);

    private void Awake()
    {
      // Plugin startup logic
      Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void DoPatch()
    {
      harmony.PatchAll();
      patched = true;
      Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} patches applied!");
    }

    private void OnLevelWasLoaded(int level)
    {
      if (!patched)
      {
        DoPatch();
      }
    }
  }


  [HarmonyDebug]
  [HarmonyPatch(typeof(FishingTask), "Logic")]
  class FishAnywherePatch
  {
    // static void Prefix(Brain brain, bool enemy_ship)
    // {
    //   Debug.Log("Prefix fishing test");
    // }

    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
      var found = false;
      var code = new List<CodeInstruction>(instructions);

      // MethodInfo raycastInfo = AccessTools.Method(typeof(Physics), nameof(Physics.Raycast));

      for (int i = 0; i < code.Count; i++)
      {
        var instruction = code[i];

        if (found == false && instruction.opcode == OpCodes.Ldc_R4 && instruction.OperandIs(float.PositiveInfinity))
        {
          found = true;

          yield return new CodeInstruction(OpCodes.Ldc_R4, 1f);
        }
        else
        {
          yield return instruction;
        }
      }

      if (found is false)
      {
        Debug.Log("Unable to find Raycast check in Fishing.Logic, skipping fishing patch.");
        //throw new ArgumentException("Cannot find Raycast check in Fishing.Logic");
      }
    }
  }
}
