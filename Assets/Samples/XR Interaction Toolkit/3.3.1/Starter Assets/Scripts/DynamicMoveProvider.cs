using UnityEngine;
using UnityEngine.Assertions;
using Unity.XR.CoreUtils; // <-- Garante o mapeamento do XR Origin
using UnityEngine.XR.Interaction.Toolkit; // <-- RECOLOQUE ISSO: Corrige o erro do GetWorldPose!
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets
{
    public class DynamicMoveProvider : ContinuousMoveProvider
    {
        // Sua variável estática perfeita
        public static bool movimentoBloqueado = false;

        public enum MovementDirection
        {
            HeadRelative,
            HandRelative,
        }

        [Space, Header("Movement Direction")]
        [SerializeField]
        [Tooltip("Directs the XR Origin's movement when using the head-relative mode. If not set, will automatically find and use the XR Origin Camera.")]
        Transform m_HeadTransform;

        public Transform headTransform
        {
            get => m_HeadTransform;
            set => m_HeadTransform = value;
        }

        [SerializeField]
        [Tooltip("Directs the XR Origin's movement when using the hand-relative mode with the left hand.")]
        Transform m_LeftControllerTransform;

        public Transform leftControllerTransform
        {
            get => m_LeftControllerTransform;
            set => m_LeftControllerTransform = value;
        }

        [SerializeField]
        [Tooltip("Directs the XR Origin's movement when using the hand-relative mode with the right hand.")]
        Transform m_RightControllerTransform;

        public Transform rightControllerTransform
        {
            get => m_RightControllerTransform;
            set => m_RightControllerTransform = value;
        }

        [SerializeField]
        MovementDirection m_LeftHandMovementDirection;

        public MovementDirection leftHandMovementDirection
        {
            get => m_LeftHandMovementDirection;
            set => m_LeftHandMovementDirection = value;
        }

        [SerializeField]
        MovementDirection m_RightHandMovementDirection;

        public MovementDirection rightHandMovementDirection
        {
            get => m_RightHandMovementDirection;
            set => m_RightHandMovementDirection = value;
        }

        Transform m_CombinedTransform;
        Pose m_LeftMovementPose = Pose.identity;
        Pose m_RightMovementPose = Pose.identity;

        protected override void Awake()
        {
            base.Awake();

            m_CombinedTransform = new GameObject("[Dynamic Move Provider] Combined Forward Source").transform;
            m_CombinedTransform.SetParent(transform, false);
            m_CombinedTransform.localPosition = Vector3.zero;
            m_CombinedTransform.localRotation = Quaternion.identity;

            forwardSource = m_CombinedTransform;
        }
        
        protected override Vector3 ComputeDesiredMove(Vector2 input)
        {
            // Bloqueio seguro: se true, o analógico retorna velocidade zero
            if (movimentoBloqueado)
                return Vector3.zero;

            if (input == Vector2.zero)
                return base.ComputeDesiredMove(input);

            if (m_HeadTransform == null)
            {
                var xrOrigin = mediator.xrOrigin;
                if (xrOrigin != null)
                {
                    var xrCamera = xrOrigin.Camera;
                    if (xrCamera != null)
                        m_HeadTransform = xrCamera.transform;
                }
            }

            switch (m_LeftHandMovementDirection)
            {
                case MovementDirection.HeadRelative:
                    if (m_HeadTransform != null)
                        m_LeftMovementPose = m_HeadTransform.GetWorldPose(); // Agora ele vai achar o método!
                    break;

                case MovementDirection.HandRelative:
                    if (m_LeftControllerTransform != null)
                        m_LeftMovementPose = m_LeftControllerTransform.GetWorldPose();
                    break;

                default:
                    Assert.IsTrue(false, $"Unhandled {nameof(MovementDirection)}={m_LeftHandMovementDirection}");
                    break;
            }

            switch (m_RightHandMovementDirection)
            {
                case MovementDirection.HeadRelative:
                    if (m_HeadTransform != null)
                        m_RightMovementPose = m_HeadTransform.GetWorldPose();
                    break;

                case MovementDirection.HandRelative:
                    if (m_RightControllerTransform != null)
                        m_RightMovementPose = m_RightControllerTransform.GetWorldPose();
                    break;

                default:
                    Assert.IsTrue(false, $"Unhandled {nameof(MovementDirection)}={m_RightHandMovementDirection}");
                    break;
            }

            var leftHandValue = leftHandMoveInput.ReadValue();
            var rightHandValue = rightHandMoveInput.ReadValue();

            var totalSqrMagnitude = leftHandValue.sqrMagnitude + rightHandValue.sqrMagnitude;
            var leftHandBlend = 0.5f;
            if (totalSqrMagnitude > Mathf.Epsilon)
                leftHandBlend = leftHandValue.sqrMagnitude / totalSqrMagnitude;

            var combinedPosition = Vector3.Lerp(m_RightMovementPose.position, m_LeftMovementPose.position, leftHandBlend);
            var combinedRotation = Quaternion.Slerp(m_RightMovementPose.rotation, m_LeftMovementPose.rotation, leftHandBlend);
            m_CombinedTransform.SetPositionAndRotation(combinedPosition, combinedRotation);

            return base.ComputeDesiredMove(input);
        }
    }
}