using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePlayParticle : MonoBehaviour
{
    public ParticleSystem[] psArr;

    public void PlayParticle()
    {
        for (int i = 0; i < psArr.Length; ++i)
        {
            var ps = psArr[i];
            if (ps != null)
            {
                ps.Play();
            }
        }
    }
}
