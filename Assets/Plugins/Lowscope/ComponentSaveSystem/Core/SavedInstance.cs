using Lowscope.Saving.Components;
using UnityEngine;

namespace Lowscope.Saving.Core
{
    /// <summary>
    /// Saved instances are objects that should respawn when they are not destroyed.
    /// </summary>
    [AddComponentMenu("")]
    public class SavedInstance : MonoBehaviour
    {
        private SaveInstanceManager instanceManager;
        internal Saveable Saveable { private set; get; }

        // By default, when destroyed, the saved instance will wipe itself from existance.
        private bool removeData = true;

        private SaveInstanceManager.SpawnInfo spawnInfo;

        /// <summary>
        /// Info regarding the saved instance (read only)
        /// </summary>
        public SaveInstanceManager.SpawnInfo SpawnInfo { get { return spawnInfo; } }

        public bool DontSaveInstance { get { return !Saveable.SaveWhenDisabled && disableFrame != -1 && disableFrame != Time.frameCount; } }

        private int disableFrame = -1;

        public void Configure(Saveable saveable, SaveInstanceManager instanceManager, SaveInstanceManager.SpawnInfo spawnInfo)
        {
            this.Saveable = saveable;
            this.instanceManager = instanceManager;
            this.spawnInfo = spawnInfo;
        }

        public void Destroy()
        {
            Saveable.ManualSaveLoad = true;
            removeData = false;
            SaveMaster.RemoveListener(Saveable);
            Destroy(this.gameObject);
        }

        private void OnEnable()
        {
            disableFrame = -1;
        }

        private void OnDisable()
        {
            disableFrame = Time.frameCount;
        }

        private void OnDestroy()
        {
            if (SaveMaster.DeactivatedObjectExplicitly(this.gameObject))
            {
                if (!Saveable.SaveWhenDisabled && !this.gameObject.activeSelf)
                {
                    return;
                }

                if (removeData)
                {
                    SaveMaster.WipeSaveable(Saveable);
                    instanceManager.DestroyObject(this, Saveable);
                }
            }
        }
    }
}
