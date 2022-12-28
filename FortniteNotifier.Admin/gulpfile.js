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
        ]
    },
    dest: {
        js: [
            'wwwroot/dist/js'
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

exports.default = series(jsBundleAndCopy);