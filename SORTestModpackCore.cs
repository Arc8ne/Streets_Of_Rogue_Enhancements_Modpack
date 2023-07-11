using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using HarmonyLib;

namespace SOR_Test_Modpack
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class SORTestModpackCore : BaseUnityPlugin
    {
        public const string pluginGuid = "streets.of.rogue.test.modpack";

        public const string pluginName = "Streets Of Rogue Test Modpack";

        public const string pluginVersion = "0.1.0";

        private List<ISORModpackModule> activatedModules = new List<ISORModpackModule>();

        public static SORTestModpackCore instance = null;

		public Harmony harmony = new Harmony(pluginGuid);

        private void ActivateModules()
        {
            /*
            this.activatedModules.Add(
                CustomizeableInventorySpaceModule.instance
            );
            */

            this.activatedModules.Add(
                NoKnockbackModule.instance
            );

            this.activatedModules.Add(
                HoldAndShootModule.instance
            );
        }

        private void InitActivatedModules()
        {
			foreach (ISORModpackModule activatedModule in activatedModules)
			{
				activatedModule.Init();
			}
		}

		// TODO (Low priority): Implement customizable knockback module.
		public void Awake()
        {
            instance = this;

            harmony.PatchAll();

            this.ActivateModules();

            this.InitActivatedModules();

            this.LogInfo(pluginName + " loaded successfully.");
        }

        public void LogInfo(string msg)
        {
            Logger.LogInfo(msg);
        }
    }
}
