using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            ParticleSystem burstParticle = GameManager.Instance.BurstParticle;
            gameObject.SetActive(false);
            burstParticle.transform.position = transform.position;
            burstParticle.Play();
        }
    }
}
