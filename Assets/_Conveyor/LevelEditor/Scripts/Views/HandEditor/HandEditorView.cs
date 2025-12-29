using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor
{
	public class HandEditorView : MonoBehaviour
	{
		public event Action<Hand> Save;
		public event Action<Hand> GoBack;

		[SerializeField]
		private InitialCustomHandView initialCustomHandView;
		[SerializeField]
		private InitialCustomColorWeightsView initialCustomColorWeightsView;

		[Header("Other Buttons")]
		[SerializeField]
		private Button saveButton;
		[SerializeField]
		private Button backButton;

		private void Awake()
		{
			saveButton.onClick.AddListener(OnSaveButton);
			backButton.onClick.AddListener(OnBackButton);
		}

		public void Initialize(HashSet<Color> availableColors)
		{
			initialCustomHandView.Initialize(availableColors);
			initialCustomColorWeightsView.SetAvailableColors(availableColors);
		}

		public void Initialize(Hand hand, HashSet<Color> availableColors)
		{
			initialCustomHandView.Initialize(hand, availableColors);
			initialCustomColorWeightsView.SetAvailableColors(hand.ColorWeightsDict);
		}
		
		private void OnBackButton()
		{
			var hand = new Hand(initialCustomHandView.InitialCustomHand, initialCustomColorWeightsView.ColorWeightsDict);
			GoBack?.Invoke(hand);
		}

		private void OnSaveButton()
		{
			var hand = new Hand(initialCustomHandView.InitialCustomHand,initialCustomColorWeightsView.ColorWeightsDict);
			Save?.Invoke(hand);
		}
	}
}