using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class HandView : MonoBehaviour
    {
        public event Action<HandButtonView, Color> ColorSelected;

        [SerializeField]
        private List<HandButtonView> handButtonViews;

        public bool HasEmptySlots => handButtonViews.Count(handButtonView => handButtonView.IsEmpty) > 0;

        private void Awake()
        {
            foreach (var handButtonView in handButtonViews)
            {
                handButtonView.ColorSelected += OnColorSelected;
            }
        }

        public void AddConveyorBlockButton(ConveyorBlock conveyorBlock)
        {
            foreach (var handButtonView in handButtonViews)
            {
                if (handButtonView.IsEmpty)
                {
                    handButtonView.UpdateView(conveyorBlock.Color);
                    return;
                }
            }
        }

        private void OnColorSelected(HandButtonView handButtonView, Color color)
        {
            ColorSelected?.Invoke(handButtonView, color);
        }
    }
}
