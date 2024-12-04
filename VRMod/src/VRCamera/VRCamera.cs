using MelonLoader;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;
using Valve.VR;

namespace PoY_VR.Mod
{
    public class VRCamera
    {
        private AssetBundle cameraRigAssetBundle;

        public void Initialize(bool startup)
        {
            if (cameraRigAssetBundle == null)
            {
                var assembly = Assembly.GetExecutingAssembly();
                using (var stream = assembly.GetManifestResourceStream(assembly.GetName().Name + ".res.vrassetbundle"))
                {
                    if (stream == null)
                    {
                        MelonLogger.Error("CameraRig AssetBundle not found!");
                        return;
                    }

                    byte[] bundleData;
                    using (var memoryStream = new MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        bundleData = memoryStream.ToArray();
                    }

                    cameraRigAssetBundle = AssetBundle.LoadFromMemory(bundleData);
                    if (cameraRigAssetBundle == null)
                    {
                        MelonLogger.Error("Failed to load AssetBundle!");
                        return;
                    }
                }
            }

            var prefab = cameraRigAssetBundle.LoadAsset<GameObject>("[CameraRig]");
            if (prefab == null)
            {
                MelonLogger.Error("[CameraRig] prefab not found in AssetBundle!");
                return;
            }

            if (startup)
            {
                return;
            }

            GameObject player = GameObject.Find("PlayerCameraHolder");
            
            if (player != null)
            {
                player.SetActive(false);
            }

            var cameraRig = Object.Instantiate(prefab);
            cameraRig.name = "[CameraRig]";
            SetupCameraRig(cameraRig);
            MelonLogger.Msg("[CameraRig] instantiated from AssetBundle.");
        }

        private void SetupCameraRig(GameObject rig)
        {
            rig.transform.position = Vector3.zero;
            rig.transform.rotation = Quaternion.identity;

            var existingCamera = Camera.main;
            if (existingCamera != null)
            {
                rig.transform.position = existingCamera.transform.position;
                rig.transform.rotation = existingCamera.transform.rotation;
            }

            MelonLogger.Msg("Successfully added [CameraRig] GameObject.");
        }
    }
}
