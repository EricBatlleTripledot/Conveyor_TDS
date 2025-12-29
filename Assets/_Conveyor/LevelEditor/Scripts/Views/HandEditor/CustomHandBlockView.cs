using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor
{
    public class CustomHandBlockView : MonoBehaviour
    {
        public event Action<CustomHandBlockView> SetColorRequested;

        [SerializeField]
        private TextMeshProUGUI positionText;
        [SerializeField]
        private Button setColorButton;

        public Color SelectedColor { get; private set; }

        private void Awake()
        {
            setColorButton.onClick.AddListener(OnSetColorRequested);
        }

        private void OnSetColorRequested()
        {
            SetColorRequested?.Invoke(this);
        }

        public void SetPosition(int position)
        {
            positionText.text = $"{position} - ";
        }
        
        public void SetColor(Color color)
        {
            setColorButton.image.color = color;
            SelectedColor = color;
        }
    }
}
