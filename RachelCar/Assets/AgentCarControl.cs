using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof(CarController))]
    public class AgentCarControl : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use
        private CarAgent cAgent;

        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
            cAgent = GetComponent<CarAgent>();
        }


        private void FixedUpdate()
        {
            // pass the input to the car!
            /*float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");
#if !MOBILE_INPUT
            float handbrake = CrossPlatformInputManager.GetAxis("Jump");
            m_Car.Move(h, v, v, handbrake);
#else
            m_Car.Move(h, v, v, 0f);
#endif*/
           // m_Car.Move();

        }
        public void ControlMove(float[] commands)
        {
            m_Car.Move(commands[0], commands[1], commands[1], commands[2]);
        }
    }
}
