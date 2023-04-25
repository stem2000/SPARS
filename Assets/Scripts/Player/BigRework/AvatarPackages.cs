using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AvatarModel
{
    public class MovementDataPackage
    {
        public MovementData data;
        public MovementType type;
        public bool stateChanged;
    }

    public class StateInfoPackage
    {
        public MovementType CurrentMoveState;
        public bool Grounded;
        public bool StateWasChanged;
    }

    public class StateUpdatePackage
    {
        public Vector3 MoveDirection = Vector3.zero;
        public Vector3 Normal = Vector3.zero;
        public Vector3 Rotation = Vector3.zero;

        public bool Grounded;
        public bool OnSlope;
        public bool ShouldDash;
        public bool ShouldJump;
        public bool ShouldShoot;
    }
}
