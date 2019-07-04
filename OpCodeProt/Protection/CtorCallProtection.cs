using Confuser.Core;
using Confuser.Core.Services;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System.Linq;

namespace Confuser.Protections.OpCodeProt {
	public class CtorCallProtection : ProtectionPhase {
		public CtorCallProtection(OpCodeProtection parent) : base(parent) { }

		public override ProtectionTargets Targets => ProtectionTargets.Types;

		public override string Name => "Ctor Call Protection";

		protected override void Execute(ConfuserContext context, ProtectionParameters parameters) {
			var random = context.Registry.GetService<IRandomService>().GetRandomGenerator(Parent.FullId + ".CtorCall");
			foreach (TypeDef type in parameters.Targets.OfType<TypeDef>()) {
				if (type.InGlobalModuleType()) continue;
				foreach (MethodDef method in type.Methods) {
					if (method.InGlobalModuleType()) continue;
					if (method.FullName.Contains("My.")) continue;
					if (method.IsConstructor) continue;
					if (!method.HasBody) continue;
					for (int i = 0; i < method.Body.Instructions.Count; i++) {
						if (method.Body.Instructions[i].OpCode == OpCodes.Call) {
							if (method.Body.Instructions[i].Operand.ToString().ToLower().Contains("void")) {
								if ((i - 1) > 0 && method.Body.Instructions[i - 1].IsLdarg()) {
									Local new_local = new Local(method.Module.CorLibTypes.Int32);
									method.Body.Variables.Add(new_local);

									method.Body.Instructions.Insert(i - 1, OpCodes.Ldc_I4.ToInstruction(random.NextInt32()));
									method.Body.Instructions.Insert(i, OpCodes.Stloc_S.ToInstruction(new_local));
									method.Body.Instructions.Insert(i + 1, OpCodes.Ldloc_S.ToInstruction(new_local));
									method.Body.Instructions.Insert(i + 2, OpCodes.Ldc_I4.ToInstruction(random.NextInt32()));
									method.Body.Instructions.Insert(i + 3, OpCodes.Ldarg_0.ToInstruction());
									method.Body.Instructions.Insert(i + 4, OpCodes.Nop.ToInstruction());
									method.Body.Instructions.Insert(i + 6, OpCodes.Nop.ToInstruction());
									method.Body.Instructions.Insert(i + 3, new Instruction(OpCodes.Bne_Un_S, method.Body.Instructions[i + 4]));
									method.Body.Instructions.Insert(i + 5, new Instruction(OpCodes.Br_S, method.Body.Instructions[i + 8]));
									method.Body.Instructions.Insert(i + 8, new Instruction(OpCodes.Br_S, method.Body.Instructions[i + 9]));
								}
							}
						}
					}
				}
			}
		}
	}
}
