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
using Microsoft.Build.Utilities;

namespace CenterCLR.RelaxVersioner
{
    internal sealed class TaskLogger : Logger
    {
        private readonly TaskLoggingHelper logger;

        public TaskLogger(TaskLoggingHelper logger) :
            base($"RelaxVersioner[{AssemblyLoadHelper.EnvironmentIdentifier}]") =>
            this.logger = logger;

        public override void Message(LogImportance importance, string message)
        {
            switch (importance)
            {
                case LogImportance.Low:
                    logger.LogMessage(MessageImportance.Low, message);
                    break;
                case LogImportance.Normal:
                    logger.LogMessage(MessageImportance.Normal, message);
                    break;
                default:
                    logger.LogMessage(MessageImportance.High, message);
                    break;
            }
        }

        public override void Warning(string message) =>
            logger.LogWarning(message);

        public override void Error(string message) =>
            logger.LogError(message);
    }
}
