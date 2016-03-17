#region license
// DataProfiler
// Copyright 2013 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System.Collections.Generic;
using System.Linq;
using Cfg.Net.Ext;
using Pipeline.Configuration;
using Pipeline.Contracts;

namespace DataProfiler {

    public class Importer : IImporter {
        readonly ISchemaReader _reader;
        readonly IRunTimeRun _runner;

        public Importer(ISchemaReader reader, IRunTimeRun runner) {
            _reader = reader;
            _runner = runner;
        }

        public ImportResult Import(Connection connection) {

            var schema = _reader.Read();

            var cfg = new Process {
                Name = "Import",
                Connections = new List<Connection> { connection },
                Entities = new List<Entity> { schema.Entities.First() }
            }.WithDefaults().Serialize();

            var process = new Process();
            process.Load(cfg);

            return new ImportResult {
                Connection = connection,
                Fields = process.Entities.First().Fields.Where(f=>!f.System).ToList(),
                Schema = schema,
                Rows = _runner.Run(process),
            };
        }

    }
}