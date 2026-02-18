using UnityEngine;
using System.Collections;

namespace InterdimensionalGroceries.Core
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(HingeJoint))]
    public class SwingingDoor : MonoBehaviour
    {
        [Header("Rotation Settings")]
        [SerializeField] private float maxRotationAngle = 90f;
        [SerializeField] private Vector3 hingeAxis = Vector3.up;

        [Header("Push Settings")]
        [SerializeField] private float pushForce = 50f;
        [SerializeField] private float pushTorque = 30f;

        [Header("Auto-Close Settings")]
        [SerializeField] private float autoCloseDelay = 2f;
        [SerializeField] private float springStrength = 30f;
        [SerializeField] private float springDamper = 5f;

        [Header("Sound Settings")]
        [SerializeField] private float soundTriggerAngle = 5f;

        private HingeJoint hingeJoint;
        private Rigidbody rb;
        private bool hasSoundPlayed;
        private Vector3 hingeWorldPosition;
        private Coroutine autoCloseCoroutine;
        private float lastAngle;
        private float timeSinceLastMovement;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            hingeJoint = GetComponent<HingeJoint>();
        }

        private void Start()
        {
            ConfigureHingeJoint();
        }

        private void ConfigureHingeJoint()
        {
            hingeJoint.axis = hingeAxis;
            
            hingeJoint.useLimits = true;
            JointLimits limits = hingeJoint.limits;
            limits.min = -maxRotationAngle;
            limits.max = maxRotationAngle;
            limits.bounciness = 0f;
            hingeJoint.limits = limits;

            hingeJoint.useSpring = false;
        }

        private void Update()
        {
            float currentAngle = hingeJoint.angle;
            
            if (Mathf.Abs(currentAngle) > soundTriggerAngle && !hasSoundPlayed)
            {
                PlayDoorSound();
                hasSoundPlayed = true;
            }
            
            if (Mathf.Abs(currentAngle) < soundTriggerAngle)
            {
                hasSoundPlayed = false;
            }

            hingeWorldPosition = transform.TransformPoint(hingeJoint.anchor);

            if (Mathf.Abs(currentAngle - lastAngle) > 0.5f)
            {
                timeSinceLastMovement = 0f;
                
                if (hingeJoint.useSpring)
                {
                    hingeJoint.useSpring = false;
                }
            }
            else
            {
                timeSinceLastMovement += Time.deltaTime;
                
                if (timeSinceLastMovement >= autoCloseDelay && !hingeJoint.useSpring && Mathf.Abs(currentAngle) > 1f)
                {
                    EnableSpring();
                }
            }
            
            lastAngle = currentAngle;
        }

        private void EnableSpring()
        {
            JointSpring spring = hingeJoint.spring;
            spring.spring = springStrength;
            spring.damper = springDamper;
            spring.targetPosition = 0f;
            hingeJoint.spring = spring;
            hingeJoint.useSpring = true;
        }

        private void PlayDoorSound()
        {
        }
    }
}
