using Dave6.CharacterKit.UnityUI.ItemSystem;
using Dave6.ItemSystem.Application.Container;
using UnityEngine;

namespace Dave6.CharacterKit.Handler.Loadout
{
    /// <summary>
    /// NPC도 로드아웃 구성할 수 있음;
    /// </summary>
    public abstract class BaseLoadout : MonoBehaviour
    {
        protected LoadoutRootContext _Context;
        protected ContainerService _Service;
    }
}