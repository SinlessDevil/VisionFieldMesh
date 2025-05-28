using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Code.VisionCone.Factory;

namespace Code.UI.Levels
{
    public class LevelView : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _timeProgressLevel;
        [SerializeField] private TMP_Dropdown _levelDropdown;
        [SerializeField] private Button _startLevelButton;

        private LevelPresenter _levelPresenter;

        public void Initialize(LevelPresenter levelPresenter)
        {
            _levelPresenter = levelPresenter;

            PopulateDropdownWithVisionTypes();
            Subscribe();
        }

        public void Dispose()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _startLevelButton.onClick.AddListener(OnPlayLevel);
        }

        private void Unsubscribe()
        {
            _startLevelButton.onClick.RemoveListener(OnPlayLevel);
        }

        private void PopulateDropdownWithVisionTypes()
        {
            var visionTypeNames = Enum.GetNames(typeof(VisionType)).ToList();
            _levelDropdown.ClearOptions();
            _levelDropdown.AddOptions(visionTypeNames);
        }

        private void OnPlayLevel()
        {
            var visionType = _levelPresenter.GetVisionTypeFromInput(_levelDropdown);
            var timeProgress = _levelPresenter.GetTimeProgressFromInput(_timeProgressLevel);
            
            _levelPresenter.OnPlayLevel(visionType, timeProgress);
        }
    }
}