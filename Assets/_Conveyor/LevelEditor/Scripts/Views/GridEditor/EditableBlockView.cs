using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor
{
    public class EditableBlockView : MonoBehaviour
    {
        public event Action<EditableBlockView> BlockSelected;

        [SerializeField]
        private CustomButton customButton;
        [SerializeField]
        private Image backgroundColor;
        [SerializeField]
        private TextMeshProUGUI directionText;
        [SerializeField]
        private Color emptyBlockColor;

        public Color EmptyBlockColor => emptyBlockColor;
        public EditableBlockData CurrentData => currentData;
        [SerializeField]
        private EditableBlockData currentData;
    
        private void Awake()
        {
            customButton.onRightClick.AddListener(ClearBlock);
            customButton.onLeftClick.AddListener(OnClickBlock);

            currentData = new EditableBlockData(new EmptyBlockData(new Vector2Int()));
        }

        private void ClearBlock()
        {
            UpdateView(new EditableBlockData(new EmptyBlockData(currentData.BlockData.Position)));
        }

        private void OnClickBlock()
        {
            BlockSelected?.Invoke(this);
        }

        public void UpdateView(EditableBlockData editableBlockData)
        {
            currentData = editableBlockData;
            switch (currentData.BlockData)
            {
                case EmptyBlockData:
                    backgroundColor.color = emptyBlockColor;
                    directionText.text = BlockDirection.None.ToSymbolString();
                    break;
                case ConveyorBeltBlockData conveyorBelt:
                    backgroundColor.color = emptyBlockColor;
                    directionText.text = "BELT";
                    break;
                case ColorBlockData color:
                    backgroundColor.color = color.Color;
                    directionText.text = color.Direction.ToSymbolString();
                    break;
            }
        }
    }
}