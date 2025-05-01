using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void StartGame()
    {
        Debug.Log("게임 시작 버튼 클릭됨!"); // 테스트용 로그
        SceneManager.LoadScene("Store");
    }

    public void QuitGame()
    {
        Debug.Log("게임 종료 버튼 클릭됨!"); // 테스트용 로그
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}
