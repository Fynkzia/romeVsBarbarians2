using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudsFade : MonoBehaviour
{

    [SerializeField] public ParticleSystem[] clouds;
    [SerializeField] public bool doFade;

    [SerializeField] public bool fadeOut;

    [SerializeField] private float fadeSpeed;
    [SerializeField] private float timeToFade;
    [SerializeField] private float currentTime = 0f;

    Material instMat;

    // Start is called before the first frame update
    void Start()
    {
        //CloudsFadeSet(true);
    }

    private void Update()
    {
        if (doFade)
        {
            //currentTime += Time.deltaTime;
            if (fadeOut)
            {
                instMat.color += new Color(0, 0, 0, -fadeSpeed * Time.deltaTime);

                if (instMat.color.a <= 0)
                {
                    doFade = false;
                }
            }
            else
            {
                instMat.color += new Color(0, 0, 0, fadeSpeed * Time.deltaTime);

                if (instMat.color.a >= 0.5f)
                {
                    doFade = false;
                }
            }

           

            

         }
        
    }

    // Update is called once per frame
    public void CloudsFadeSet(bool fade)
    {
        if (!doFade)
        {
            fadeOut = fade;
            doFade = true;

            if (instMat == null)
            {
                ParticleSystemRenderer render = clouds[0].GetComponent<ParticleSystemRenderer>();

                instMat = new Material(render.sharedMaterial);

                for (int i = 0; i < clouds.Length; i++)
                {

                    clouds[i].GetComponent<ParticleSystemRenderer>().sharedMaterial = instMat;


                }
            }
        }
    }
}
