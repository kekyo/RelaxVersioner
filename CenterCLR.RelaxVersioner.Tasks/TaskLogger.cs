/////////////////////////////////////////////////////////////////////////////////////////////////
//
// CenterCLR.RelaxVersioner - Easy-usage, Git-based, auto-generate version informations toolset.
// Copyright (c) 2016-2019 Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//	http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
/////////////////////////////////////////////////////////////////////////////////////////////////

using Microsoft.Build.Framework;

namespace CenterCLR.RelaxVersioner
{
    internal sealed class TaskLogger : Logger
    {
        private readonly IBuildEngine engine;

        public TaskLogger(IBuildEngine engine) :
            base($"RelaxVersioner[{AssemblyLoadHelper.EnvironmentIdentifier}]") =>
            this.engine = engine;

        public override void Message(LogImportance importance, string message)
        {
            switch (importance)
            {
                case LogImportance.Low:
                    engine.LogMessageEvent(new BuildMessageEventArgs(message, null, "RelaxVersionerTask", MessageImportance.Low));
                    break;
                case LogImportance.Normal:
                    engine.LogMessageEvent(new BuildMessageEventArgs(message, null, "RelaxVersionerTask", MessageImportance.Normal));
                    break;
                default:
                    engine.LogMessageEvent(new BuildMessageEventArgs(message, null, "RelaxVersionerTask", MessageImportance.High));
                    break;
            }
        }

        public override void Warning(string message) =>
            engine.LogWarningEvent(new BuildWarningEventArgs(
                null, null,
                engine.ProjectFileOfTaskNode, engine.LineNumberOfTaskNode, engine.ColumnNumberOfTaskNode, 0, 0,
                message, null, "RelaxVersionerTask"));

        public override void Error(string message) =>
            engine.LogErrorEvent(new BuildErrorEventArgs(
                null, null,
                engine.ProjectFileOfTaskNode, engine.LineNumberOfTaskNode, engine.ColumnNumberOfTaskNode, 0, 0,
                message, null, "RelaxVersionerTask"));
    }
}
