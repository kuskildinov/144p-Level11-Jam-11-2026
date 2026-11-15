using UnityEngine;
using UnityEngine.UI;

public class Furnace : Interactable
{

    [Header("VFX")]
    [SerializeField] private ParticleSystem _particles;
    [Header("Notes")]
    [SerializeField] private Text _noteText;
    [Header("other")]
    [SerializeField] private Letter _letter;
    [SerializeField] private Shovle _shovle;
    [SerializeField] private Lamp _lamp;
    [SerializeField] private Coal _coal;
    [SerializeField] private GameObject _light;
    [SerializeField] private GameObject _letterLight;

    private RoomRoot _root;
    private Player _player;
    private int _shovleCount;

    public override void Initialize(RoomRoot root)
    {
        _root = root;
        _shovleCount = 0;
    }

    public override void Interact(Player player)
    {
        _player = player;

        if (player.IsShovleOnHand && !player.Shovle.IsEmpty)
        {
            OnCoalPutOn();
            player.RemoveCoal();
        }
        else
        {
            player.ShowInteractionMessage("Ãîðÿ÷î. Íóæíà ëîïàòà.");
        }
    }

    private void OnCoalPutOn()
    {
        _shovleCount++;
        _particles.Play();

        PlayCommentByIndex();
        CheckSwitchOn();
    }

    public void UpdateNote()
    {
        switch (_shovleCount)
        {
            case 1:
                {
                    _noteText.text = "ÍÀÑÛÏÀÒÜ ÓÃËß Â ÏÅ×Ü";
                    break;
                }
            case 2:
                {
                    _noteText.text = "ÄÀÒÜ ÒÀÁËÅÒÊÓ ÏÅÐÅÄ ÑÍÎÌ";
                    break;
                }
            case 3:
                {
                    _noteText.text = "ÄÀÒÜ ÄÂÅ ÒÀÁËÅÒÊÈ ÏÅÐÅÄ ÑÍÎÌ";
                    break;
                }
            case 4:
                {
                    _noteText.text = "...";
                    break;
                }
            case 5:
                {
                    _noteText.text = "ÏÎÑÀÄÈÒÜ ÖÂÅÒÎÊ Ó ÎÃÐÀÄÊÈ";
                    break;
                }
            case 6:
                {
                    _noteText.text = "ÏÎ×ÈÑÒÈÒÜ ÒÐÎÏÓ Ê ÌÎÃÈËÊÅ";
                    break;
                }
            case 7:
                {
                    _noteText.text = "ÇÀÆÅ×Ü ÌÀßÊ";
                    break;
                }
            default:
                break;
        }
    }

    private void PlayCommentByIndex()
    {
        switch (_shovleCount)
        {
            case 1:
                {
                    break;
                }
            case 2:
                {

                    break;
                }
            case 3:
                {
                    break;
                }
            case 4:
                {
                    break;
                }
            case 5:
                {
                    break;
                }
            case 6:
                {
                    break;
                }
            case 7:
                {
                    break;
                }
            default:
                break;
        }
    }

    private void CheckSwitchOn()
    {
        if (_shovleCount > 7)
        {
            RemoveShovle();
            RemoveLamp();
            ActivateLetter();
            DeactivateCoal();

            _light.gameObject.SetActive(true);
            _letterLight.gameObject.SetActive(true);
        }
    }

    private void RemoveShovle()
    {
        _player.ToggleShovleVisibility(false);
        _shovle.gameObject.SetActive(true);
        _shovle.IsActive = false;
    }

    private void RemoveLamp()
    {
        _player.ToggleLampLight(false);
        _lamp.gameObject.SetActive(true);
        _lamp.IsActive = false;
    }

    private void ActivateLetter()
    {
        _letter.IsActive = true;
    }

    private void DeactivateCoal()
    {
        _coal.IsActive = false;
    }

    public override void Dispose()
    {

    }
}
