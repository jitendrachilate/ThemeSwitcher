using Microsoft.Extensions.DependencyInjection;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.Pipelines;
using Sitecore.XA.Foundation.Abstractions;
using Sitecore.XA.Foundation.SitecoreExtensions.Comparers;
using Sitecore.XA.Foundation.SitecoreExtensions.Extensions;
using Sitecore.XA.Foundation.SitecoreExtensions.Repositories;
using Sitecore.XA.Foundation.Theming;
using Sitecore.XA.Foundation.Theming.Bundler;
using Sitecore.XA.Foundation.Theming.Configuration;
using Sitecore.XA.Foundation.Theming.EventHandlers;
using Sitecore.XA.Foundation.Theming.Extensions;
using Sitecore.XA.Foundation.Theming.Pipelines.AssetService;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Caching;


namespace ThemeSwitcher
{
    public class CustomAssetLinksGenerator
    {
        private readonly TreeComparer _comparer;
        private readonly AssetConfiguration _configuration;
        private const string CookieName = "Default-Theme";
        protected IContentRepository ContentRepository { get; }
        protected IDatabaseRepository DatabaseRepository { get; }
        protected IContext Context { get; }
        protected IAssetBundler AssetBundler { get; }
        public CustomAssetLinksGenerator()
        {
            this._comparer = new TreeComparer();
            this._configuration = AssetConfigurationReader.Read();
            this.ContentRepository = ServiceLocator.ServiceProvider.GetService<IContentRepository>();
            this.DatabaseRepository = ServiceLocator.ServiceProvider.GetService<IDatabaseRepository>();
            this.Context = ServiceLocator.ServiceProvider.GetService<IContext>();
            this.AssetBundler = ServiceLocator.ServiceProvider.GetService<IAssetBundler>();
            this.InitializeCache();
            
        }
        public static AssetLinks GenerateLinks(IThemesProvider themesProvider) => AssetContentRefresher.IsPublishing() || CustomAssetLinksGenerator.IsAddingRendering() ? new AssetLinks() : new CustomAssetLinksGenerator().GenerateAssetLinks(themesProvider);
        protected static bool IsAddingRendering() => ((IEnumerable<string>)new string[3]
        {
      "sc_co",
      "sc_phk",
      "sc_ruid"
        }).All<string>(new Func<string, bool>((HttpUtility.ParseQueryString(ServiceLocator.ServiceProvider.GetService<IContext>().Data.RawUrl).AllKeys).Contains<string>));
        public virtual AssetLinks GenerateAssetLinks(IThemesProvider themesProvider)
        {
            HttpCookie cookie = null;
            cookie = cookie ?? HttpContext.Current.Request.Cookies.Get(CookieName);
            if (!Sitecore.SecurityModel.License.License.HasModule("Sitecore.SXA"))
            {
                HttpContext.Current.Response.Redirect(Settings.NoLicenseUrl + "?license=Sitecore.SXA");
                return (AssetLinks)null;
            }

            string str = string.Format("{0}#{1}#{2}#{3}#{4}", (object)this.Context.Item.ID, (object)this.Context.Device.ID, 
                (object)this.Context.Database.Name, (object)this._configuration.RequestAssetsOptimizationDisabled, (object)cookie?.Value);
            string cacheKey1;
            if (HttpContext.Current.Cache[str] != null && HttpContext.Current.Cache[cacheKey1 = this.GenerateCacheKey((int)HttpContext.Current.Cache[str])] != null)
            {
                Sitecore.Diagnostics.Log.Info("GenerateAssetLinks: returning cache value", this);
                return HttpContext.Current.Cache[cacheKey1] as AssetLinks;
            }
            AssetsArgs args = new AssetsArgs();
            CorePipeline.Run("assetService", (PipelineArgs)args);
            int hashCode = args.GetHashCode();
            string cacheKey2 = this.GenerateCacheKey(hashCode);
            if (!(HttpContext.Current.Cache[cacheKey2] is AssetLinks result) || this._configuration.RequestAssetsOptimizationDisabled)
            {
                result = new AssetLinks();
                if (!args.AssetsList.Any<AssetInclude>())
                    return result;
                args.AssetsList = (IList<AssetInclude>)args.AssetsList.OrderBy<AssetInclude, int>((Func<AssetInclude, int>)(a => a.SortOrder)).ToList<AssetInclude>();
                foreach (AssetInclude assets in (IEnumerable<AssetInclude>)args.AssetsList)
                {
                    switch (assets)
                    {
                        case ThemeInclude _:
                            this.AddThemeInclude(assets as ThemeInclude, result, themesProvider);
                            continue;
                        case UrlInclude _:
                            this.AddUrlInclude(assets as UrlInclude, result);
                            continue;
                        case PlainInclude _:
                            this.AddPlainInclude(assets as PlainInclude, result);
                            continue;
                        default:
                            continue;
                    }
                }
                this.CacheLinks(cacheKey2, result, this.DatabaseRepository.GetContentDatabase().Name.ToLowerInvariant().Equals("master", StringComparison.Ordinal) ? AssetContentRefresher.MasterCacheDependencyKeys : AssetContentRefresher.WebCacheDependencyKeys);
                this.CacheHash(str, hashCode);

            }
            return result;
        }

