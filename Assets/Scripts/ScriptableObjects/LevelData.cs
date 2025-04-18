using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/Level Data")]
public class LevelData : ScriptableObject
{
    public int levelIndex;
    public string displayName;
    public string sceneName;
    public bool unlockedByDefault;
    
    [TextArea]
    public string description;
}
