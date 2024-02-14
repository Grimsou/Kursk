using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankModel : MonoBehaviour
{
    [SerializeField] private Transform m_tankBody;
    [SerializeField] private Transform m_turretTransform;
    [SerializeField] private Transform m_cannonTransform;

    public void SetupPlayerTank(TankController controller)
    {
        controller.m_tankBody = m_tankBody;
        controller.m_turretTransform = m_turretTransform;
        controller.m_cannonTransform = m_cannonTransform;
    }
}
