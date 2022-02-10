using System;
using System.IO;
using System.Linq;
using Mono.CecilX;
using Mono.CecilX.Cil;
using Multiverse.Tests.Backend;
using Unity.CompilationPipeline.Common.ILPostProcessing;

namespace Tests.Editor.CodeGen
{
    public class MultiverseTestILPostProcessor : ILPostProcessor
    {
        private readonly ILPostProcessorLogger _log = new ILPostProcessorLogger();

        public override ILPostProcessor GetInstance() => this;

        public override bool WillProcess(ICompiledAssembly compiledAssembly)
        {
            using var stream = new MemoryStream(compiledAssembly.InMemoryAssembly.PeData);
            using var def = AssemblyDefinition.ReadAssembly(stream);
            return def.Modules.SelectMany(m => m.Types).Any(t
                => t.Interfaces.Any(i => i.InterfaceType.FullName == typeof(IMvLibraryTestSuite).FullName));
        }

        public override ILPostProcessResult Process(ICompiledAssembly compiledAssembly)
        {
            _log.LogWarning($"Processing {compiledAssembly.Name}");

            using var stream = new MemoryStream(compiledAssembly.InMemoryAssembly.PeData);
            using var symbols = new MemoryStream(compiledAssembly.InMemoryAssembly.PdbData);

            using var assembly = AssemblyDefinition.ReadAssembly(stream, new ReaderParameters
            {
                ReadWrite = true,
                SymbolStream = symbols,
                ReadSymbols = true,
            });

            try
            {
                new MultiverseBackendFixtureGenerator(_log).GenerateFixtures(assembly);
            }
            catch (Exception e)
            {
                _log.LogError($"{e.Message} Stacktrace: {e.StackTrace}");
            }

            if (_log.HasErrors())
                return new ILPostProcessResult(compiledAssembly.InMemoryAssembly, _log.GetLogList());

            var peOut = new MemoryStream();
            var pdbOut = new MemoryStream();

            assembly.Write(peOut, new WriterParameters
            {
                SymbolWriterProvider = new PortablePdbWriterProvider(),
                SymbolStream = pdbOut,
                WriteSymbols = true
            });

            var inMemory = new InMemoryAssembly(peOut.ToArray(), pdbOut.ToArray());
            return new ILPostProcessResult(inMemory, _log.GetLogList());
        }
    }
}