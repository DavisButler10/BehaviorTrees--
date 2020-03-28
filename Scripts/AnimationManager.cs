using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void SetAnimOff()
    {
        anim.SetBool("stopAnim", true);
    }

    public void SetAnimOn()
    {
        anim.SetBool("stopAnim", false);
    }

    public void SetDedOff()
    {
        anim.SetBool("ded", false);
        anim.SetBool("stopAnim2", false);
    }
}
