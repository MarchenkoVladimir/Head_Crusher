using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;
    private Vector3 originalPos;

    void Awake()
    {
        Instance = this;
        originalPos = transform.localPosition;
    }

    public void ShakeCamera(float duration, float magnitude)
    {
        StopAllCoroutines();
        StartCoroutine(DoShake(duration, magnitude));
    }

    System.Collections.IEnumerator DoShake(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Vector3 offset = Random.insideUnitSphere * magnitude;
            transform.localPosition = originalPos + new Vector3(offset.x, offset.y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}