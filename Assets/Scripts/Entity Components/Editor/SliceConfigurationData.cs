using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

[CreateAssetMenu]
public class SliceConfigurationData : ScriptableObject
{
    [System.Serializable]
    public struct Configuration
    {
        public Vector2Int startLocation;
        public int Cells;
        public ECategory category;
        public EDirection direction;
    }

    public List<Configuration> configurations = new List<Configuration>()
    {
        new Configuration(){ startLocation = new Vector2Int(0,0), Cells = 5, category = ECategory.arms, direction = EDirection.side},
        new Configuration(){ startLocation = new Vector2Int(0,1), Cells = 5, category = ECategory.arms, direction = EDirection.front },
        new Configuration(){ startLocation = new Vector2Int(0,2), Cells = 5, category = ECategory.arms, direction = EDirection.back },
        new Configuration(){ startLocation = new Vector2Int(0,3), Cells = 5, category = ECategory.legs, direction = EDirection.side },
        new Configuration(){ startLocation = new Vector2Int(0,4), Cells = 5, category = ECategory.legs, direction = EDirection.front },
        new Configuration(){ startLocation = new Vector2Int(0,5), Cells = 5, category = ECategory.legs, direction = EDirection.back },

        new Configuration(){startLocation = new Vector2Int(0,6), Cells = 1, category = ECategory.chest, direction = EDirection.front },
        new Configuration(){startLocation = new Vector2Int(1,6), Cells = 1, category = ECategory.chest, direction = EDirection.side},
        new Configuration(){startLocation = new Vector2Int(2,6), Cells = 1, category = ECategory.chest, direction = EDirection.back },

        new Configuration(){startLocation = new Vector2Int(0,7), Cells = 1, category = ECategory.head, direction = EDirection.front },
        new Configuration(){startLocation = new Vector2Int(1,7), Cells = 1, category = ECategory.head, direction = EDirection.side},
        new Configuration(){startLocation = new Vector2Int(2,7), Cells = 1, category = ECategory.head, direction = EDirection.back },

        new Configuration(){startLocation = new Vector2Int(0,8), Cells = 1, category = ECategory.hair, direction = EDirection.front },
        new Configuration(){startLocation = new Vector2Int(1,8), Cells = 1, category = ECategory.hair, direction = EDirection.side},
        new Configuration(){startLocation = new Vector2Int(2,8), Cells = 1, category = ECategory.hair, direction = EDirection.back},

        new Configuration(){startLocation = new Vector2Int(0,9), Cells = 1, category = ECategory.eye, direction = EDirection.front },
        new Configuration(){startLocation = new Vector2Int(1,9), Cells = 1, category = ECategory.eye, direction = EDirection.side },
    };
}