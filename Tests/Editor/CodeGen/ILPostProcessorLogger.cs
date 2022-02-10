using System.Collections.Generic;
using System.Linq;
using Unity.CompilationPipeline.Common.Diagnostics;

namespace Tests.Editor.CodeGen
{
    internal class ILPostProcessorLogger
    {
        private readonly List<DiagnosticMessage> _logs = new List<DiagnosticMessage>();

        public void LogWarning(string message)
        {
            _logs.Add(new DiagnosticMessage
            {
                DiagnosticType = DiagnosticType.Warning,
                MessageData = $"[MVIL] {message}"
            });
        }
        
        public void LogError(string message)
        {
            _logs.Add(new DiagnosticMessage
            {
                DiagnosticType = DiagnosticType.Error,
                MessageData = $"[MVIL] {message}"
            });
        }

        public bool HasErrors()
        {
            return _logs.Any(l => l.DiagnosticType == DiagnosticType.Error);
        }

        public List<DiagnosticMessage> GetLogList()
        {
            return _logs.ToList();
        }
    }
}