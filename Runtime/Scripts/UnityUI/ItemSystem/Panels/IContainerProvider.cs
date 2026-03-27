using System.Collections.Generic;
using Dave6.ItemSystem.Application.Container;
using Dave6.ItemSystem.Domain.Container;

namespace Dave6.CharacterKit.UnityUI.ItemSystem
{
    public interface IContainerProvider
    {
        IEnumerable<IItemContainer> GetRootContainers();
        RootContainerContext GetLoadoutContext();
    }
}