        protected virtual void AddThemeInclude(
          ThemeInclude themeInclude,
          AssetLinks result,
          IThemesProvider themesProvider)
        {
            Item theme = themeInclude.Theme;
            if (theme == null && !themeInclude.ThemeId.IsNull)
                theme = this.ContentRepository.GetItem(themeInclude.ThemeId);
            if (theme == null)
                return;
            Log.Debug(string.Format("Starting optimized files generation process for {0} with following configuration {1}", (object)theme.Name, (object)this._configuration));
            IList<Item> allThemes = ServiceLocator.ServiceProvider.GetService<IThemingContext>().GetAllThemes(theme);
            this.GetLinks(allThemes.FilterBaseThemes(), this._configuration.ScriptsMode, this._configuration.StylesMode, result);
            this.GetLinks(themesProvider.GetThemes(theme, (IEnumerable<Item>)allThemes), this._configuration.ScriptsMode, this._configuration.StylesMode, result);
        }

        protected virtual void AddPlainInclude(PlainInclude plainInclude, AssetLinks result)
        {
            if (plainInclude.Type == AssetType.Script)
                result.Scripts.Add(plainInclude.Content);
            else
                result.Styles.Add(plainInclude.Content);
        }

        protected virtual void AddUrlInclude(UrlInclude urlInclude, AssetLinks result)
        {
            if (urlInclude.Type == AssetType.Script)
                result.Scripts.Add("<script src=\"" + urlInclude.Url + "\"></script>");
            else
                result.Styles.Add("<link href=\"" + urlInclude.Url + "\" rel=\"stylesheet\" />");
        }

        protected virtual void GetLinks(
          IEnumerable<Item> allThemes,
          AssetServiceMode scriptsMode,
          AssetServiceMode stylesMode,
          AssetLinks result)
        {
            foreach (Item allTheme in allThemes)
            {
                AssetLinks result1 = new AssetLinks();
                if (this._configuration.RequestAssetsOptimizationDisabled)
                {
                    scriptsMode = AssetServiceMode.Disabled;
                    stylesMode = AssetServiceMode.Disabled;
                }
                else
                    this.GetAssetServiceModeFromTheme(ref scriptsMode, ref stylesMode, allTheme);
                this.GetScriptLinks(allTheme, scriptsMode, result1);
                this.GetStylesLinks(allTheme, stylesMode, result1);
                foreach (string str in result1.Styles.Select<string, string>((Func<string, string>)(link => "<link href=\"" + link + "\" rel=\"stylesheet\" />")))
                    result.Styles.Add(str);
                foreach (string str in result1.Scripts.Select<string, string>((Func<string, string>)(link => "<script src=\"" + link + "\"></script>")))
                    result.Scripts.Add(str);
            }
        }

