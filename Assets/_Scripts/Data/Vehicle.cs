using UnityEngine;

[CreateAssetMenu(fileName = "New Vehicle Data", menuName = "Data/Vehicle Data")]
public class Vehicle : ScriptableObject
{
    // Name of the vehicle
    public string m_vehicleName;
    
    // Icon representing the vehicle
    public Sprite m_vehicleIcon;
    
    // Prefab used for spawning the PlayerNObject vehicle
    public TankModel m_tankModel;
    
    // Speed of the vehicle
    public float m_vehicleSpeed;
    
    // Turning rate of the vehicle (for movement)
    public float m_vehicleTurningRate;
    
    // Damage per second inflicted by the vehicle (if applicable)
    public float m_damagePerSec;
    
    // Turning rate of the turret (for aiming)
    public float m_turretTurningRate;
    
    // Minimum elevation angle of the cannon (for aiming)
    public float m_minElevation;
    
    // Maximum elevation angle of the cannon (for aiming)
    public float m_maxElevation;
    
    // Speed of cannon rotation (for aiming)
    public float m_cannonRotationSpeed;

    // Additional variables related to the vehicle can be added here with appropriate comments
}