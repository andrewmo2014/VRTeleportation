using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour
{

    public float threshold;

    void OnTriggerStay(Collider col)
    {

        if (col.CompareTag("Token"))
        {

            float distSq = (col.transform.position - transform.position).sqrMagnitude;

            if (distSq <= Mathf.Pow(threshold, 2f))
            {
                Destroy(col.gameObject);
            }
        }
    }
}
