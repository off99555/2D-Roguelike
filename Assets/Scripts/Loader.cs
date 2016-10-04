using UnityEngine;

public class Loader : MonoBehaviour
{
    public GameManager gameManager;
    public SoundManager soundManager;

    private void Awake()
    {
        if (GameManager.instance == null)
        {
            Instantiate(gameManager);
        }
        if (SoundManager.instance == null)
        {
            Instantiate(soundManager);
        }
    }
}