using Code.Levels;
using Code.VisionCone.Factory;
using TMPro;
using UnityEngine;

namespace Code.UI.Levels
{
    public class LevelPresenter
    {
        private readonly ILevelController _levelController;

        public LevelPresenter(
            ILevelController levelController)
        {
            _levelController = levelController;
        }
        
        public void OnPlayLevel(VisionType visionType, float time)
        {
            _levelController.PlayLevel(visionType, time);
        }
        
        public VisionType GetVisionTypeFromInput(TMP_Dropdown levelDropdown)
        {
            int selectedIndex = levelDropdown.value;
            VisionType selectedType = (VisionType)selectedIndex;
            return selectedType;
        }
        
        public float GetTimeProgressFromInput(TMP_InputField inputField)
        {
            if (float.TryParse(inputField.text, out float time))
            {
                return Mathf.Max(0f, time);
            }
            Debug.LogWarning("Invalid time input. Defaulting to 1 second.");
            return 1f;
        }
    }
}
