using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] public List<Material> animtionMaterials;
    [SerializeField] public int state;


    [SerializeField] public MeshRenderer meshRenderer;
    // Start is called before the first frame update
   
    public void SpriteAnimationChange(int animationindex)
    {
        meshRenderer.material = animtionMaterials[animationindex];
        state = animationindex;
    }
}
