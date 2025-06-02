using System;
using UnityEngine;

namespace Code.VisionCone.Detectors
{
    public class BaseTargetDetector : MonoBehaviour
    {
        public Action<Enemy> OnTargetDetected;
        public Action<Enemy> OnTargetLost;
    }
}