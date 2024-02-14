using System;
using System.Collections;
using System.Collections.Generic;
using CustomInputs;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TankController : NetworkBehaviour
{
    #region Exposed members
    
    [Space(5)]
    [Header("Movements Settings")]
    public Vehicle m_vehicleData; // Reference to the Vehicle ScriptableObject

    [Header("Scene Refs")]
    [SerializeField] private InputReader m_inputReaderAsset;

    [Space(5)]
    [Header("Aiming Settings")]
    [SerializeField] private LayerMask m_worldLayer;

    #endregion
    
    public Transform m_tankBody;
    public Transform m_turretTransform;
    public Transform m_cannonTransform;
    public bool m_isSetup; 

    private Rigidbody playerRb;
    private Vector2 previousMovementInput;
    private Quaternion m_targetTurretRotation;
    private Quaternion m_targetCannonRotation;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        m_inputReaderAsset.MoveEvent += HandleMove;
        playerRb = GetComponent<Rigidbody>();
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        m_inputReaderAsset.MoveEvent -= HandleMove;
    }

    private void Update()
    {
        if (!IsOwner) return;
        
        if(!m_isSetup) return;
        
        // Rotate tank body
        float yRot = previousMovementInput.x * m_vehicleData.m_vehicleTurningRate * Time.deltaTime;
        m_tankBody.Rotate(0f, yRot, 0f);

        // Update turret and cannon rotation
        UpdateTurretAndCannonAim();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        
        if(!m_isSetup) return;
        
        // Move tank forward/backward
        playerRb.velocity = m_tankBody.transform.forward * (previousMovementInput.y * m_vehicleData.m_vehicleSpeed);
    }

    private void HandleMove(Vector2 movementInput)
    {
        previousMovementInput = movementInput;
    }

    private void UpdateTurretAndCannonAim()
    {
        if (Camera.main != null)
        {
            Vector3 aimScreenPosition = m_inputReaderAsset.AimPosition;
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
                float angleX = Mathf.Clamp(targetCannonRotation.eulerAngles.x, m_vehicleData.m_minElevation, m_vehicleData.m_maxElevation);
                targetCannonRotation = Quaternion.Euler(angleX, 0f, 0f);

                // Set the target cannon rotation
                m_targetCannonRotation = targetCannonRotation;
            }
        }
        
        // Interpolate turret rotation
        m_turretTransform.rotation = Quaternion.RotateTowards(m_turretTransform.rotation, m_targetTurretRotation, m_vehicleData.m_turretTurningRate * Time.deltaTime);

        // Interpolate cannon rotation
        m_cannonTransform.localRotation = Quaternion.RotateTowards(m_cannonTransform.localRotation, m_targetCannonRotation, m_vehicleData.m_cannonRotationSpeed * Time.deltaTime);
    }
}
