using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class ParticlesWhileMovingAndHolding : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private KeyCode keyboardTestKey = KeyCode.L;
    [SerializeField] private KeyCode controllerButton = KeyCode.JoystickButton1; // Xbox B

    [Header("Movement Detection")]
    [SerializeField] private float movementThreshold = 0.05f;

    [Header("Particle Systems")]
    [SerializeField] private List<ParticleSystem> particleSystems = new List<ParticleSystem>();

    private CharacterController controller;
    private bool particlesPlaying;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        bool buttonHeld =
            Input.GetKey(keyboardTestKey) ||
            Input.GetKey(controllerButton);

        bool isMoving = controller.velocity.magnitude > movementThreshold;

        if (buttonHeld && isMoving)
        {
            PlayParticles();
        }
        else
        {
            StopParticles();
        }
    }

    private void PlayParticles()
    {
        if (particlesPlaying) return;

        foreach (ParticleSystem ps in particleSystems)
        {
            if (ps == null) continue;
            ps.Play();
        }

        particlesPlaying = true;
        Debug.Log("Particles playing (held + moving)");
    }

    private void StopParticles()
    {
        if (!particlesPlaying) return;

        foreach (ParticleSystem ps in particleSystems)
        {
            if (ps == null) continue;
            ps.Stop();
        }

        particlesPlaying = false;
    }
}
