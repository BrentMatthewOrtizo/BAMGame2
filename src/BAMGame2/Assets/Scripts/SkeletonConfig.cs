using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkeletonConfig", menuName = "BAM/Skeleton Config")]
public class SkeletonConfig : ScriptableObject
{
    private static SkeletonConfig _instance;
    public static SkeletonConfig Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<SkeletonConfig>("SkeletonConfig");
            }
            return _instance;
        }
    }

    [Header("Night Skeleton Waves")]
    public List<SkeletonDefinition> night1Skeletons;
    public List<SkeletonDefinition> night2Skeletons;
    public List<SkeletonDefinition> night3Skeletons;

    public List<SkeletonDefinition> GetSkeletonsForNight(int night)
    {
        switch (night)
        {
            case 1: return night1Skeletons;
            case 2: return night2Skeletons;
            case 3: return night3Skeletons;
            default: return night3Skeletons;
        }
    }
}