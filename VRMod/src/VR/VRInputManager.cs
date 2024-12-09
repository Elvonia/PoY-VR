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

using MelonLoader.Utils;
using System.IO;
using Valve.VR;
using UnityEngine;

namespace PoY_VR.Mod
{
    public class VRInputManager
    {
        private SteamVR_Action_Boolean interactUI;
        private SteamVR_Action_Boolean teleport;
        private SteamVR_Action_Single squeeze;
        private SteamVR_Action_Vector2 move;

        public void Initialize()
        {
            string actionPath = Path.Combine(MelonEnvironment.UserDataDirectory, "VRMod", "actions.json");

            if (File.Exists(actionPath))
            {
                OpenVR.Input.SetActionManifestPath(actionPath);
                SteamVR_Input.Initialize(true);

                interactUI = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("InteractUI");
                teleport = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Teleport");
                squeeze = SteamVR_Input.GetAction<SteamVR_Action_Single>("Squeeze");
                move = SteamVR_Input.GetAction<SteamVR_Action_Vector2>("Move");

                Logger.Log("SteamVR Input initialized with actions and bindings configurations.");
            }
            else
            {
                Logger.Error("Failed to find actions.json file.");
            }
        }

        public void UpdateInputs(GameObject cameraRig)
        {
            if (interactUI.GetStateDown(SteamVR_Input_Sources.LeftHand))
                Logger.Log("InteractUI Pressed (Left Hand)");

            if (interactUI.GetStateDown(SteamVR_Input_Sources.RightHand))
                Logger.Log("InteractUI Pressed (Right Hand)");

            if (teleport.GetState(SteamVR_Input_Sources.LeftHand))
                Logger.Log("Teleport Held (Left Hand)");

            if (teleport.GetState(SteamVR_Input_Sources.RightHand))
                Logger.Log("Teleport Held (Right Hand)");

            float squeezeValueLeft = squeeze.GetAxis(SteamVR_Input_Sources.LeftHand);
            if (squeezeValueLeft > 0)
                Logger.Log($"Squeeze Left Value: {squeezeValueLeft}");

            float squeezeValueRight = squeeze.GetAxis(SteamVR_Input_Sources.RightHand);
            if (squeezeValueRight > 0)
                Logger.Log($"Squeeze Right Value: {squeezeValueRight}");

            Vector2 moveInput = move.GetAxis(SteamVR_Input_Sources.Any);
            Logger.Log($"Move Input: {moveInput}");
        }

        public void Shutdown()
        {
            
        }
    }
}
