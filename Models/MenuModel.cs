namespace com.emecca.model
{
    public class MenuModel
    {
        public MenuModel()
        {
        }

        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Desc { get; set; }
        public List<MenuModel>? Menus { get; set; }
        public List<MenuItemModel>? MenuItems { get; set; }
    }

    public class MenuItemModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Desc { get; set; }
        public string? Url { get; set; }
    }
}