        protected virtual void GetAssetServiceModeFromTheme(
          ref AssetServiceMode scriptsMode,
          ref AssetServiceMode stylesMode,
          Item theme)
        {
            switch (theme.Fields[Sitecore.XA.Foundation.Theming.Templates._AssetServiceThemeMode.Fields.Mode].ToEnum<AssetServiceThemeMode>())
            {
                case AssetServiceThemeMode.InheritFromSite:
                    AssetServiceMode? nullable1;
                    if (this._configuration.ScriptsSiteConfig.HasValue)
                    {
                        ref AssetServiceMode local = ref scriptsMode;
                        nullable1 = this._configuration.ScriptsSiteConfig;
                        int num = (int)nullable1.Value;
                        local = (AssetServiceMode)num;
                    }
                    else
                        scriptsMode = this._configuration.ScriptsMode;
                    nullable1 = this._configuration.StylesSiteConfig;
                    if (nullable1.HasValue)
                    {
                        ref AssetServiceMode local = ref stylesMode;
                        nullable1 = this._configuration.StylesSiteConfig;
                        int num = (int)nullable1.Value;
                        local = (AssetServiceMode)num;
                        break;
                    }
                    stylesMode = this._configuration.StylesMode;
                    break;
                case AssetServiceThemeMode.InheritFromSystem:
                    AssetServiceMode? nullable2;
                    if (this._configuration.ScriptsGlobalConfig.HasValue)
                    {
                        ref AssetServiceMode local = ref scriptsMode;
                        nullable2 = this._configuration.ScriptsGlobalConfig;
                        int num = (int)nullable2.Value;
                        local = (AssetServiceMode)num;
                    }
                    else
                        scriptsMode = this._configuration.ScriptsMode;
                    nullable2 = this._configuration.StylesGlobalConfig;
                    if (nullable2.HasValue)
                    {
                        ref AssetServiceMode local = ref stylesMode;
                        nullable2 = this._configuration.StylesGlobalConfig;
                        int num = (int)nullable2.Value;
                        local = (AssetServiceMode)num;
                        break;
                    }
                    stylesMode = this._configuration.StylesMode;
                    break;
                case AssetServiceThemeMode.Off:
                    scriptsMode = AssetServiceMode.Disabled;
                    stylesMode = AssetServiceMode.Disabled;
                    break;
                case AssetServiceThemeMode.On:
                    scriptsMode = AssetServiceMode.ConcatenateAndMinify;
                    stylesMode = AssetServiceMode.ConcatenateAndMinify;
                    break;
            }
        }

        protected virtual void GetStylesLinks(
          Item theme,
          AssetServiceMode stylesMode,
          AssetLinks result)
        {
            switch (stylesMode)
            {
                case AssetServiceMode.ConcatenateAndMinify:
                    string optimizedItemLink1 = this.GetOptimizedItemLink(theme, OptimizationType.Styles, stylesMode, "./Styles//*[(@@templateid='{0}' and @@name='{1}') or @@name='pre-optimized-min']", "optimized-min");
                    if (string.IsNullOrWhiteSpace(optimizedItemLink1))
                        break;
                    result.Styles.Add(optimizedItemLink1);
                    break;
                case AssetServiceMode.Concatenate:
                    string optimizedItemLink2 = this.GetOptimizedItemLink(theme, OptimizationType.Styles, stylesMode, "./Styles//*[(@@templateid='{0}' and @@name='{1}') or @@name='pre-optimized-min']", "optimized");
                    if (string.IsNullOrWhiteSpace(optimizedItemLink2))
                        break;
                    result.Styles.Add(optimizedItemLink2);
                    break;
                case AssetServiceMode.Disabled:
                    using (IEnumerator<string> enumerator = this.QueryAssets(theme, OptimizationType.Styles).GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            string current = enumerator.Current;
                            result.Styles.Add(current);
                        }
                        break;
                    }
            }
        }

