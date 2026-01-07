using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class HandButtonView : MonoBehaviour
    {
        public event Action<HandButtonView, Color> ColorSelected;

        [SerializeField]
        private Button button;
        [SerializeField]
        private Color emptyColor;

        private Color currentColor;

        public bool IsEmpty { get; private set; } = true;

        private void Awake()
        {
            button.onClick.AddListener(OnButtonClicked);
            UpdateView(emptyColor);
        }

        public void UpdateView(Color color)
        {
            IsEmpty = color == emptyColor;
            currentColor = color;
            button.image.color = currentColor;
        }

        public void Clear()
        {
            UpdateView(emptyColor);
        }

        private void OnButtonClicked()
        {
            if (!IsEmpty)
            {
                ColorSelected?.Invoke(this, currentColor);
            }
        }
    }
}
