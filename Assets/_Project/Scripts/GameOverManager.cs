using System.Collections;
using UnityEngine;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private TMP_Text gameOverUI;

    private Cookie[] _requiredJars;
    private Transform _transform;
    private int _childCount;
    private int _jarsCollected = 0;

    private void Awake()
    {
        _transform = transform;
        _childCount = _transform.childCount;

        _requiredJars = new Cookie[_childCount];

        gameOverUI.gameObject.SetActive(false);

        for (int i = 0; i < _childCount; i++)
        {
            _requiredJars[i] = _transform.GetChild(i).GetComponent<Cookie>();
        }

    }

    private void OnEnable()
    {
        for (int i = 0; i < _childCount; i++)
        {
            _requiredJars[i].OnSpecialCookieCollect += UpdateCount;
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < _childCount; i++)
        {
            _requiredJars[i].OnSpecialCookieCollect -= UpdateCount;
        }
    }

    private IEnumerator GameOverRoutine()
    {
        gameOverUI.gameObject.SetActive(true);
        AudioManager.Instance.PlayVictorySound();
        yield return new WaitForSeconds(10f);
        gameOverUI.text = "You can now go and explore the rest of the Island.";
        yield return new WaitForSeconds(5f);
        gameOverUI.text = "Or just exit the game.";
        yield return new WaitForSeconds(2f);
        gameOverUI.gameObject.SetActive(false);
    }

    private void UpdateCount()
    {
        _jarsCollected++;
        if (_jarsCollected == _childCount)
        {
            StartCoroutine(GameOverRoutine());
        }
    }
}
