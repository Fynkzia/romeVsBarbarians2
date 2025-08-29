using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudsFade : MonoBehaviour
{

    [SerializeField] public MeshRenderer[] clouds;
    [SerializeField] public bool doFade;

    [SerializeField] public bool fadeOut;

    [SerializeField] private float fadeSpeed;
    [SerializeField] private float timeFade;
    [SerializeField] private float currentTime = 0f;

    [SerializeField] private BoxCollider collider;

    Material instMat;

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        if (doFade)
        {
           

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

                if (instMat.color.a >= 0.75f)
                {
                    doFade = false;
                }
            }

           




         }

        if (fadeOut)
        {
            currentTime += Time.deltaTime;

            if (timeFade <= currentTime)
            {
                currentTime = 0;
                CloudsFadeSet(false);

            }
        }

    }

    // Update is called once per frame
    public void CloudsFadeSet(bool fade)
    {
        if (!doFade || fade)
        {
            fadeOut = fade;
            doFade = true;
            collider.enabled = !fade;


            if (instMat == null)
            {
               

                instMat = new Material(clouds[0].material);

                for (int i = 0; i < clouds.Length; i++)
                {

                    clouds[i].material = instMat;


                }
            }
        }
    }
}
