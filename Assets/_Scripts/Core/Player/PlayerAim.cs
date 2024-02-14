using System;
using System.Collections;
using System.Collections.Generic;
using CustomInputs;
using Unity.Netcode;
using UnityEngine;

public class PlayerAim : NetworkBehaviour
{
    [SerializeField] private Transform m_turretTransform;
    [SerializeField] private Transform m_cannonTransform;
    [SerializeField] private InputReader m_inputReader;
    [SerializeField] private LayerMask m_worldLayer;
    [SerializeField] private float m_maxTurretRotationSpeed = 90f;
    [SerializeField] private float m_maxCannonRotationSpeed = 90f;
    [SerializeField] private float m_minElevation = -45f; // Minimum elevation angle
    [SerializeField] private float m_maxElevation = 45f;  // Maximum elevation angle

    private Quaternion m_targetTurretRotation;
    private Quaternion m_targetCannonRotation;

    private void LateUpdate()
    {
        if (!IsOwner) return;

        if (Camera.main != null)
        {
            Vector3 aimScreenPosition = m_inputReader.AimPosition;
            Camera mainCam = Camera.main;

            Ray aimWorldPos = mainCam.ScreenPointToRay(aimScreenPosition);

            if (Physics.Raycast(aimWorldPos, out RaycastHit hitInfo, 1000f, m_worldLayer))
            {
                Vector3 turretToTarget = hitInfo.point - m_turretTransform.position;
                turretToTarget.y = 0; // Limit rotation to vertical axis

                m_targetTurretRotation = Quaternion.LookRotation(turretToTarget);

                // Calculate direction to target
                Vector3 directionToTarget = hitInfo.point - m_cannonTransform.position;
                directionToTarget.y = 0f; // Ignore vertical component

                // Calculate rotation to face the target
                Quaternion targetCannonRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);

                // Clamp the rotation to the specified elevation limits
                float angleX = Mathf.Clamp(targetCannonRotation.eulerAngles.x, m_minElevation, m_maxElevation);
                targetCannonRotation = Quaternion.Euler(angleX, 0f, 0f);

                // Set the target cannon rotation
                m_targetCannonRotation = targetCannonRotation;
            }
        }

        // Interpolate turret rotation
        m_turretTransform.rotation = Quaternion.RotateTowards(m_turretTransform.rotation, m_targetTurretRotation, m_maxTurretRotationSpeed * Time.deltaTime);

        // Interpolate cannon rotation
        m_cannonTransform.localRotation = Quaternion.RotateTowards(m_cannonTransform.localRotation, m_targetCannonRotation, m_maxCannonRotationSpeed * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(m_turretTransform.position, m_turretTransform.position + m_turretTransform.forward);

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(m_cannonTransform.position, m_cannonTransform.position + m_cannonTransform.forward);
    }
}
