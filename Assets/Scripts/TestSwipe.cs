using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TestSwipe : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private float _minSwipeDistance;
    [SerializeField] private RectTransform _menuContainer;
    [SerializeField] private RectTransform _menuFirstItem;
    [SerializeField] private Image[] _dotImages;
    [SerializeField] private Color _activeDotColor = Color.white;
    [SerializeField] private Color _inactiveDotColor = Color.gray;
    //[SerializeField] private TMP_Text[] _buttonsTexts;
    //[SerializeField] private Sprite _activeButtonSprite;
    //[SerializeField] private Sprite _inactiveButtonSprite;
    [SerializeField] private GameObject[] _menuWindows;    
    [SerializeField] private float _timeToScreen;
    [SerializeField] private bool _isDragging = true;

    private Vector2 _startTouchPosition;
    private Vector2 _currentTouchPosition;
    private bool _isTouching = false;
    private int _currentMenuIndex = 0;
    private float _menuWidth;

    private void Start()
    {
        _menuWidth = _menuFirstItem.rect.width;
        _menuContainer.anchoredPosition = new Vector2(-_currentMenuIndex * _menuWidth, 0);
               
        ShowActiveWindow();        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _startTouchPosition = eventData.position;
        _isTouching = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _currentTouchPosition = eventData.position;

        if (_isTouching)
        {
            if (Mathf.Abs(_currentTouchPosition.x - _startTouchPosition.x) > _minSwipeDistance)
            {
                if (_currentTouchPosition.x > _startTouchPosition.x)
                {
                    OnSwipeRight();
                }
                else
                {
                    OnSwipeLeft();
                }
            }
        }

        _isTouching = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _currentTouchPosition = eventData.position;
    }

    private void OnSwipeRight()
    {
        if (_isDragging)
        {
            _currentMenuIndex = Mathf.Clamp(_currentMenuIndex - 1, 0, 2);
            //UpdateActiveButton();
            UpdateActiveDot();            
            ShowActiveWindow();
        }
    }

    private void OnSwipeLeft()
    {
        if (_isDragging)
        {
            _currentMenuIndex = Mathf.Clamp(_currentMenuIndex + 1, 0, 2);
            //UpdateActiveButton();
            UpdateActiveDot();
            ShowActiveWindow();
            
        }
    }

    public void OnButtonClick(int index)
    {
        _currentMenuIndex = index;
        //UpdateActiveButton();
        UpdateActiveDot();
        ShowActiveWindow();
        
    }

    private void UpdateActiveDot()
    {
        for (int i = 0; i < _dotImages.Length; i++)
        {
            _dotImages[i].color = i == _currentMenuIndex ? _activeDotColor : _inactiveDotColor;
        }
    }

    /*private void UpdateActiveButton()
    {
        Color activeColor = Color.black;
        Color inactiveColor;
        ColorUtility.TryParseHtmlString("#8E8E93", out inactiveColor);

        for (int i = 0; i < _menuButtons.Length; i++)
        {
            _menuButtons[i].image.sprite = i == _currentMenuIndex ? _activeButtonSprite : _inactiveButtonSprite;
            _buttonsTexts[i].color = i == _currentMenuIndex ? activeColor : inactiveColor;
        }
    }*/

    private void ShowActiveWindow()
    {
        for (int i = 0; i < _menuWindows.Length; i++)
        {
            float targetX = i == _currentMenuIndex ? 0f : (i - _currentMenuIndex) * _menuWidth;
            _menuWindows[i].transform.DOLocalMoveX(targetX, _timeToScreen);
        }
    }
   

    public void OffSwipe()
    {
        _isDragging = false;
    }

    public void OnSwipe()
    {
        _isDragging = true;
    }
}
