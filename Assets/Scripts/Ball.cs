using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(DeactivateBall(1f));
    }

    IEnumerator DeactivateBall(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }

}
