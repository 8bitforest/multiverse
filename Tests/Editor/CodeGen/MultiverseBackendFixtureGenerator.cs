using System;
using System.Linq;
using Mono.CecilX;
using Mono.CecilX.Cil;
using Multiverse.Tests.Backend;
using Multiverse.Tests.Backend.Utils;
using NUnit.Framework;

namespace Tests.Editor.CodeGen
{
    internal class MultiverseBackendFixtureGenerator
    {
        private readonly ILPostProcessorLogger _log;

        public MultiverseBackendFixtureGenerator(ILPostProcessorLogger log)
        {
            _log = log;
        }

        public void GenerateFixtures(AssemblyDefinition assembly)
        {
            var assemblyTypes = assembly.Modules.SelectMany(m => m.Types);
            var testSuites = assemblyTypes
                .Where(t => t.Interfaces.Any(i => i.InterfaceType.FullName == typeof(IMvLibraryTestSuite).FullName))
                .Select(l => l).ToList();

            var allTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes());
            var backendFixtures = allTypes.Where(t
                => t.CustomAttributes.Any(a
                    => a.AttributeType.FullName == typeof(MultiverseBackendFixtureAttribute).FullName)).ToList();

            var module = assembly.MainModule;
            var testFixtureRef = module.ImportReference(typeof(TestFixtureAttribute).GetConstructor(Type.EmptyTypes));
            var setUpFixtureRef = module.ImportReference(typeof(SetUpFixtureAttribute).GetConstructor(Type.EmptyTypes));
            var multiverseTestSetUpRef = module.ImportReference(typeof(MultiverseTestSetUp<>));

            foreach (var testSuite in testSuites)
            {
                _log.LogWarning($"Generating fixtures for {testSuite.Name}");
                var testSuiteNamespace = $"{testSuite.Namespace}.GeneratedSuites.{testSuite.Name}";

                var setUp = new TypeDefinition(testSuiteNamespace,
                    $"{testSuite.Name}SetUp",
                    TypeAttributes.Class | TypeAttributes.Public,
                    new GenericInstanceType(multiverseTestSetUpRef)
                    {
                        GenericArguments = {testSuite}
                    }
                );
                setUp.CustomAttributes.Add(new CustomAttribute(setUpFixtureRef));
                AddEmptyConstructor(setUp, module);
                module.Types.Add(setUp);

                foreach (var backendFixture in backendFixtures)
                {
                    var fixture = new TypeDefinition(
                        testSuiteNamespace +
                        backendFixture.Namespace!.Replace(typeof(IMvLibraryTestSuite).Namespace!, ""),
                        backendFixture.Name,
                        TypeAttributes.Class | TypeAttributes.Public,
                        module.ImportReference(backendFixture)
                    );
                    fixture.CustomAttributes.Add(new CustomAttribute(testFixtureRef));
                    AddEmptyConstructor(fixture, module);
                    module.Types.Add(fixture);
                }
            }
        }

        private void AddEmptyConstructor(TypeDefinition type, ModuleDefinition module)
        {
            var methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName |
                                   MethodAttributes.RTSpecialName;
            var method = new MethodDefinition(".ctor", methodAttributes, module.ImportReference(typeof(void)));
            method.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
            type.Methods.Add(method);
        }
    }
}