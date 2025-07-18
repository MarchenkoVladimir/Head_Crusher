using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
public class HeadDamage : MonoBehaviour
{
    public int maxHP = 100;
    public float cameraDuration = 0.01f;
    public float cameraMagnitude = 0.01f;
    private int currentHP;

    public ParticleSystem bloodEffect;
    public AudioSource hitSound;
    public TextMeshProUGUI hpText;
    public SkinnedMeshDeformer SkinnedMeshDeformer;

    private Rigidbody rb;
    private Renderer rend;
    
    private void Start()
    {
        currentHP = maxHP;
        rb = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();
        UpdateHPUI();
    }

    private void OnMouseDown()
    {
        TakeDamage(Random.Range(5, 15));
    }

    public void TakeDamage(int amount)
    {
        currentHP = Mathf.Max(currentHP - amount, 0);
        UpdateHPUI();

        rb.AddForce(Vector3.back * 3f + Vector3.up * 2f, ForceMode.Impulse);
        StartCoroutine(HitFlash());

        CameraShake.Instance.ShakeCamera(cameraDuration, cameraMagnitude);
        
        if (hitSound) hitSound.Play();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
           SkinnedMeshDeformer.Deform(hit.point);
        if (bloodEffect) Instantiate(bloodEffect, hit.point, Quaternion.identity);
        
        if (currentHP <= 0)
        {
            Destroy(gameObject, 1f);
        }
    }

    IEnumerator HitFlash()
    {
        rend.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        rend.material.color = Color.white;
    }

    void UpdateHPUI()
    {
        if (hpText != null)
            hpText.text = "HP: " + currentHP;
    }
}