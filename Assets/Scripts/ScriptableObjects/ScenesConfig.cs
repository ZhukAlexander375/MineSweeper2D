using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScenesConfig", menuName = "Create Scenes Config")]
public class ScenesConfig : ScriptableObject
{
    [Serializable]
    public class SceneEntry
    {
        public SceneType SceneType;
        public string SceneName;
    }

    public List<SceneEntry> Scenes = new List<SceneEntry>();

    public string GetSceneName(SceneType sceneType)
    {
        SceneEntry entry = Scenes.Find(scene => scene.SceneType == sceneType);
        return entry != null ? entry.SceneName : null;
    }
}
