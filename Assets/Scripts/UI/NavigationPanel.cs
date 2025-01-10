using System;
using UnityEngine;
using UnityEngine.UI;

public class NavigationPanel : MonoBehaviour
{
    [SerializeField] private MainMenuUI _mainMenuUi;
    [SerializeField] private Button[] _navigationButtons;
    [SerializeField] private Image[] _selectedImages;

    private void Start()
    {
        for (int i = 0; i < _navigationButtons.Length; i++)
        {
            int index = i;
            _navigationButtons[i].onClick.AddListener(() =>
            {
                SetSelectedImage(index);
                _mainMenuUi.SelectMenu(index);
            });
        }
        for (int i = 0; i < _selectedImages.Length; i++)
        {
            _selectedImages[i].gameObject.SetActive(i == 0);
        }

        _mainMenuUi.SelectMenu(0);
    }

    public void SetSelectedImage(int selectedIndex)
    {
        for (int i = 0; i < _selectedImages.Length; i++)
        {
            _selectedImages[i].gameObject.SetActive(i == selectedIndex);
        }
    }
}