        protected virtual void GetScriptLinks(
          Item theme,
          AssetServiceMode scriptsMode,
          AssetLinks result)
        {
            switch (scriptsMode)
            {
                case AssetServiceMode.ConcatenateAndMinify:
                    string optimizedItemLink1 = this.GetOptimizedItemLink(theme, OptimizationType.Scripts, scriptsMode, "./Scripts//*[(@@templateid='{0}' and @@name='{1}') or @@name='pre-optimized-min']", "optimized-min");
                    if (string.IsNullOrWhiteSpace(optimizedItemLink1))
                        break;
                    result.Scripts.Add(optimizedItemLink1);
                    break;
                case AssetServiceMode.Concatenate:
                    string optimizedItemLink2 = this.GetOptimizedItemLink(theme, OptimizationType.Scripts, scriptsMode, "./Scripts//*[(@@templateid='{0}' and @@name='{1}') or @@name='pre-optimized-min']", "optimized");
                    if (string.IsNullOrWhiteSpace(optimizedItemLink2))
                        break;
                    result.Scripts.Add(optimizedItemLink2);
                    break;
                case AssetServiceMode.Disabled:
                    using (IEnumerator<string> enumerator = this.QueryAssets(theme, OptimizationType.Scripts).GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            string current = enumerator.Current;
                            result.Scripts.Add(current);
                        }
                        break;
                    }
            }
        }

        protected virtual string GetOptimizedItemLink(
          Item theme,
          OptimizationType type,
          AssetServiceMode mode,
          string query,
          string fileName)
        {
            query = string.Format((IFormatProvider)CultureInfo.InvariantCulture, query, (object)Sitecore.XA.Foundation.Theming.Templates.OptimizedFile.ID, (object)fileName);
            Item optimizedScriptItem = theme.Axes.SelectSingleItem(query);
            return optimizedScriptItem != null && this.IsNotEmpty(optimizedScriptItem) ? optimizedScriptItem.BuildAssetPath(true) : this.AssetBundler.GetOptimizedItemPath(theme, type, mode);
        }

        protected virtual IEnumerable<string> QueryAssets(
          Item theme,
          OptimizationType optimizationType)
        {
            List<Item> themeChildItems = this.AssetBundler.GetThemeChildItems(theme, optimizationType);
            if (!themeChildItems.Any<Item>())
                return Enumerable.Empty<string>();
            themeChildItems.Sort((IComparer<Item>)this._comparer);
            IEnumerable<Item> source = themeChildItems.Where<Item>((Func<Item, bool>)(i => i.TemplateID != Sitecore.XA.Foundation.Theming.Templates.OptimizedFile.ID));
            Item obj = source.FirstOrDefault<Item>((Func<Item, bool>)(i => string.Equals(i.Name, "pre-optimized-min", StringComparison.OrdinalIgnoreCase)));
            if (obj == null)
                return source.Select<Item, string>((Func<Item, string>)(i => i.BuildAssetPath()));
            return (IEnumerable<string>)new List<string>()
      {
        obj.BuildAssetPath(true)
      };
        }

        protected virtual void CacheLinks(string cacheKey, AssetLinks result, string[] dependencyKeys) => HttpContext.Current.Cache.Add(cacheKey, (object)result, new CacheDependency((string[])null, dependencyKeys), DateTime.UtcNow.AddMinutes(10.0), Cache.NoSlidingExpiration, CacheItemPriority.High, (CacheItemRemovedCallback)null);

        protected virtual void CacheHash(string hashKey, int hashCode) => HttpContext.Current.Cache.Add(hashKey, (object)hashCode, new CacheDependency((string[])null, this.Context.Database.Name.ToLowerInvariant().Equals("master", StringComparison.Ordinal) ? AssetContentRefresher.MasterCacheDependencyKeys : AssetContentRefresher.WebCacheDependencyKeys), DateTime.UtcNow.AddMinutes(10.0), Cache.NoSlidingExpiration, CacheItemPriority.High, (CacheItemRemovedCallback)null);

        protected virtual string GenerateCacheKey(int hash)
        {
            string hostName = this.Context.Site.HostName;
            return string.Format("{0}#{1}#{2}#{3}#{4}", (object)this.DatabaseRepository.GetContentDatabase().Name, (object)this.Context.User.IsAuthenticated, (object)this._configuration.CacheKey, (object)hostName, (object)hash);
        }

        private void InitializeCache()
        {
            if (HttpContext.Current.Cache["XA.AssetsService.Content.Version.Master"] == null)
                HttpContext.Current.Cache.Insert("XA.AssetsService.Content.Version.Master", (object)0, (CacheDependency)null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.High, (CacheItemRemovedCallback)null);
            if (HttpContext.Current.Cache["XA.AssetsService.Content.Version.Web"] != null)
                return;
            HttpContext.Current.Cache.Insert("XA.AssetsService.Content.Version.Web", (object)0, (CacheDependency)null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.High, (CacheItemRemovedCallback)null);
        }


        private bool IsNotEmpty(Item optimizedScriptItem) => ((MediaItem)optimizedScriptItem).Size > 0L;
    }
}