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
        Debug.Log("���� ���� ��ư Ŭ����!"); // �׽�Ʈ�� �α�
        SceneManager.LoadScene("Store");
    }

    public void QuitGame()
    {
        Debug.Log("���� ���� ��ư Ŭ����!"); // �׽�Ʈ�� �α�
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}
