using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
public class Reticle : MonoBehaviour
{

    private Light halo;
    public SteamVR_PlayArea playArea;

    // Use this for initialization
    void Start()
    {
        halo = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetColor(Color color)
    {

        halo.color = color;
        playArea.color = color;
    }

    public void ShowPlayArea(bool doShow)
    {
        playArea.enabled = doShow;
        playArea.gameObject.SetActive(doShow);
    }
}