using UnityEngine;

namespace InterdimensionalGroceries.Scenes
{
    [RequireComponent(typeof(ParticleSystem))]
    public class PortalParticleSetup : MonoBehaviour
    {
        private void Awake()
        {
            ParticleSystem ps = GetComponent<ParticleSystem>();
            
            var main = ps.main;
            main.startLifetime = new ParticleSystem.MinMaxCurve(2f, 3f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(0.5f, 1.5f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.15f);
            main.startColor = new ParticleSystem.MinMaxGradient(new Color(1f, 1f, 1f, 0.8f));
            main.gravityModifier = -0.3f;
            main.maxParticles = 200;
            
            var emission = ps.emission;
            emission.rateOverTime = 50f;
            
            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 1.5f;
            shape.radiusThickness = 0.3f;
            
            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient grad = new Gradient();
            grad.SetKeys(
                new GradientColorKey[] { 
                    new GradientColorKey(Color.white, 0.0f), 
                    new GradientColorKey(Color.white, 1.0f) 
                },
                new GradientAlphaKey[] { 
                    new GradientAlphaKey(0.8f, 0.0f), 
                    new GradientAlphaKey(0f, 1.0f) 
                }
            );
            colorOverLifetime.color = new ParticleSystem.MinMaxGradient(grad);
            
            var sizeOverLifetime = ps.sizeOverLifetime;
            sizeOverLifetime.enabled = true;
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(0f, 1f);
            curve.AddKey(1f, 0.5f);
            sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, curve);
            
            var noise = ps.noise;
            noise.enabled = true;
            noise.strength = 0.5f;
            noise.frequency = 0.5f;
            noise.scrollSpeed = 0.2f;
            noise.damping = true;
            
            var renderer = ps.GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                renderer.renderMode = ParticleSystemRenderMode.Billboard;
                Material particleMat = Resources.Load<Material>("Materials/PortalParticle");
                if (particleMat == null)
                {
                    particleMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                    particleMat.SetColor("_BaseColor", new Color(1f, 1f, 1f, 0.8f));
                    particleMat.SetColor("_EmissionColor", new Color(1.5f, 1.5f, 1.5f, 1f));
                    particleMat.SetFloat("_Surface", 1);
                    particleMat.SetFloat("_Blend", 0);
                    particleMat.enableInstancing = true;
                }
                renderer.material = particleMat;
            }
        }
    }
}
