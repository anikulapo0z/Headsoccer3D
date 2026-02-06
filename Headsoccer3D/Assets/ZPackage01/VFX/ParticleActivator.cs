using UnityEngine;
using System.Collections.Generic;

public class ParticleSystemInputActivator : MonoBehaviour
{
    [Header("Inputs")]

    [Tooltip("Keyboard key used for testing (ex: L)")]
    [SerializeField] private KeyCode keyboardTestKey = KeyCode.L;

    [Tooltip("Xbox controller B button (Joystick Button 1)")]
    [SerializeField] private KeyCode controllerButton = KeyCode.JoystickButton1;

    [Header("Particle Systems")]
    [SerializeField] private List<ParticleSystem> particleSystems = new List<ParticleSystem>();

    private void Update()
    {
        if (Input.GetKeyDown(keyboardTestKey) || Input.GetKeyDown(controllerButton))
        {
            PlayAllParticles();
        }
    }

    private void PlayAllParticles()
    {
        foreach (ParticleSystem ps in particleSystems)
        {
            if (ps == null) continue;

            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.Play();

            Debug.Log($"Particle played: {ps.name}");
        }
    }
}
