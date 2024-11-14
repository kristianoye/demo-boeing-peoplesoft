const path = require('path');

module.exports = {
    mode: 'development',
    entry: {
        reactClient: './Client/react/main.tsx',
        main: './Client/main.ts'
    },
    devtool: 'source-map',
    externals: {
        jquery: "jQuery"
    },
    optimization: {
        usedExports: true
    },
    output: {
        filename: '[name].js',
        path: path.resolve(__dirname, 'wwwroot'),
    },
    module: {
        rules: [
            {
                test: /\.scss$/,
                use: ['style-loader', 'css-loader', 'sass-loader']
            },
            {
                test: /\.css$/,
                use: ['style-loader', 'css-loader']
            },
            {
                test: /\.(js|jsx|mjs)$/,
                exclude: /node_modules/,
                use: {
                    loader: 'babel-loader'
                }
            },
            {
                test: /\.(ts|tsx)$/,
                exclude: /node_modules/,
                use: {
                    loader: 'ts-loader'
                }
            }
        ]
    },
    resolve: {
        extensions: ['.ts', '.tsx', '.js', '.jsx','.css'],
    },
    target: 'web'
};
