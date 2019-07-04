using Confuser.Core;
using Confuser.Protections.OpCodeProt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Confuser.Protections {
	[AfterProtection("Ki.ControlFlow")]
	public class OpCodeProtection : Protection {
		public override ProtectionPreset Preset => ProtectionPreset.Aggressive;

		public override string Name => "OpCode Protection";

		public override string Description => "Protects OpCodes such as Ldlfd.";

		public override string Author => "Confuser";

		public override string Id => "opcode prot";

		public override string FullId => "Confuser.OpCode";

		protected override void Initialize(ConfuserContext context) { }

		protected override void PopulatePipeline(ProtectionPipeline pipeline) {
			pipeline.InsertPreStage(PipelineStage.OptimizeMethods, new LdfldPhase(this));
			pipeline.InsertPreStage(PipelineStage.OptimizeMethods, new CallvirtPhase(this));
			pipeline.InsertPreStage(PipelineStage.OptimizeMethods, new CtorCallProtection(this));
			pipeline.InsertPreStage(PipelineStage.OptimizeMethods, new MultiplyPhase(this));
		}
	}
}
