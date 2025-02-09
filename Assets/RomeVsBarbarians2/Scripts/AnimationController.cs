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

    void Awake ()
    {
        startPos = meshRenderer.transform.localPosition;
        startScale = meshRenderer.transform.localScale;
    }

    public void SpriteAnimationChange(int animationindex)
    {
        meshRenderer.material = animtionMaterials[animationindex];
        state = animationindex;
    }

    public void SpriteTransform(Vector3 dir)
    {

        meshRenderer.transform.localPosition = dir;
       
        
    }


    public void SpriteReset()
    {

        meshRenderer.transform.localPosition = startPos;
        meshRenderer.transform.localScale = startScale;


    }
}
