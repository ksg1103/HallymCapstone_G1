
using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;

    public AudioSource bgmSource;
    public AudioClip[] bgmClips;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        //SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        // ���� �� �������� ������ �� �� BGM ���
        PlayBGMForScene(SceneManager.GetActiveScene().name);
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayBGMForScene(scene.name);
    }

    void PlayBGMForScene(string sceneName)
    {
        // ����: �� �̸��� ���� �ٸ� BGM ���
        switch (sceneName)
        {
            case "Title":
                Play(bgmClips[0]);
                break;
            case "Store":
                Play(bgmClips[1]);
                break;
            case "BossScene":
                Play(bgmClips[2]);
                break;
            case "Finalboss":
                Play(bgmClips[3]);
                break;
            
            default:
                Play(null);
                break;
        }
    }

    public void Play(AudioClip clip)
    {
        if (clip == null) return;

        if (bgmSource.clip == clip) return;

        bgmSource.Stop();
        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void Stop()
    {
        bgmSource.Stop();
    }
}
