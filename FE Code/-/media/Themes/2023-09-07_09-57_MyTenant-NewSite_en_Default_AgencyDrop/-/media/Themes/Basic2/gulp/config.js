const fs = require('fs')
const path = require('path');
const MEDIA_PATH = '-\\media';
const SCRIBAN_PATH = '-/scriban';

module.exports = {
    // Change to TRUE if you want add source map for sass files
    sassSourceMap: false,
    // Please configure
    serverOptions: {
        server: 'https://sxa', //need to be changed
        removeScriptPath: '/-/script/v2/master/RemoveMedia',
        uploadScriptPath: '/sitecore modules/PowerShell/Services/RemoteScriptCall.ashx',
        updateTemplatePath: '/-/script/v2/master/ChangeTemplate',
        updateScribanPath: '/-/script/v2/master/ChangeScriban',
        mediLibraryPath: '/-/script/media/master'
    },
    // Where cli will take url from
    // local - from theme gulp config
    // global - from url registered globally by cli
    serverUrlStrategy: 'local',
    autoprefixer: {
        "overrideBrowserslist": [
            ">0.2%",
            "not dead",
            "not op_mini all"
        ],
        cascade: false
    },
    //Rules for excluding files from uploading
    excludedPath: [
        'styles/has.css', // can be a string
        ///[\\\/][\w-]*.css$/g   //exclude all css files
        /[\\\/]test.css$/g, //exclude test.css files
        ///[\\\/][\w-]*.css.map$/g //exclude css.map files
        ///styles[\\\/][\w-]*.css$/g //exclude all css files from style folder
    ],
    //Server check all items names with this rule. It is not recommended to change
    serverNameValidation: [
        /^[\w\*\$][\w\s\-\$]*(\(\d{1,}\)){0,1}$/
    ],
    map: {
        jsPath: ['scripts/**/*js.map'],
        cssPath: ['styles/**/*css.map'],
    },
    minifyOptions: {
        js: {
            compress: {
                hoist_funs: true,
                passes: 1
            },
            toplevel: false
        },
        css: { compatibility: 'ie8' }
    },

    html: {
        path: (function () {
          const paths = [];
          const _path = getPosixRelativePath();

          if(_path) {
            paths.push(`${_path}/**/*.html`.replace('//', '/'));
          }
          paths.push(`!${_path}/${MEDIA_PATH.split(path.sep).join(path.posix.sep)}/**/*.html`);
          return paths;
        })()
    },
    scriban: {
        path: (function () {
          const _path = getPosixRelativePath('scriban');

          if(_path) {
            return `${_path}${SCRIBAN_PATH}/**/*.scriban`;
          }
        })(),
        metadataFilePath: (function () {
          const _path = getPosixRelativePath('scriban');

          if(_path) {
            return `${_path}${SCRIBAN_PATH}/metadata.json`;
          }
        })()
    },
    img: {
        path: 'images/**/*.*'
    },
    js: {
        path: 'scripts/**/*.js',
        esLintUploadOnError: true,
        minificationPath: ['scripts/**/*.js'],
        jsOptimiserFilePath: 'scripts/**/',
        jsOptimiserFileName: 'pre-optimized-min.js',
        es6Support: true, 
        jsSourceMap: false,
        enableMinification: true,
        disableSourceUploading: true
    },
    es: {
        path: 'sources/**/*.js',
        targetPath: 'scripts/',
        disableSourceUploading: true
    },
    css: {
        path: 'styles/**/*.css',
        targetPath: '',
        minificationPath: ['styles/*.css'],
        cssOptimiserFilePath: 'styles/',
        cssOptimiserFileName: 'pre-optimized-min.css',
        cssSourceMap: false,
        enableMinification: true,
        disableSourceUploading: true
    },
    sass: {
        root: 'sass/**/*.scss',
        components: {
            sassPath: 'sass/*.scss',
            stylePath: 'styles'
        },
        styles: {
            sassPath: [
                'sass/styles/common/*.scss',
                'sass/styles/add-highlight/*.scss',
                'sass/styles/background-colors/*.scss',
                'sass/styles/content-alignment/*.scss',
                'sass/styles/layout/*.scss',
                'sass/styles/spacing/*.scss'
            ],
            stylePath: 'styles',
            concatName: 'styles.css'
        },
        dependency: {
            sassPath: ['sass/styles/**/*.scss'],
            exclusion: ['!sass/styles/common/*.scss',
                '!sass/styles/content-alignment/*.scss',
                '!sass/styles/layout/*.scss',
                '!sass/styles/add-highlight/*.scss',
                '!sass/styles/background-colors/*.scss',
                '!sass/styles/spacing/*.scss'
            ],
        },
        core: {
            sassPath: ['sass/abstracts/**/*.scss',
                'sass/base/**/*.scss',
                'sass/components/**/*.scss'
            ],
            stylePath: 'styles'
        },
        disableSourceUploading: true
    },
    fonts: {
        path: [
            'fonts/**/*.eot',
            'fonts/**/*.otf',
            'fonts/**/*.svg',
            'fonts/**/*.ttf',
            'fonts/**/*.woff',
            'fonts/**/*.woff2',
        ],
    },
    gulpConfig: {
        path: ['gulp/*.js', 'gulp/*.json', 'gulpfile.js']
    },
    sprites: {
        flags: {
            spritesmith: {
                imgName: 'sprite-flag.png',
                cssName: '_sprite-flag.scss',
                imgPath: '../images/sprite-flag',
                cssFormat: 'scss',
                padding: 10,
                algorithm: 'top-down',
                cssOpts: {
                    cssSelector: function (sprite) {
                        return '.flags-' + sprite.name;
                    }
                },
                cssVarMap: function (sprite) {
                    sprite.name = 'flags-' + sprite.name;
                }

            },
            flagsFolder: 'images/flags/*.png',
            imgDest: './images',
            cssDest: './sass/base/sprites'
        }

    },

    stylesConfig: {
        'accordion': 'component-accordion.scss',
        'breadcrumb': 'component-breadcrumb.scss',
        'carousel': 'component-carousel.scss',
        'container': 'component-container.scss',
        'divider': 'component-divider.scss',
        'feed': 'component-feed.scss',
        'file-list': 'component-file-list.scss',
        'flip': 'component-flip.scss',
        'forms': 'component-forms.scss',
        'galleria': 'component-galleria.scss',
        'image': 'component-image.scss',
        'image-alignment': 'component-image.scss',
        'language-selector': 'component-language-selector.scss',
        'link-list': 'component-link-list.scss',
        'media-link': 'component-media-link.scss',
        'navigation': 'component-navigation.scss',
        'play-list': 'component-playlist.scss',
        'promo': 'component-promo.scss',
        'rich-text': 'component-richtext-content.scss',
        'search': 'component-search-other.scss',
        'tabs': 'component-tabs.scss',
        'tabs-overflow': 'component-tabs.scss',
        'archive': 'component-archive.scss',
        'field-editor': 'component-field-editor.scss',
        'map': 'component-map.scss',
        'page-content': 'component-richtext-content.scss',
        'page-list': 'component-page-list.scss',
        'tag-cloud': 'component-tag-cloud.scss',
        'tag-list': 'component-tag-list.scss',
        'title': 'component-title.scss',

    },

    loginQuestions: [{
        type: 'login',
        name: 'login',
        message: 'Enter your login',
        default: 'sitecore\\admin'
    },
    {
        type: 'password',
        name: 'password',
        message: 'Enter your password',
        default: 'b'
    }
    ],


    user: { login: '', password: '' },

    init: function () {
        if (!isCeltInstalled()) return this;

        // gulpfile.js didn't run, so postinstall script is running
        if (!global.rootPath) {
            postInit();

            return this;
        }

        const { configUtils } = require('@sxa/celt');

        let confUtil = configUtils ? configUtils : global.configUtils;
        this.serverOptions = {
          ...this.serverOptions,
          ...confUtil.getConf().serverOptions,
        };

        return this;
    }

}.init();

function isCeltInstalled() {
    return fs.existsSync(path.resolve(__dirname, '../node_modules/@sxa/celt'));
}

function postInit() {
    global.rootPath = path.resolve(__dirname, '../');

    require('@sxa/celt/util/postInit');
}

function getPosixRelativePath(type) {
  if (!global.rootPath) return;

  const rootCreativeExchangePath = global.rootPath.split(MEDIA_PATH);
  let _path = './';
  
  if (rootCreativeExchangePath.length > 1) {
      _path = ( type !== 'scriban' ) ? path.relative(_path, global.rootPath.split(MEDIA_PATH)[0])
       : path.relative(_path, global.rootPath);

      _path = _path.split(path.sep).join(path.posix.sep);
  }

  _path = path.posix.normalize(_path) !== '.' ? path.posix.normalize(_path) : './';

  return _path;
}
