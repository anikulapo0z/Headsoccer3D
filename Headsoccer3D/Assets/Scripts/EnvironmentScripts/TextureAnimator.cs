using UnityEngine;

public class TextureAnimator : MonoBehaviour
{
    [SerializeField] private Texture[] treeTextures;
    [SerializeField] private float frameDuration = 0.1f;
    [SerializeField] private bool loop = true;
    [SerializeField] private int materialIndex = 0;

    private Renderer rend;
    private int currentFrame = 0;
    private float timer = 0f;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend == null)
        {
            Debug.LogError("TextureAnimator requires a Renderer component!");
            enabled = false;
            return;
        }

        if (treeTextures.Length == 0)
        {
            Debug.LogWarning("No textures assigned to TextureAnimator!");
            enabled = false;
            return;
        }

        // Set the initial texture
        SetTexture(0);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= frameDuration)
        {
            timer -= frameDuration;
            currentFrame++;

            if (currentFrame >= treeTextures.Length)
            {
                if (loop)
                {
                    currentFrame = 0;
                }
                else
                {
                    currentFrame = treeTextures.Length - 1;
                    enabled = false;
                    return;
                }
            }

            SetTexture(currentFrame);
        }
    }

    private void SetTexture(int frameIndex)
    {
        if (frameIndex >= 0 && frameIndex < treeTextures.Length)
        {
            rend.materials[materialIndex].mainTexture = treeTextures[frameIndex];
        }
    }

    public void Play()
    {
        enabled = true;
        currentFrame = 0;
        timer = 0f;
    }

    public void Stop()
    {
        enabled = false;
    }

    public void SetSpeed(float newDuration)
    {
        frameDuration = newDuration;
    }
}
