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
    public class ClimbController : MonoBehaviour
    {
        public Transform cameraRig;
        public Transform leftHand;
        public Transform rightHand;

        private SteamVR_Action_Single gripAction;
        private Transform grabbedObject = null;
        private Vector3 initialControllerPosition;
        private Vector3 initialCameraRigPosition;

        private void Awake()
        {
            gripAction = SteamVR_Input.GetAction<SteamVR_Action_Single>("Squeeze");
            cameraRig = GameObject.Find("CameraRig").GetComponent<Transform>();
            leftHand = GameObject.Find("LeftHand").GetComponent<Transform>();
            rightHand = GameObject.Find("RightHand").GetComponent<Transform>();
        }

        private void Update()
        {
            HandleGrab();
        }

        private void HandleGrab()
        {
            float squeezeValueLeft = gripAction.GetAxis(SteamVR_Input_Sources.LeftHand);

            if (squeezeValueLeft == 1)
                GrabObject(leftHand);

            float squeezeValueRight = gripAction.GetAxis(SteamVR_Input_Sources.RightHand);

            if (squeezeValueRight == 1)
                GrabObject(rightHand);

            if (squeezeValueLeft == 0 || squeezeValueRight == 0)
            {
                ReleaseObject();
            }
        }

        private void GrabObject(Transform hand)
        {
            Collider[] hits = Physics.OverlapSphere(hand.position, 0.25f);

            foreach (var hit in hits)
            {
                if (hit.CompareTag("Climbable"))
                {
                    grabbedObject = hit.transform;
                    Logger.Log($"Grabbed object: {grabbedObject.name}");
                    break;
                }
            }
        }

        private void ReleaseObject()
        {
            if (grabbedObject != null)
            {
                grabbedObject = null;
                Logger.Log($"Released object: {grabbedObject.name}");
            }
        }
    }
}
