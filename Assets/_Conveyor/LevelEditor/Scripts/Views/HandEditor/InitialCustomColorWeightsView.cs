using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
    public class InitialCustomColorWeightsView : MonoBehaviour
    {
        [SerializeField]
        private Transform customColorWeightsContentTransform;
        [SerializeField]
        private CustomColorWeightView customColorWeightViewPrefab;
        
        public Dictionary<Color, float> ColorWeightsDict { get; private set; }

        public void SetAvailableColors(HashSet<Color> availableColors)
        {
            ColorWeightsDict = new Dictionary<Color, float>();
            foreach (var color in availableColors)
            {
                CreateCustomColorWeightView(color, Hand.DEFAULT_COLOR_WEIGHT);
            }
        }
        
        public void SetAvailableColors(IReadOnlyDictionary<Color, float> colorWeightsDict)
        {
            ColorWeightsDict = new Dictionary<Color, float>();
            foreach (var (color, weight) in colorWeightsDict)
            {
                CreateCustomColorWeightView(color, weight);
            }
        }

        private void CreateCustomColorWeightView(Color color, float weight)
        {
            var customColorWeightView = Instantiate(customColorWeightViewPrefab, customColorWeightsContentTransform, false);
            customColorWeightView.SetColor(color);
            customColorWeightView.SetWeight(weight);
            customColorWeightView.ColorWeightChanged += OnColorWeightChanged;
            ColorWeightsDict.Add(color, weight);
        }

        private void OnColorWeightChanged(Color color, float weight)
        {
            ColorWeightsDict[color] = weight;
        }
    }
}
