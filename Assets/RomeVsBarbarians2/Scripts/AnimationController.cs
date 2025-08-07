using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] public List<Material> animtionMaterials;
    [SerializeField] public int state;


    [SerializeField] public MeshRenderer meshRenderer;
    [SerializeField] public Vector3 startPos;
    [SerializeField] public Vector3 startScale;
    // Start is called before the first frame update


     Material damageMaterial;
    bool damageAnimation;

    [SerializeField] public float shakeIntensity = 0.5f; // Амплитуда тряски
    [SerializeField] public float shakeFrequency = 200f; // Частота тряски
    private float shakeOffset; // Временной сдвиг для перлин шума


    void Awake ()
    {
        startPos = meshRenderer.transform.localPosition;
        startScale = meshRenderer.transform.localScale;
    }

    public void SpriteAnimationChange(int animationindex)
    {
        
        state = animationindex;

        if (!damageAnimation)
        {

            meshRenderer.material = animtionMaterials[animationindex];

            if (animationindex == 7 || animationindex == 8) // дамаг
            {
                damageMaterial = new Material(animtionMaterials[animationindex]);
                meshRenderer.material = damageMaterial;
                damageAnimation = true;
                StartCoroutine(FlashWhite());
            }
        }
    }

    IEnumerator FlashWhite()
    {
        damageMaterial.SetFloat("_WhiteAmount", 1f);
        yield return new WaitForSeconds(0.2f); // Длительность эффекта
        damageMaterial.SetFloat("_WhiteAmount", 0.8f);
        yield return new WaitForSeconds(0.1f); // Длительность эффекта
        damageMaterial.SetFloat("_WhiteAmount", 0.6f);
        yield return new WaitForSeconds(0.1f); // Длительность эффекта
        damageMaterial.SetFloat("_WhiteAmount", 0.3f);
        yield return new WaitForSeconds(0.1f); // Длительность эффекта
        damageMaterial.SetFloat("_WhiteAmount", 0f);

        meshRenderer.material = animtionMaterials[state];
        damageAnimation = false;
        Destroy(damageMaterial);
            
    }

    public void SpriteTransform(Vector3 dir)
    {

        meshRenderer.transform.localPosition = dir;
       
        
    }

    public void ShakeAnimation(float intesity)
    {
        Debug.Log("ShakeAnimation = " + intesity);

        float time = Time.time * shakeFrequency + shakeOffset;

        time *= intesity;

        // Генерация шума по X и Y
        float offsetX = (Mathf.PerlinNoise(time, 0f) - 0.5f) * shakeIntensity * 2f;
        float offsetY = (Mathf.PerlinNoise(0f, time) - 0.5f) * shakeIntensity * 2f;

        // Применяем тряску
        meshRenderer.transform.localPosition = startPos + new Vector3(offsetX, 0, offsetY);


    }

    

    public void SpriteReset()
    {

        meshRenderer.transform.localPosition = startPos;
       // //meshRenderer.transform.localScale = startScale;


    }
}
