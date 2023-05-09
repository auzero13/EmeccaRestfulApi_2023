using System.Collections.Generic;
using com.emecca.model;


namespace com.emecca.service
{

    public class MenuServiceImpl : EmeccaService, IMenuService
    {
        protected override object GetServiceInstance()
        {
            // Here you can get the instance of your MenuService
            return new MenuServiceImpl();
        }
        [MethodAlias("getmenu")]
        public List<MenuModel> GetMenus()
        {
            return new List<MenuModel>()
            {
                new MenuModel
                {
                    Id = 1,
                    Name = "Menu1",
                    Desc = "Menu1",
                    Menus = new List<MenuModel>()
                    {
                        new MenuModel
                        {
                            Id = 2,
                            Name = "Menu2",
                            Desc = "Menu2",
                            MenuItems = new List<MenuItemModel>()
                            {
                                new MenuItemModel
                                {
                                    Id = 3,
                                    Name = "MenuItem3",
                                    Desc = "MenuItem3",
                                    Url = "/menu1/menu2/menuitem3"
                                },
                                new MenuItemModel
                                {
                                    Id = 4,
                                    Name = "MenuItem4",
                                    Desc = "MenuItem4",
                                    Url = "/menu1/menu2/menuitem4"
                                }
                            }
                        }
                    }
                },
                new MenuModel
                {
                    Id = 5,
                    Name = "Menu5",
                    Desc = "Menu5",
                    MenuItems = new List<MenuItemModel>()
                    {
                        new MenuItemModel
                        {
                            Id = 6,
                            Name = "DeletePacsApply",
                            Desc = "刪除申請",
                            Url = "/"
                        },
                        new MenuItemModel
                        {
                            Id = 7,
                            Name = "ApplyCheck",
                            Desc = "審核申請",
                            Url = "/applycheck"
                        }
                    }
                }
            };
        }
    }
}