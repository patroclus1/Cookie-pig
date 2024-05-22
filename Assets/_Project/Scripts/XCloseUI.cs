using UnityEngine;
using UnityEngine.UI;

public class XCloseUI : MonoBehaviour
{
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite hoveredSprite;

    private Image _hoverableButton;
    private GameObject _buttonUIContainer;

    private void Awake()
    {
        _hoverableButton = GetComponent<Image>();
        _buttonUIContainer = _hoverableButton.transform.parent.gameObject;
    }

    public void ButtonRedOnMouseEnter()
    {
        _hoverableButton.sprite = hoveredSprite;
    }

    public void ButtonGreyOnMouseExit()
    {
        _hoverableButton.sprite = defaultSprite;
    }

    public void CloseParentUIOnClick()
    {
        AudioManager.Instance.PlayClickSound();
        _buttonUIContainer.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        InputManager.Instance.EnableInput();
    }
}
