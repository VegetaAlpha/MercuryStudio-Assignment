using UnityEngine;
using UnityEngine.UI;

public class InstructionPopup : MonoBehaviour
{
    [SerializeField] Button _okBtn;
    [SerializeField] CanvasGroup _canvasGroup;

    private void Awake()
    {
        SetEnable(true);
        _okBtn.onClick.AddListener(() => SetEnable(false));
    }

    private void OnDestroy()
    {
        _okBtn.onClick.RemoveAllListeners();
    }

    private void SetEnable(bool value)
    {
        if (value)
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
        }
        else
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
        }
    }
}
