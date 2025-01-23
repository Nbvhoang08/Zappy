using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
public class Sucess : UIItems
{
     private RectTransform _panelTransform;
    public Sprite OnVolume;
    public Sprite OffVolume;

    [SerializeField] private Image buttonImage;
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
    public void NextBtn()
    {
        Time.timeScale = 1;
        ManagerSound.Instance.PlayClickSound();
        StartCoroutine(NextSence());
    }
    public void LoadNextScene()
    {
        int lvIndex =  SceneManager.GetActiveScene().buildIndex + 1; // Tăng chỉ số level
        string nextSceneName = "LV" + lvIndex;

        // Kiểm tra xem scene tiếp theo có tồn tại hay không
        if (Application.CanStreamedLevelBeLoaded(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            // Nếu không tồn tại scene tiếp theo, quay về Home
            SceneManager.LoadScene("Home");
            ManagerUI.Instance.CloseAll();
            ManagerUI.Instance.OpenUI<LevelUI>();
        }
}
    IEnumerator NextSence()
    {
        yield return new WaitForSeconds(0.3f);
        LoadNextScene();
        ManagerUI.Instance.CloseUIDirectly<Sucess>();
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
        ManagerSound.Instance.PlayClickSound();
        UpdateButtonImage();
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
