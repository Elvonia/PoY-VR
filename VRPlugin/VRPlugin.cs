// BSD 2-Clause License
//
// Copyright (c) 2024, Elvonia
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//
// 1. Redistributions of source code must retain the above copyright notice,
//    this list of conditions and the following disclaimer.
//
// 2. Redistributions in binary form must reproduce the above copyright notice,
//    this list of conditions and the following disclaimer in the documentation
//    and/or other materials provided with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
// POSSIBILITY OF SUCH DAMAGE.

using AssetsToolsCustom.NET;
using AssetsToolsCustom.NET.Extra;
using MelonLoader;
using MelonLoader.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

[assembly: MelonInfo(typeof(PoY_VR.Plugin.VRPlugin), "VR Patcher", PluginInfo.PLUGIN_VERSION, "Kalico")]
[assembly: MelonGame("TraipseWare", "Peaks of Yore")]

namespace PoY_VR.Plugin
{
    public class VRPlugin : MelonPlugin
    {
        public override void OnPreInitialization()
        {
            string classDataTempPath = null;

            try
            {
                MelonLogger.Msg("Starting VR patch process...");

                string dataPath = MelonEnvironment.UnityGameDataDirectory;
                string gameManagersPath = Path.Combine(dataPath, "globalgamemanagers");
                string gameManagersBackupPath = CreateGameManagersBackup(gameManagersPath);
                string patcherPath = MelonEnvironment.UserLibsDirectory;

                classDataTempPath = ExtractEmbeddedDatFile(
                    Assembly.GetExecutingAssembly().GetName().Name + ".res.cldb.dat", Path.GetTempFileName());

                CopyPlugins(patcherPath, dataPath);
                PatchVR(gameManagersBackupPath, gameManagersPath, classDataTempPath);

                MelonLogger.Msg("Patched successfully!");
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Error during patching: {ex}");
            }
            finally
            {
                if (!string.IsNullOrEmpty(classDataTempPath) && File.Exists(classDataTempPath))
                {
                    try
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        File.Delete(classDataTempPath);
                        MelonLogger.Msg($"Temporary file '{classDataTempPath}' deleted.");
                    }
                    catch (Exception cleanupEx)
                    {
                        MelonLogger.Warning($"Failed to clean up temporary file '{classDataTempPath}': {cleanupEx.Message}");
                    }
                }
            }
        }

        private static void CopyPlugins(string patcherPath, string dataPath)
        {
            MelonLogger.Msg("Copying plugins...");

            string gamePluginsPath = Path.Combine(dataPath, "Plugins", "x86_64");

            if (!Directory.Exists(gamePluginsPath))
            {
                MelonLogger.Error("Plugins folder not found.");
                return;
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(gamePluginsPath);
            FileInfo[] files = directoryInfo.GetFiles();
            Assembly assembly = Assembly.GetExecutingAssembly();
            string assemblyName = assembly.GetName().Name;
            string[] resourceNames = assembly.GetManifestResourceNames();
            MelonLogger.Msg($"Found embedded resources:\n{string.Join(",\n", resourceNames)}");

            string[] pluginArray = new string[]
            {
                "AudioPluginOculusSpatializer.dll",
                "openvr_api.dll",
                "OVRGamepad.dll",
                "OVRPlugin.dll"
            };

            for (int i = 0; i < pluginArray.Length; i++)
            {
                string pluginName = pluginArray[i];
                
                if (!Array.Exists<FileInfo>(files, (FileInfo file) => pluginName == file.Name))
                {
                    using (Stream manifestResourceStream = 
                        assembly.GetManifestResourceStream(assemblyName + ".res." + pluginName))
                    {
                        using (FileStream fileStream = 
                            new FileStream(Path.Combine(directoryInfo.FullName, pluginName), FileMode.Create, FileAccess.Write, FileShare.Delete))
                        {
                            MelonLogger.Msg("Copying " + pluginName);
                            manifestResourceStream.CopyTo(fileStream);
                        }
                    }
                }
                else
                {
                    MelonLogger.Msg($"Plugin: {pluginName} exists.");
                }
            }

            MelonLogger.Msg("VR plugins copied successfully.");
        }

        private static string CreateGameManagersBackup(string gameManagersPath)
        {
            MelonLogger.Msg($"Backing up '{gameManagersPath}'...");
            string backupPath = gameManagersPath + ".bak";

            if (File.Exists(backupPath))
            {
                MelonLogger.Msg("Backup already exists.");
                return backupPath;
            }

            File.Copy(gameManagersPath, backupPath);
            MelonLogger.Msg($"Created backup in '{backupPath}'");

            return backupPath;
        }

        private static void PatchVR(string gameManagersBackupPath, string gameManagersPath, string classDataPath)
        {
            MelonLogger.Msg("Patching globalgamemanagers...");
            MelonLogger.Msg($"Using classData file from path '{classDataPath}'");

            try
            {
                AssetsManager am = new AssetsManager();
                am.LoadClassDatabase(classDataPath);

                AssetsFileInstance ggm = am.LoadAssetsFile(gameManagersBackupPath, false);
                AssetFileInfoEx assetInfo = ggm.table.GetAssetInfo(11);
                AssetTypeInstance ati = am.GetTypeInstance(ggm.file, assetInfo, false);
                AssetTypeValueField assetTypeValueField = ati?.GetBaseField(0);
                AssetTypeValueField enabledVRDevices = assetTypeValueField?.Get("enabledVRDevices");

                if (enabledVRDevices != null)
                {
                    AssetTypeValueField arrayField = enabledVRDevices.Get("Array");

                    if (arrayField != null)
                    {
                        AssetTypeValueField newValue = ValueBuilder.DefaultValueFieldFromArrayTemplate(arrayField);
                        newValue.GetValue().Set("OpenVR");
                        arrayField.SetChildrenList(new[] { newValue });

                        byte[] buffer;
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            using (AssetsFileWriter writer = new AssetsFileWriter(memoryStream))
                            {
                                writer.bigEndian = false;
                                assetTypeValueField.Write(writer, 0);
                                buffer = memoryStream.ToArray();
                            }
                        }

                        List<AssetsReplacer> replacers = new List<AssetsReplacer>
                            {
                                new AssetsReplacerFromMemory(0, assetInfo.index, (int)assetInfo.curFileType, ushort.MaxValue, buffer)
                            };

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            using (AssetsFileWriter writer = new AssetsFileWriter(memoryStream))
                            {
                                ggm.file.Write(writer, 0, replacers, 0, null);
                                ggm.stream.Close();
                                File.WriteAllBytes(gameManagersPath, memoryStream.ToArray());
                            }
                        }

                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Error processing asset: {ex.Message}");
            }
        }

        private static string ExtractEmbeddedDatFile(string resourceName, string outputPath)
        {
            MelonLogger.Msg($"Extracting embedded resource '{resourceName}' to '{outputPath}'...");

            var assembly = Assembly.GetExecutingAssembly();
            using (Stream resourceStream = assembly.GetManifestResourceStream(resourceName))
            {
                if (resourceStream == null)
                {
                    MelonLogger.Error($"Embedded resource '{resourceName}' not found.");
                    throw new FileNotFoundException($"Resource '{resourceName}' not found in assembly.");
                }

                using (FileStream fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                {
                    resourceStream.CopyTo(fileStream);
                }
            }

            MelonLogger.Msg($"Embedded resource '{resourceName}' successfully extracted to '{outputPath}'.");
            return outputPath;
        }
    }
}
