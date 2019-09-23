const path = require('path');
const { CleanWebpackPlugin } = require('clean-webpack-plugin');
const webpack = require('webpack');
const fs = require('fs');

const banner = 'Lara Web Engine\n'
    + 'Copyright (c) 2019 Integrative Software LLC.\n'
    + 'License: Apache-2.0';

var data = fs.readFileSync('../environment.txt', 'utf8');
var configuration = data.toString().trim();
var buildMode = "production";
if (configuration === "Debug") {
    buildMode = "development";
}
console.log("configuration: " + configuration);
console.log("mode: " + buildMode);

module.exports = {
    entry: './src/index.ts',
    mode: buildMode,
    plugins: [
        new CleanWebpackPlugin(),
        new webpack.BannerPlugin(banner),
        new webpack.ProvidePlugin({
            $: "jquery",
            jQuery: "jquery"
        })
    ],
    module: {
        rules: [
            {
                use: 'ts-loader',
                exclude: /node_modules/
            }
        ]
    },
    resolve: {
        extensions: ['.ts', '.js'],
        alias: {
            jquery: "jquery/src/jquery"
        }
    },
    output: {
        filename: 'lara-client.js',
        path: path.resolve(__dirname, 'dist'),
        libraryTarget: 'var',
        library: 'LaraUI'
    }
};