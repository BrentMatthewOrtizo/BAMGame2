using UnityEngine;

[System.Serializable]
public class CropData
{
    public Vector3 worldPos;   // Where the crop is placed in the world
    public int currentStage;   // Which growth stage it’s in
    public float timeElapsed;  // How far along in its current stage it is
}