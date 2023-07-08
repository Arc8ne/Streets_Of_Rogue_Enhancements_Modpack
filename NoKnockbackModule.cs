using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOR_Test_Modpack
{
	public class NoKnockbackModule : ISORModpackModule
	{
		public static NoKnockbackModule instance = new NoKnockbackModule();

		public NoKnockbackModule()
		{

		}

		public void Init()
		{
			SORTestModpackCore.instance.harmony.Patch(
				original: AccessTools.Method(
					typeof(Movement),
					nameof(Movement.FindKnockBackStrength)
				),
				prefix: new HarmonyMethod(
					AccessTools.Method(
						typeof(NoKnockbackModule),
						nameof(Movement_FindKnockBackStrength_PrefixPatch)
					)
				)
			);
		}

		public static bool Movement_FindKnockBackStrength_PrefixPatch(float strength, ref float __result)
		{
			__result = 0;

			return false;
		}
	}
}
