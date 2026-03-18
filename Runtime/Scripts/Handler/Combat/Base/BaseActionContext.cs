using UnityEngine;
using UnityUtils.Timer;

namespace Dave6.CharacterKit.Handler.Combat
{
    public class BaseActionContext
    {
        public Timer AttackTimer;

        public EActionExitReason ExitReason;
    }
}

/*
// 의도 (FSM이 써줌)
        public ActionExitReason attackExitReason;
        public bool wantAttack;

        public bool attacking;

        public Timer attackTimer;

        public bool animAttackFinished;


        public Transform currentTarget;
        public Vector3 aimPoint;
        public Transform owner;

        public bool isBusy => attacking;


*/