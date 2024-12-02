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

using Rewired;
using Rewired.Data;
using Rewired.Data.Mapping;
using System.Collections.Generic;
using System.Linq;

namespace PoY_VR.Mod
{
    internal static class RewiredVR
    {
        internal static CustomController CreateRewiredController()
        {
            HardwareControllerMap_Game hcMap = new HardwareControllerMap_Game(
                "VRControllers",
                new ControllerElementIdentifier[]
                {
                    
                },
                new int[] { },
                new int[] { },
                new AxisCalibrationData[]
                {
                   
                },
                new AxisRange[]
                {
                    
                },
                new HardwareAxisInfo[]
                {
                    
                },
                new HardwareButtonInfo[] { },
                null
            );

            ReInput.UserData.AddCustomController();
            CustomController_Editor newController = ReInput.UserData.customControllers.Last();
            newController.name = "VRControllers";

            foreach (ControllerElementIdentifier element in hcMap.elementIdentifiers.Values)
            {
                
            }

            CustomController customController = ReInput.controllers.CreateCustomController(newController.id);
            customController.bisgSaPvoYbFxjlNuMXxGTjalLinb = false;
            return customController;
        }

        internal static CustomControllerMap CreateGameplayMap(int controllerID)
        {

            List<ActionElementMap> defaultElementMaps = new List<ActionElementMap>()
            {
                
            };

            return CreateCustomMap("VRDefault", 0, controllerID, defaultElementMaps);

        }

        private static CustomControllerMap CreateCustomMap(string mapName, int categoryId, int controllerId, List<ActionElementMap> actionElementMaps)
        {
            ReInput.UserData.CreateCustomControllerMap(categoryId, controllerId, 0);

            ControllerMap_Editor newMap = ReInput.UserData.customControllerMaps.Last();
            newMap.name = mapName;

            foreach (ActionElementMap elementMap in actionElementMaps)
            {
                
            }

            return ReInput.UserData.yvXDSzNJCHuGPngEgzbrDtokQxXk(categoryId, controllerId, 0);
        }
    }
}