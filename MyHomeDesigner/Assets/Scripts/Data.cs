using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SceneObjectData
{
    public string prefabName;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
}

[System.Serializable]
public class ProjectSaveData
{
    public string projectName;
    public List<SceneObjectData> objects;
}
