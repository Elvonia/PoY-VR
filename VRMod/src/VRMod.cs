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

using MelonLoader;
using MelonLoader.Utils;
using System.IO;
using UnityEngine;
using Valve.VR;

[assembly: MelonInfo(typeof(PoY_VR.Mod.VRMod), "VR Mod", PluginInfo.PLUGIN_VERSION, "Kalico")]
[assembly: MelonGame("TraipseWare", "Peaks of Yore")]

namespace PoY_VR.Mod
{
    public class VRMod : MelonMod
    {
        VRInputManager vrInputManager;
        VRCameraRig vrCameraRig;

        public override void OnInitializeMelon()
        {
            base.OnInitializeMelon();

            Logger.Log("Initializing SteamVR Mod...");
            VR.Initialize();
            vrInputManager = new VRInputManager();
            vrInputManager.Initialize();
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            base.OnSceneWasLoaded(buildIndex, sceneName);

            if (buildIndex == 0 && vrCameraRig == null)
            {
                vrCameraRig = new VRCameraRig();
                vrCameraRig.Initialize();
            }

            Logger.Log($"Scene index: {buildIndex}");
            Logger.Log($"Scene name: {sceneName}");

            vrCameraRig.DisableExistingPlayerCamera();
            vrCameraRig.DisablePostProcessing();
            vrCameraRig.UpdateCameraRigTransform();

            if (!vrCameraRig.cameraRig.GetComponent<ClimbController>())
                vrCameraRig.cameraRig.AddComponent<ClimbController>();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (vrInputManager != null && vrCameraRig != null)
            {
                vrInputManager.UpdateInputs(vrCameraRig.cameraRig);
            }
        }

        public override void OnApplicationQuit()
        {
            Logger.Log("SteamVR shut down.");
            base.OnApplicationQuit();
        }
    }
}
