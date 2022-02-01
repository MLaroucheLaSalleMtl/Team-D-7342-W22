using System.IO;
using System.Text;
using Lowscope.Saving.Enums;
using UnityEngine;

namespace Lowscope.Saving.Data
{
    // Not available in the free version

    public class SaveGameBinary : SaveGame
    {
        // Empty, so code compiles.
        public override void Dispose() { }
        public override string Get(string id) { return ""; }
        public override void OnAfterLoad() { }
        public override void OnBeforeWrite() { }

        public override void ReadSaveFromPath(string savePath)
        {

        }

        public override void Remove(string id) { }
        public override void Set(string id, string data, string scene) { }
        public override void WipeSceneData(string sceneName) { }
        public override void WriteSaveFile(SaveGame saveGame, string savePath) { }
    }
}