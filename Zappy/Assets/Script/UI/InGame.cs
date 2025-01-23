using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class InGame : UIItems
{
    [SerializeField] private Text LevelName;
    // Start is called before the first frame update
    public void MoveUp() => PlayerMove.Instance.StartMove(Vector2.up);
    public void MoveDown() => PlayerMove.Instance.StartMove(Vector2.down);
    public void MoveLeft() => PlayerMove.Instance.StartMove(Vector2.left);
    public void MoveRight() => PlayerMove.Instance.StartMove(Vector2.right);
    public void StopMove() => PlayerMove.Instance.StopMove();
    public void PickOrDropItem() => PlayerMove.Instance.PickOrDropItem();
    public void Pause()
    {
        Time.timeScale = 0;
        ManagerUI.Instance.OpenUI<Pause>();
        ManagerSound.Instance.PlayClickSound();
    }
    void Update()
    {
        UpdateLevelText();
    }
    private void UpdateLevelText()
    {
        if (LevelName != null)
        {   
            int levelNumber = SceneManager.GetActiveScene().buildIndex;
            LevelName.text = $"Level: {levelNumber:D2}"; // Hiển thị với 2 chữ số, ví dụ: 01, 02
        }   
    }   
}
