using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InteractionInfoPanel : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private Text _infoText;
    [Header("Message Settings")]
    [SerializeField] private Text _msgText;
    [SerializeField] private float _showTime = 3f;

    private Coroutine _showMsgRountine;
        
    public void ToggleVisibility(bool value, string info = "")
    {
        _panel.gameObject.SetActive(value);
        _infoText.text = $"{info}";
    }

    public void ShowMessage(string msg)
    {
        if(_showMsgRountine != null)
        {
            _msgText.text = "";
            StopCoroutine(_showMsgRountine);
        }

        StartCoroutine(ShowMsgRoutnie(msg));
    }

    private IEnumerator ShowMsgRoutnie(string msg)
    {
        _msgText.text = msg;
        yield return new WaitForSecondsRealtime(_showTime);
        _msgText.text = "";
    }
}
