using Confuser.Core;
using Confuser.Core.Helpers;
using Confuser.Core.Services;
using Confuser.Renamer;
using Confuser.Runtime.FakeObfuscator;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confuser.Protections.FakeObuscator
{
    public class FakeObfuscatorAttributesPhase : ProtectionPhase
    {
        public override ProtectionTargets Targets => ProtectionTargets.Modules;
        public override string Name => "Fake obfuscator attribute addition";

        public FakeObfuscatorAttributesPhase(ConfuserComponent parent) : base(parent) { }

        protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
        {
            var marker = context.Registry.GetService<IMarkerService>();
            var name = context.Registry.GetService<INameService>();
            var allAddedTypes = new List<IDnlibDef>();
            var module = context.CurrentModule;

            TypeDefUser[] attributesToAdd = {
                new TypeDefUser("SecureTeam.Attributes", "ObfuscatedByCliSecureAttribute", module.CorLibTypes.GetTypeRef("System", "Attribute")),
                new TypeDefUser("SecureTeam.Attributes", "ObfuscatedByAgileDotNetAttribute", module.CorLibTypes.GetTypeRef("System", "Attribute")),
                new TypeDefUser("BabelAttribute", module.CorLibTypes.GetTypeRef("System", "Attribute")),
                new TypeDefUser("Beds-Protector-v7.0", module.CorLibTypes.GetTypeRef("System", "Attribute")),
                new TypeDefUser("CryptoObfuscator", "ProtectedWithCryptoObfuscatorAttribute", module.CorLibTypes.GetTypeRef("System", "Attribute")),
                new TypeDefUser("DotfuscatorAttribute", module.CorLibTypes.GetTypeRef("System", "Attribute")),
                new TypeDefUser("ObfuscatedByGoliath", module.CorLibTypes.GetTypeRef("System", "Attribute")),
                new TypeDefUser("MaxtoCodeAttribute", module.CorLibTypes.GetTypeRef("System", "Attribute")),
                new TypeDefUser("NETSecure", module.CorLibTypes.GetTypeRef("System", "Attribute")),
                new TypeDefUser("SmartAssembly.Attributes", "PoweredByAttribute", module.CorLibTypes.GetTypeRef("System", "Attribute")),
                new TypeDefUser("NineRays.Obfuscator", "SoftwareWatermarkAttribute", module.CorLibTypes.GetTypeRef("System", "Attribute")),
                new TypeDefUser("NineRays.Obfuscator", "Evaluation", module.CorLibTypes.GetTypeRef("System", "Attribute")),
                new TypeDefUser("VMProtect", module.CorLibTypes.GetTypeRef("System", "Attribute")),
                new TypeDefUser("Xenocode.Client.Attributes.AssemblyAttributes", "ProcessedByXenocode", module.CorLibTypes.GetTypeRef("System", "Attribute")),
                new TypeDefUser("ZYXDNGuarder", module.CorLibTypes.GetTypeRef("System", "Attribute")),
                new TypeDefUser("YanoAttribute", module.CorLibTypes.GetTypeRef("System", "Attribute"))
            };

            foreach (var m in parameters.Targets.Cast<ModuleDef>().WithProgress(context.Logger))
            {
                //inject types
                foreach (TypeDefUser idk in attributesToAdd.WithProgress(context.Logger))
                    allAddedTypes.AddRange(InjectType(m, context.Logger, idk));

                //mark types to NOT be renamed
                foreach (IDnlibDef def in allAddedTypes)
                {
                    marker.Mark(def, Parent);
                    name.MarkHelper(def, marker, Parent);
                    name.SetCanRename(def, false);
                }
            }
        }

        private IEnumerable<IDnlibDef> InjectType(ModuleDef m, Core.ILogger l, params TypeDefUser[] types)
        {
            List<IDnlibDef> ret = new List<IDnlibDef>();

            foreach (TypeDefUser type in types)
            {
                m.Types.Add(type);
                l.Debug("Added attribute " + type);

                ret.AddRange(InjectHelper.Inject(type, type, m));
            }

            return ret;
        }
    }
}
