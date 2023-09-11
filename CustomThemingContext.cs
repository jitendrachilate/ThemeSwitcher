using System.Web;
using Sitecore.Data;
using Sitecore.XA.Foundation.Theming;
using Sitecore.Data.Items;

namespace ThemeSwitcher
{
    public class CustomThemingContext : ThemingContext
    {
        private const string CookieName = "Default-Theme";

        protected override Item DoGetThemeItem(Item item, DeviceItem device)
        {
            HttpCookie cookie = null;
            cookie = cookie ?? HttpContext.Current.Request.Cookies.Get(CookieName);
            Item defaultThemeItem = Sitecore.Context.Database.GetItem(new ID(Templates.Themes.DefaultTheme));
            Item newThemeItem = Sitecore.Context.Database.GetItem(new ID(Templates.Themes.NewTheme));
            return (cookie?.Value == "1") ? newThemeItem : defaultThemeItem;
              
        }
    }
}