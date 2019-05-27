using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class TriggerParticleSystem : MonoBehaviour
{
    public void PlayParticles(){
		GetComponent<ParticleSystem>().Play();
	}
}
