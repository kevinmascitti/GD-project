using UnityEngine;

public class ScaleParticleSystem : MonoBehaviour
{
    [SerializeField] private float scaleMultiplier = 10.0f; // Fattore di scala

    void Start()
    {
        ScaleVFX(scaleMultiplier);
    }

    void ScaleVFX(float scale)
    {
        // Ottieni tutti i componenti ParticleSystem nel prefab
        ParticleSystem[] particleSystems = GetComponents<ParticleSystem>();

        foreach (ParticleSystem ps in particleSystems)
        {
            var mainModule = ps.main;
            mainModule.startSize = new ParticleSystem.MinMaxCurve(mainModule.startSize.constantMin * scale, mainModule.startSize.constantMax * scale);
            mainModule.startSpeed = new ParticleSystem.MinMaxCurve(mainModule.startSpeed.constantMin * scale, mainModule.startSpeed.constantMax * scale);
            
            // Se il modulo Shape è utilizzato, puoi anche scalare le dimensioni
            var shapeModule = ps.shape;
            shapeModule.scale *= scale;
        }
    }
}