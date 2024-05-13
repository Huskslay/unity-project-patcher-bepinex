﻿using HarmonyLib;
using UnityEditor;
using UnityEngine;

namespace Nomnom.BepInEx.Editor.Patches {
    [HarmonyPatch(typeof(Object))]
    [InjectPatch(PatchLifetime.Always)]
    internal sealed class BlockAssetDestroyPatch {
        [HarmonyPatch(nameof(Object.Destroy), argumentTypes: new []{ typeof(Object) })]
        [HarmonyPrefix]
        private static bool OnDestroy(Object obj) {
            if (!obj) return false;
            if (AssetDatabase.Contains(obj)) {
                Debug.LogWarning($"Tried to destroy an asset: {obj.name}");
                return false;
            }
            return true;
        }
        
        [HarmonyPatch(nameof(Object.DestroyImmediate), argumentTypes: new []{ typeof(Object) })]
        [HarmonyPrefix]
        private static bool OnDestroyImmediate(Object obj) {
            if (!obj) return false;
            if (AssetDatabase.Contains(obj)) {
                Debug.LogWarning($"Tried to destroy an asset immediately: {obj.name}");
                return false;
            }
            return true;
        }
        
        [HarmonyPatch(nameof(Object.DestroyImmediate), argumentTypes: new []{ typeof(Object), typeof(bool) })]
        [HarmonyPrefix]
        private static bool OnDestroyImmediate(Object obj, bool allowDestroyingAssets) {
            return OnDestroyImmediate(obj);
        }
    }
}