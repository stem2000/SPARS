using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace AvatarModel
{
    public class AvatarState
    {
        public AvatarStats AvatarStats;

        public RaycastHit SlopeHit;
        public Vector2 MoveDirection;
        public Vector2 Rotation;

        public ShouldMove ShouldMove;
        public ShouldAttack ShouldAttack;
        public MovementState MovementState;

        public Rigidbody Rigidbody;
        public GameObject Avatar;
        public MovementData MovementData;


        public AvatarState(GameObject avatar, AvatarStats avatarStats)
        {
            Avatar = avatar;
            Rigidbody = Avatar.GetComponent<Rigidbody>();
            AvatarStats = avatarStats.Clone();
            ShouldMove = new ShouldMove();
            ShouldAttack = new ShouldAttack();
            MovementState = new MovementState();
        }
        

        public MovementData TransferMovementData()
        {
            switch (MovementState.CurrentMoveType)
            {
                case MovementType.Run:
                    return GetRunData();
                default:
                    return GetRunData();
            }
        }


        private RunData GetRunData()
        {
            var convertedVector = new Vector3(MoveDirection.x, 0f, MoveDirection.y);
            return new RunData(Avatar.transform.TransformVector(convertedVector) * AvatarStats.RunSpeed);
        }
    }


    public class ShouldMove
    {
        public bool ShouldDash;
        public bool ShouldJump;
    }


    public class ShouldAttack
    {
        public bool ShouldShoot;
    }


    public class MovementState
    {
        public bool Grounded;
        public bool InRunOnSlope;
        public bool InFly;
        public bool InJump;
        public bool InDash;
        public bool InRun;

        public MovementType CurrentMoveType = MovementType.Run;


        public void AnalyzeState()
        {
     
        }

    }


    public enum MovementType
    {
        Run, Jump, Dash, Fly, RunOnSlope
    }


    public abstract class MovementData { }
    public class RunData: MovementData
    {
        public Vector3 velocity;

        public RunData(Vector3 velocity)
        {
            this.velocity = velocity;
        }
    }
}
