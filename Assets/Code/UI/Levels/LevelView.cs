using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI.Levels
{
    public class LevelView : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _timeProgressLevel; // in seconds
        [SerializeField] private TMP_Dropdown _levelDropdown;
        [SerializeField] private Button _startLevelButton;
        
        public void Initialize()
        {
            
        }
    }
}