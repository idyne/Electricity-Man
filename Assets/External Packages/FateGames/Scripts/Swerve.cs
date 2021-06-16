using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FateGames
{
    public class Swerve
    {
        public Vector2 Anchor = Vector2.zero;
        public Vector2 Difference = Vector2.zero;
        public bool Active = false;

        public delegate void OnReleaseCallback();

        public OnReleaseCallback OnRelease;
    }
}