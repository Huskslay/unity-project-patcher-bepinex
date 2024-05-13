using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nomnom.UnityProjectPatcher.Editor;
using UnityEditor;
using UnityEngine;

namespace Nomnom.BepInEx.Editor {
    [CustomEditor(typeof(BepinexUserSettings))]
    public sealed class BepInExUserSettingsEditor: UnityEditor.Editor {
        [NonSerialized]
        private bool _isLoading = true;

        [NonSerialized] private string[] _internalPlugins;
        [NonSerialized] private string[] _externalPlugins;
        
        private void OnEnable() {
            FindPlugins();
        }

        private void FindPlugins() {
            _isLoading = true;
            EditorApplication.delayCall += () => {
                _internalPlugins = BepInExPreloader.GetInternalPluginAssemblyNames(PatcherUtility.GetSettings()).ToArray();
                _externalPlugins = BepInExPreloader.GetExternalPluginAssemblyNames(BepInExPreloader.GetBepInExUserSettings()).ToArray();
                _isLoading = false;
            };
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            
            EditorGUILayout.Space();
            
            GUI.enabled = !_isLoading;
            if (GUILayout.Button("Reload")) {
                FindPlugins();
            }

            if (GUILayout.Button("Open BepInEx Folder")) {
                var settings = BepInExPreloader.GetBepInExUserSettings();
                EditorUtility.RevealInFinder(settings.RootFolder);
            }

            GUI.enabled = true;
            
            EditorGUILayout.Space();
            ShowInternalPlugins();
            EditorGUILayout.Space();
            ShowExternalPlugins();
            EditorGUILayout.Space();
        }

        private void ShowInternalPlugins() {
            EditorGUILayout.LabelField("Possible Plugins in the Project", EditorStyles.miniBoldLabel);
            
            if (_isLoading) {
                EditorGUILayout.LabelField(" - loading...");
            } else if (_internalPlugins == null || _internalPlugins.Length == 0) {
                EditorGUILayout.LabelField(" - no plugins found");
            } else {
                var i = -1;
                foreach (var name in _internalPlugins) {
                    i++;
                    EditorGUILayout.LabelField($" - [{i}] {name}");
                }
            }
        }

        private void ShowExternalPlugins() {
            EditorGUILayout.LabelField("External Plugins", EditorStyles.miniBoldLabel);
            
            if (_isLoading) {
                EditorGUILayout.LabelField(" - loading...");
            } else if (_externalPlugins == null || _externalPlugins.Length == 0) {
                EditorGUILayout.LabelField(" - no plugins found");
            } else {
                var i = -1;
                foreach (var name in _externalPlugins) {
                    i++;
                    EditorGUILayout.LabelField($" - [{i}] {name}");
                }
            }
        }
    }
}