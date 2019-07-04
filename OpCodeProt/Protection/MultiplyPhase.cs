using Confuser.Core;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Confuser.Protections.OpCodeProt {
	public class MultiplyPhase : ProtectionPhase {
		public MultiplyPhase(OpCodeProtection parent) : base(parent) { }

		public override ProtectionTargets Targets => ProtectionTargets.Methods;

		public override string Name => "Multiply Protection";

		protected override void Execute(ConfuserContext context, ProtectionParameters parameters) {
			foreach(MethodDef method in parameters.Targets.OfType<MethodDef>()) {
				if (method.FullName.Contains("My.")) continue;
				if (method.IsConstructor) continue;
				if (!method.HasBody) continue;
				for (int index = 0; index < method.Body.Instructions.Count; index++) {
					if (method.Body.Instructions[index].OpCode == OpCodes.Mul) {
						if (method.Body.Instructions[index - 1].IsLdcI4() && method.Body.Instructions[index - 2].IsLdcI4()) {
							var wl = method.Body.Instructions[index - 2].GetLdcI4Value();

							var val = method.Body.Instructions[index - 1].GetLdcI4Value();
							if (val >= 3) {
								Local lcl = new Local(method.Module.CorLibTypes.Int32);
								method.Body.Variables.Add(lcl);

								method.Body.Instructions.Insert(0, new Instruction(OpCodes.Stloc, lcl));
								method.Body.Instructions.Insert(0, new Instruction(OpCodes.Ldc_I4, wl));
								index += 2;

								method.Body.Instructions[index - 2].OpCode = OpCodes.Ldloc;
								method.Body.Instructions[index - 2].Operand = lcl;

								//now we have lcl * val
								method.Body.Instructions[index - 1].OpCode = OpCodes.Nop;
								method.Body.Instructions[index].OpCode = OpCodes.Nop;

								int count = 0;
								int curval = val;
								while (curval > 0) {
									// check for set bit and left  
									// shift n, count times 
									if ((curval & 1) == 1) {
										if (count != 0) {
											method.Body.Instructions.Insert(++index, new Instruction(OpCodes.Ldloc, lcl));
											method.Body.Instructions.Insert(++index, new Instruction(OpCodes.Ldc_I4, count));
											method.Body.Instructions.Insert(++index, new Instruction(OpCodes.Shl));
											method.Body.Instructions.Insert(++index, new Instruction(OpCodes.Add));
										}
									}
									count++;
									curval = curval >> 1;
								}
								if ((val & 1) == 0) {
									method.Body.Instructions.Insert(++index, new Instruction(OpCodes.Ldloc, lcl));
									method.Body.Instructions.Insert(++index, new Instruction(OpCodes.Sub));
								}
							}
						}
					}
				}
			}
		}
	}
}
