using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StopPanel : MonoBehaviour
{
    public GameObject zhiZhen;  //指针UI Image
    void OnEnable()
    {
        Time.timeScale = 0;
    }
    void OnDisable()
    {
        Time.timeScale = 1;
    }
    void Update()
    {
        ZhiZhenController();
    }
    public void ContinueGame()
    {
        this.gameObject.SetActive(false);
    }
    public void Quit()
    {
        SceneManager.LoadScene("主场景");
    }
    private void ZhiZhenController()
    {
        Vector2 mousePos = Input.mousePosition;
        RectTransform rect = zhiZhen.GetComponent<RectTransform>();
        Vector2 zzPos = RectTransformUtility.WorldToScreenPoint(null,rect.position); //指针位置
        Vector2 direction = mousePos-zzPos;
        float angle = Mathf.Atan2(direction.y,direction.x)*Mathf.Rad2Deg-90f;
        rect.rotation = Quaternion.Euler(0,0,angle);
    }
}
