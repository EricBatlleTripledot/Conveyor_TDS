using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

namespace LevelEditor
{
    public class InitialCustomHandView : MonoBehaviour
    {
        [Header("CustomHandSize")]
        [SerializeField]
        private TMP_InputField initialBlocksAmountInputField;
        
        [Header("ColorSelection")]
        [SerializeField]
        private ColorSelectionView colorSelectionView;

        [Header("CustomHandList")]
        [SerializeField]
        private Transform customHandListContentTransform;
        [SerializeField]
        private CustomHandBlockView customHandBlockViewPrefab;

        private Color selectedColor;
        private List<CustomHandBlockView> customHandBlockViews;

        public List<Color> InitialCustomHand => customHandBlockViews?.Select(view => view.SelectedColor).ToList() ?? new List<Color>();

        private void Awake()
        {
            initialBlocksAmountInputField.onValueChanged.AddListener(OnInitialBlocksAmountChanged);
            selectedColor = colorSelectionView.DefaultSelectedColor;
        }

        private void OnInitialBlocksAmountChanged(string value)
        {
            if (int.TryParse(value, out var initialBlocksAmount))
            {
                UpdateCustomHandList(initialBlocksAmount);
            }
        }

        public void Initialize(HashSet<Color> colors)
        {
            SetAvailableColors(colors.ToList());
        }
        
        public void Initialize(Hand hand, HashSet<Color> availableColors)
        {
            SetAvailableColors(availableColors.ToList());
            UpdateCustomHandList(hand.InitialCustomHand);
            initialBlocksAmountInputField.SetTextWithoutNotify(hand.InitialCustomHand.Count.ToString());
        }
        
        private void SetAvailableColors(List<Color> colors)
        {
            colorSelectionView.SetColorSelections(colors);
            colorSelectionView.ColorSelected += OnColorSelected;
        }

        private void OnColorSelected(Color color)
        {
            selectedColor = color;
        }

        private void UpdateCustomHandList(List<Color> initialBlockColors)
        {
            customHandListContentTransform.DestroyAllChilds();
            customHandBlockViews = new List<CustomHandBlockView>();
            for (var i = 0; i < initialBlockColors.Count; i++)
            {
                CreateCustomHandBlockView(i, initialBlockColors[i]);
            }
        }
        
        private void UpdateCustomHandList(int initialBlocksAmount)
        {
            customHandListContentTransform.DestroyAllChilds();
            customHandBlockViews = new List<CustomHandBlockView>();
            for (var i = 0; i < initialBlocksAmount; i++)
            {
                CreateCustomHandBlockView(i, selectedColor);
            }
        }

        private void CreateCustomHandBlockView(int position, Color color)
        {
            var customHandBlock = Instantiate(customHandBlockViewPrefab, customHandListContentTransform, false);
            customHandBlock.SetPosition(position);
            customHandBlock.SetColor(color);
            customHandBlock.SetColorRequested += SetCustomHandBlockColor;
            customHandBlockViews.Add(customHandBlock);
        }

        private void SetCustomHandBlockColor(CustomHandBlockView customHandBlockView)
        {
            customHandBlockView.SetColor(selectedColor);
        }
    }
}
