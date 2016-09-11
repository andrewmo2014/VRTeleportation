using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ScenePlay : MonoBehaviour
{

    public Transform player;
    public ConstantForce token;

    public Animator additiveDissolveAnim;
    public AnimationClip additiveDissolveClip;

    private bool didEnd;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (!token && !didEnd)
        {

            didEnd = true;
            StartCoroutine(Restart());
        }

        if (didEnd) { return; }

        token.force = -player.up * Physics.gravity.magnitude;
    }

    private IEnumerator Restart()
    {

        additiveDissolveAnim.SetTrigger("Appear");

        yield return new WaitForSeconds(additiveDissolveClip.length);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}