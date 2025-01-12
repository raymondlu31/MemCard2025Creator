// MemCard2025
// MIT License
// Copyright (c) 2025 Raymond Lou Independent Developer
// See LICENSE file for full license information.

// Utilities/ConfigHelper.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace MemCard2025Creator.Utilities
{

    public static class ConfigHelper
    {
        public static Dictionary<string, string> ReadConfigFile(string configFilePath)
        {
            var configData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (!File.Exists(configFilePath))
                return configData;

            foreach (var line in File.ReadAllLines(configFilePath))
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) // Ignore comments or blank lines
                    continue;

                // var parts = line.Split('=', 2);
                var parts = line.Split(new[] { '=' }, 2); // Use char array for delimiter

                if (parts.Length == 2)
                {
                    configData[parts[0].Trim()] = parts[1].Trim();
                }
            }

            return configData;
        }
    }
}
