using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FateGames
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance;

        private Swerve swerve;

        private void Awake()
        {
            if (Instance)
                return;
            Instance = this;
            swerve = new Swerve();
        }

        private void Update()
        {
            CheckSwerve();
        }
        private void CheckSwerve()
        {

            if (Input.GetMouseButtonDown(0))
            {
                swerve.Active = true;
                swerve.Anchor = Input.mousePosition;
            }
            else if (Input.GetMouseButton(0))
                swerve.Difference = (Vector2)Input.mousePosition - swerve.Anchor;
            else if (Input.GetMouseButtonUp(0))
            {
                swerve.Active = false;
                swerve.OnRelease();
            }
        }

        public Swerve Swerve
        {
            get
            {
                return swerve;
            }
        }
    }
}