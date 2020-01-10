const path = require('path');
const { CleanWebpackPlugin } = require('clean-webpack-plugin');
const webpack = require('webpack');

const banner = 'Lara Web Engine\n'
    + 'Copyright (c) 2019 Integrative Software LLC.\n'
    + 'License: Apache-2.0';

module.exports = env => {

    var buildMode, devtool;
    if (env.release) {
        buildMode = "production";
        devtool = "nosources-source-map";
    } else {
        buildMode = "development";
        devtool = "source-map";
    }
    console.log("mode: " + buildMode);

    return {
        entry: './src/index.ts',
        mode: buildMode,
        devtool: devtool,
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
                },
                {
                    test: /\.css$/,
                    loaders: ["style-loader", "css-loader"]
                },
                {
                    test: /\.(jpe?g|png|gif)$/i,
                    loader: "file-loader",
                    options: {
                        name: '[name].[ext]',
                        outputPath: 'assets/images/'
                    }
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
    }
};
