using UnityEngine;
using UnityEditor;
using System.IO;
using Lowscope.Saving.Data;
using UnityEditor.SceneManagement;
using Lowscope.Saving;
using Lowscope.Saving.Components;
using Lowscope.Saving.Core;
using System.Collections.Generic;

namespace Lowscope.Saving.Core.EditorTools
{
    public class SaveMenuCommands
    {
        [UnityEditor.MenuItem(itemName: "Window/Saving/Open Save Location")]
        public static void OpenSaveLocation()
        {
            string dataPath = SaveFileUtility.SaveFolderPath; //string.Format("{0}/{1}/", Application.persistentDataPath, SaveSettings.Get().fileFolderName);

#if UNITY_EDITOR_WIN
            dataPath = dataPath.Replace(@"/", @"\"); // Windows uses backward slashes
#elif UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
            dataPath = dataPath.Replace("\\", "/"); // Linux and MacOS use forward slashes
#endif

            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }

            EditorUtility.RevealInFinder(dataPath);
        }

        [MenuItem("Window/Saving/Open Save Settings")]
        public static void OpenSaveSystemSettings()
        {
            Selection.activeInstanceID = SaveSettings.Get().GetInstanceID();
        }

        [MenuItem("CONTEXT/Saveable/Wipe Save Data (Based on active slot id)")]
        static void WipeSaveDataContext(MenuCommand command)
        {
            Saveable saveable = command.context as Saveable;
            if (saveable != null)
            {
                if (!Application.isPlaying)
                {
                    List<string> ids = saveable.GetSaveIdentifications();

                    if (ids.Count > 0)
                    {

                        SaveGame saveGame = SaveFileUtility.LoadSave(SaveSettings.Get().defaultSlot);

                        foreach (var id in ids)
                        {
                            saveGame.Remove(id);
                        }

                        SaveFileUtility.WriteSave(saveGame, SaveSettings.Get().defaultSlot);
                    }
                }
                else
                {
                    SaveMaster.WipeSaveable(saveable);
                }
            }
        }

        [MenuItem("Window/Saving/Utility/Saveable/Wipe Save Data (Scene)")]
        public static void WipeSaveDataActiveScene()
        {
            if (EditorUtility.DisplayDialog("Wipe Scene Save Data?",
                "Are you sure you want to remove all save data related to the active scene? " +
                "Do note that this only removes save data based on the default slot in the configuration. " +
                "Ensure the active slot is set to the default slot to see an effect.", "Cancel", "Wipe Scene Save Data"))
                return;

            if (!Application.isPlaying)
            {
                SaveGame saveGame = SaveFileUtility.LoadSave(SaveSettings.Get().defaultSlot);
                saveGame.WipeSceneData(EditorSceneManager.GetActiveScene().name);
                SaveFileUtility.WriteSave(saveGame, SaveSettings.Get().defaultSlot);
            }
            else
            {
                SaveMaster.WipeSceneData(EditorSceneManager.GetActiveScene().name, true);
            }
        }

        [MenuItem("GameObject/Saving/Wipe Save Data (Active Selection(s))", priority = 14)]
        [MenuItem("Window/Saving/Utility/Saveable/Wipe Save Data (Active Selections(s))")]
        public static void WipeSaveDataSelection()
        {
            List<string> SaveIDs = new List<string>();

            foreach (GameObject obj in Selection.gameObjects)
            {
                foreach (Saveable item in obj.GetComponentsInChildren<Saveable>(true))
                {
                    SaveIDs.AddRange(item.GetSaveIdentifications());
                }
            }

            if (SaveIDs.Count > 0)
            {
                SaveGame saveGame = SaveFileUtility.LoadSave(SaveSettings.Get().defaultSlot);

                int saveIdCount = SaveIDs.Count;
                for (int i = 0; i < saveIdCount; i++)
                {
                    saveGame.Remove(SaveIDs[i]);
                }

                SaveFileUtility.WriteSave(saveGame, SaveSettings.Get().defaultSlot);
            }
        }

        [MenuItem("Window/Saving/Utility/Save File/Delete Save File (Default slot)")]
        public static void DeleteDefaultSaveFile()
        {
            if (EditorUtility.DisplayDialog("Delete Save File?",
                "Are you sure you want to delete the save file based on the default slot? " +
                "This action is irreversible.", "Cancel", "Delete Save File"))
                return;

            SaveFileUtility.DeleteSave(SaveSettings.Get().defaultSlot);
        }

        [MenuItem("Window/Saving/Utility/Save File/Delete Save Files (All savegame files)")]
        public static void DeleteAllSaveFiles()
        {
            if (EditorUtility.DisplayDialog("Delete All Save Files?",
                "Are you sure you want to remove all save files? " +
                "This action is irreversible.", "Cancel", "Delete Save File"))
                return;

            SaveFileUtility.DeleteAllSaveFiles();
        }

        [MenuItem("Window/Saving/Utility/Save Identification/Wipe Save Identifications (Active Scene)")]
        public static void WipeSceneSaveIdentifications()
        {
            var activeScene = EditorSceneManager.GetActiveScene();
            GameObject[] rootObjects = activeScene.GetRootGameObjects();
            int rootObjectCount = rootObjects.Length;

            // Get all Saveables, including children and inactive.
            for (int i = 0; i < rootObjectCount; i++)
            {
                foreach (Saveable item in rootObjects[i].GetComponentsInChildren<Saveable>(true))
                {
                    item.SaveIdentification = "";
                    item.OnValidate();
                }
            }
        }

        [MenuItem("Window/Saving/Utility/Save Identification/Wipe Save Identifications (Active Selection(s))")]
        public static void WipeActiveSaveIdentifications()
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                foreach (Saveable item in obj.GetComponentsInChildren<Saveable>(true))
                {
                    item.SaveIdentification = "";
                    item.OnValidate();
                }
            }
        }
    }
}