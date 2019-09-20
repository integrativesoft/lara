const path = require('path');
const { CleanWebpackPlugin } = require('clean-webpack-plugin');
const webpack = require('webpack');

const banner = 'Lara Web Engine\n'
    + 'Copyright (c) 2019 Integrative Software LLC.\n'
    + 'License: Apache-2.0';

module.exports = {
    entry: './src/index.ts',
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
        extensions: ['.ts', '.js']
    },
    output: {
        filename: 'lara-client.js',
        path: path.resolve(__dirname, 'dist'),
        libraryTarget: 'var',
        library: 'LaraUI'
    }
};