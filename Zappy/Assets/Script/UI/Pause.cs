using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Pause : UIItems
{
     // Start is called before the first frame update
    public Sprite OnVolume;
    public Sprite OffVolume;
    
    [SerializeField] private Image buttonImage;
    private RectTransform _panelTransform;
    private void Awake()
    {
        _panelTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        // Đặt kích thước ban đầu và vị trí giữa màn hình
        _panelTransform.localScale = Vector3.zero;
        _panelTransform.DOScale(Vector3.one, 0.5f)
            .SetEase(Ease.OutBack)
            .SetUpdate(true); // Bỏ qua Time.timeScale
    }  
    void Update()
    {
        UpdateButtonImage();
    }
    public void Resume()
    {
        Time.timeScale = 1;
        ManagerSound.Instance.PlayClickSound();
        ManagerUI.Instance.CloseUI<Pause>(0.2f);
            
    }
    public void HomeBtn()
    {
        
        Time.timeScale = 1;
        StartCoroutine(LoadHome());
        ManagerSound.Instance.PlayClickSound();
    }
    IEnumerator LoadHome()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("Home");
        ManagerUI.Instance.CloseAll();
        ManagerUI.Instance.OpenUI<LevelUI>();
    }
    public void SoundBtn()
    {
        ManagerSound.Instance.TurnOn = !ManagerSound.Instance.TurnOn;
        UpdateButtonImage();
        ManagerSound.Instance.PlayClickSound();
    }
    private void UpdateButtonImage()
    {
        if (ManagerSound.Instance.TurnOn)
        {
            buttonImage.sprite = OnVolume;
        }
        else
        {
            buttonImage.sprite = OffVolume;
        }
    }   
}
