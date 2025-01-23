using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public string targetTag = "Xbox"; // Tag của object cần đưa vào Goal
    public GameObject explosionPrefab; // Prefab của hiệu ứng nổ
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra nếu object có tag đúng
        if (other.CompareTag("Item") && other.gameObject.name =="XBox")
        {
            StartCoroutine(Win());
            LVManager.Instance.SaveGame();
            ManagerSound.Instance.PlayVFXSound(3);
             // Spawn hiệu ứng nổ
            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            }
        }
    }
    IEnumerator  Win()
    {
        yield return new WaitForSeconds(3f);
        ManagerUI.Instance.OpenUI<Sucess>();
        Time.timeScale = 0;
    }

  
}
