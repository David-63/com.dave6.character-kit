using Dave6.ObjectPoolingSystem;
using UnityEngine;
using UnityUtils.Timer;

namespace Dave6.CharacterKit.Handler.Combat
{
    public class RangeModule : IActionModule
    {
        float _FireRate = 200f;
        float _LastFireTime;
        public bool IsAvailable {get;}

        public void TryAction(BaseActionContext ctx, IActionAnimation anim)
        {
            if (Time.time - _LastFireTime < 60f / _FireRate) return;
            _LastFireTime = Time.time;
            Debug.Log("Action: 원거리 공격");

            ctx.AttackTimer.RestartTimer();

            // // 아이템 찾기
            // var curWeapon = m_Controller.equipHandler.selectedFirearm as Firearm;

            // // 연사속도 제어
            // if (Time.time - m_LastFireTime < 60f / curWeapon.GetFireRate()) return false;

            // m_LastFireTime = Time.time;

            // // 탄약 체크
            // if (curWeapon.TryConsume())
            // {
            //     // 투사체 생성
            //     ProjectileMover projectile = CreateProjectile(curWeapon);

            //     // 아이템에게 투사체 구성 요청 (Vector3 destPos)
            //     curWeapon.BuildProjectile(projectile, characterAimPoint);

            //     // 반동 강도는 아이템이 가지고 있어야함
            //     float amplitude = 6f;
            //     m_Controller.cameraHandler.PlayRecoil(amplitude);
            // }

            // 애니메이션 (성공 실패에 따라 달라져야함)
            anim.PlayAction("Firearm_Fire", true);

        }
        public bool IsFinished(BaseActionContext ctx)
        {
            var playerCtx = ctx as PlayerCombatContext;
            if (!playerCtx.AttackTimer.IsRunning) return true;
            //if (!playerCtx.IsFocus) return true;
            return false;
        }
        public void CleanupAction(BaseActionContext ctx)
        {
            ctx.AttackTimer.Stop();
        }
        

        // ProjectileMover CreateProjectile(Firearm firearm)
        // {
        //     GameObject projectileOjb = ObjectPoolService.Instance.Get(firearm.GetProjectilePrefab());
        //     var projectile = projectileOjb.GetComponent<ProjectileMover>();
        //     //projectile.BindOwner(m_Controller);
        //     return projectile;
        // }
    }

}