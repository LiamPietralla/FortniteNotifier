/// <binding BeforeBuild='default' />
const { src, dest, series } = require("gulp");

// Build Process
/*
 *  1. Clean up the dist folder
 *  2. Bundle and copy the js files
 */

// Configs and paths
const config = {
    src: {
        js: [
            'node_modules/jquery/dist/jquery.min.js',
            'node_modules/jquery-validation/dist/jquery.validate.min.js',
            'node_modules/jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.min.js',
            'node_modules/bootstrap/dist/js/bootstrap.bundle.min.js',
            'wwwroot/js/site.js'
        ],
        css: [
            'node_modules/bootstrap/dist/css/bootstrap.min.css',
            'node_modules/bootstrap-icons/font/bootstrap-icons.css',
            'wwwroot/css/site.css'
        ]
    },
    dest: {
        js: [
            'wwwroot/dist/js'
        ],
        css: [
            'wwwroot/dist/css'
        ]
    }
};

/****************************/
/* JS Bundling and Copying */
/**************************/

function jsBundleAndCopy() {
    return src(config.src.js)
        .pipe(dest(config.dest.js));
}

/****************************/
/* CSS Bundling and Copying */
/**************************/

function cssBundleAndCopy() {
    return src(config.src.css)
        .pipe(dest(config.dest.css));
}

exports.default = series(jsBundleAndCopy, cssBundleAndCopy);