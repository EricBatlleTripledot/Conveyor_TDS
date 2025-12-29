using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor
{
    public class GridSizeSelectorView : MonoBehaviour
    {
        public event Action<Vector2Int> OnGridSizeConfirmed;
        public event Action OnBack;
    
        [SerializeField] 
        private Button confirmButton;
        [SerializeField] 
        private Button backButton;

        [SerializeField]
        private TMP_InputField xSizeInputField;
        [SerializeField]
        private TMP_InputField ySizeInputField;

        private const int GridXSizeMax = 10;
        private const int GridXSizeMin = 3;
        private const int GridYSizeMax = 10;
        private const int GridYSizeMin = 3;
        private void Awake()
        {
            confirmButton.onClick.AddListener(ConfirmGridSize);
            backButton.onClick.AddListener(GoBack);
        }

        private void GoBack()
        {
            OnBack?.Invoke();
        }

        private void ConfirmGridSize()
        {
            if(int.TryParse(xSizeInputField.text, out var xSize) && int.TryParse(xSizeInputField.text, out var ySize))
            {
                if(xSize is >= GridXSizeMin and <= GridXSizeMax && ySize is >= GridYSizeMin and <= GridYSizeMax)
                {
                    OnGridSizeConfirmed?.Invoke(new Vector2Int(xSize, ySize));
                }
            }
        }
    }
}