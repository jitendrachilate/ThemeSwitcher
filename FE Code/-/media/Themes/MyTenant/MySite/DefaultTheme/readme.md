##
Boilerplate for creating a new theme for your Sitecore site. 

## <p style="color:red">For this version of the theme we recommend to use [@sxa/cli](https://sitecore.myget.org/feed/sc-npm-packages/package/npm/@sxa/CLI) rather than gulp tasks</p>

## Prerequisites

1. Should be installed [@sxa/cli](https://sitecore.myget.org/feed/sc-npm-packages/package/npm/@sxa/CLI) globally.  


## For using Autosynchronizer, you need to complete next steps:

1. Download the theme boilerplate;
2. Open *PathToInstance/Website/App_Config/Include/z.Feature.Overrides* (in previous version of Sitecore it can be *PathToInstance/Website/App_Config/Include/Feature*) folder and remove **.disabled** from **z.SPE.Sync.Enabler.Gulp.config.disabled** file;
3. Switch to the downloaded theme boilerplate folder.
4. run `sxa init`
5. Update the config file for Gulp tasks. **ThemeRoot/gulp/config.js** file:
    1. `serverOptions.server` - path to sitecore instance. Example `server: 'http://sxa'`;
6. If you use Creative exchange skip this step. Open **ThemeRoot/gulp/serverConfig.json** 
     1. `serverOptions.projectPath` - path to project, where the theme is placed. Example ` projectPath: '/themes'`;
    2. `serverOptions.themePath` - path to basic theme folder from the project root. Example ` themePath: '/Basic2'`;
7. Open the Theme root folder with the command line.
8. Run `npm install` (*node.js and npm should be installed already *);
9. If gulp is not yet installed - Install gulp using following command: `npm install --global gulp-cli` 
10. Run the gulp task that you need: <br/>
    Global tasks:
    1. `gulp default` or just `gulp` - starts `gulp watchAll`;
    2. `gulp watchAll` - aggregates the functionality of the following tasks:<br/>
            `watchSass`<br/>
            `watchJS`<br/>
            `watchEs`<br/>
            `watchCss`<br/>
            `watchImg`<br/>
            `watchSassSource`<br/>
            `watchScriban`<br/>
            `watchHtml`<br/>
    3. `gulp buildAll` - compiles sass, minifies css and js;
    4. `gulp uploadAll` - uploading JavaScript, CSS, and images to the Sitecore instance;
    5. `gulp rebuildAll` - compiles sass components into css, minifies js and css, uploads js, css, images;
    6. `gulp rebuildMain` - compiles sass components into css, minifies js and css, uploads js, css;

    For SASS
    1. `gulp watchSass` - aggregates the functionality of the following tasks:<br/>
        `watchSassComponents`
        `watchSassBase`
        `watchSassStyles`
        `watchSassDependency`
    1. `gulp buildSass` - compiles sass into css from root of *sass* folder;
    2. `gulp buildSassStyles` - compiles files from **sass/styles/common** , **sass/styles/content-alignment** , **sass/styles/layout**  to **styles/styles.css**;
    3. `gulp watchSassStyles` - monitors changes under **sass/styles/common** , **sass/styles/content-alignment** , **sass/styles/layout** folders, compiles all of them to **styles/styles.css**;
    4. `gulp watchSassBase` - monitors changes under  **sass/abstracts/**, **sass/base/** , **sass/components** folders and compiles components and styles;
    5. `gulp watchSassComponents` - monitors changes in component styles under *sass* folder and compiles them to **styles** folder;
    6. `gulp watchSassDependency` - monitors changes under **sass/styles/** (except for **sass/styles/common** , **sass/styles/content-alignment** , **sass/styles/layout**) and compiles appropriate components;
    7. `gulp watchSassSource` - monitors changed under **sass/\*** folder and uploads changed file to the Sitecore instance;

    For CSS
    1. `gulp watchCss` - monitors changes of css files under **styles** folder and uploads them to the Sitecore instance;
    2. `gulp buildCss` - bundles and minifies files in **styles** folder;
    3. `gulp uploadCss` - uploading CSS files from **styles** folder based on the theme configuration.

    For JavaScript:
    1. `gulp buildEslint` run eslint for all JavaScript in the **Scripts** folder;
    2. `gulp watchJs` - watches changes of js files under **Scripts** folder and upload them to the Sitecore instance;
    2. `gulp watchEs` - watches changes of ES6+ js files under **sources** folder and upload them to the Sitecore instance;
    3. `gulp buildJs` - bundles and minifies JavaScript files in the **Scripts** folder;
    4. `gulp uploadJs` - uploads JavaScript files from **Scripts** folder based on the theme configuration

    For SASS and CSS
    1. `gulp buildStyles` - compiles sass components into css, bundles and minfies css files;

    For Images
    1. `gulp watchImg` - monitors changes under **images** folder and uploads the files to the Sitecore instance;
    2. `gulp uploadImg` - upload files from **images** to the Sitecore instance;

    For Sprite
    1. `gulp buildSpriteFlag` - create sprites for icons from flags folder;

    For Gulp config
    1. `gulp uploadGulpConfig` - upload files from **config** folder and **gulpfile.js** to the Sitecore instance;

    #### Creative Exchange specific tasks<br/>
    For Scriban 
    1. `gulp watchScriban` - monitors changes of Scriban files and uploads them to the Sitecore instance;

    For HTML 
    1. `gulp watchHtml` - monitors changes of HTML files and uploads them to the Sitecore instance to be parsed by Creative Exchange;
    
11. When watcher starts you must enter your login and password for Sitecore, for uploading reason.

