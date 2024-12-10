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

using UnityEngine;
using Valve.VR;

namespace PoY_VR.Mod
{
    public class VRCameraRig
    {
        public GameObject cameraRig;

        public void Initialize()
        {
            if (cameraRig != null)
            {
                Logger.Log("CameraRig already present.");
                return;
            }

            cameraRig = new GameObject("CameraRig");
            //cameraRig.AddComponent<ClimbController>();

            GameObject camera = new GameObject("Camera");
            GameObject leftHand = new GameObject("LeftHand");
            GameObject rightHand = new GameObject("RightHand");
            GameObject leftModel = new GameObject("LeftModel");
            GameObject rightModel = new GameObject("RightModel");

            camera.transform.parent = cameraRig.transform;
            leftHand.transform.parent = cameraRig.transform;
            rightHand.transform.parent = cameraRig.transform;
            leftModel.transform.parent = leftHand.transform;
            rightModel.transform.parent = rightHand.transform;

            Camera mainCamera = camera.AddComponent<Camera>();
            camera.gameObject.tag = "MainCamera";
            camera.AddComponent<SteamVR_CameraHelper>();

            mainCamera.cullingMask = 386137879;
            mainCamera.farClipPlane = 12000f;
            mainCamera.fieldOfView = 110f;
            mainCamera.nearClipPlane = 0.01f;
            mainCamera.renderingPath = RenderingPath.DeferredShading;
            mainCamera.stereoTargetEye = StereoTargetEyeMask.Both;

            leftModel.AddComponent<SteamVR_RenderModel>();
            rightModel.AddComponent<SteamVR_RenderModel>();

            SteamVR_Behaviour_Pose leftHandPose = leftHand.AddComponent<SteamVR_Behaviour_Pose>();
            SteamVR_Behaviour_Pose rightHandPose = rightHand.AddComponent<SteamVR_Behaviour_Pose>();
            SteamVR_Action_Pose poseAction = SteamVR_Input.GetAction<SteamVR_Action_Pose>("Pose");

            if (poseAction == null)
            {
                Logger.Error("Pose action not found!");
                return;
            }

            leftHandPose.poseAction = poseAction;
            rightHandPose.poseAction = poseAction;

            leftHandPose.inputSource = SteamVR_Input_Sources.LeftHand;
            rightHandPose.inputSource = SteamVR_Input_Sources.RightHand;

            Object.DontDestroyOnLoad(cameraRig);
            Logger.Log("SteamVR CameraRig created.");
        }

        public void DisableExistingPlayerCamera()
        {
            GameObject playerCameraHolder = GameObject.Find("PlayerCameraHolder");

            if (playerCameraHolder != null)
            {
                Object.Destroy(playerCameraHolder);
                Logger.Log("PlayerCameraHolder removed from scene.");
            }
            else
            {
                Logger.Error("PlayerCameraHolder not found.");
            }
        }

        public void DisablePostProcessing()
        {
            GameObject postProcessing = GameObject.Find("_PostProcessingGlobal");

            if (postProcessing != null)
            {
                Object.Destroy(postProcessing);
            }
        }

        public void UpdateCameraRigTransform()
        {
            GameObject cameraRig = GameObject.Find("CameraRig");
            GameObject playerStartPos = GameObject.Find("PlayerStartPosition_Barometer");

            if (cameraRig != null)
            {
                if (playerStartPos != null)
                {
                    cameraRig.transform.position = playerStartPos.transform.position;
                    Logger.Log("CameraRig transform updated.");
                }
                else
                {
                    Logger.Error("PlayerStartPosition not found.");
                }
            }
            else
            {
                Logger.Error("CameraRig  not found.");
            }
        }
    }
}
