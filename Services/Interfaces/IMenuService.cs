using com.emecca.model;
using System.Collections.Generic;

namespace com.emecca.service
{
   [ServiceAlias("menuService")]
    public interface IMenuService : IEmeccaService
    {
        
        List<MenuModel> GetMenus();
        //void AddMenu(MenuModel menu);
    }
}
