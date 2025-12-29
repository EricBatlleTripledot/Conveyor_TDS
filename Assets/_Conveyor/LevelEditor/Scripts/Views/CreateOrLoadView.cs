using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor
{
	public class CreateOrLoadView : MonoBehaviour
	{
		public event Action<string> CreateClicked;
		public event Action LoadClicked;
	
		[SerializeField]
		private TMP_InputField nameInputField;
		[SerializeField]
		private Button createButton;
		[SerializeField]
		private Button loadButton;

		private void Awake()
		{
			createButton.onClick.AddListener(OnCreateClicked);
			loadButton.onClick.AddListener(OnLoadClicked);
		}

		private void OnLoadClicked()
		{
			LoadClicked?.Invoke();
		}

		private void OnCreateClicked()
		{
			CreateClicked?.Invoke(nameInputField.text);
		}
	}
}