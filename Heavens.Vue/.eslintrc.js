module.exports = {
  root: true,
  env: {
    browser: true,
    node: true,
    es6: true
  },
  // https://github.com/vuejs/vue-eslint-parser#parseroptionsparser
  parser: 'vue-eslint-parser',
  parserOptions: {
    parser: '@typescript-eslint/parser'
    // ecmaVersion: 2020,
    // sourceType: 'module',
    // jsxPragma: 'React',
    // ecmaFeatures: {
    //   tsx: true
    // }
  },
  plugins: ['@typescript-eslint', 'prettier'],
  extends: [
    'plugin:@typescript-eslint/recommended',
    'plugin:vue/vue3-recommended',
    'prettier'
    // 'plugin:prettier/recommended'
  ],
  rules: {
    'prettier/prettier': 'warn',

    'space-before-function-paren': 0,
    'vue/name-property-casing': ['error', 'PascalCase'], // vue/component-definition-name-casing 对组件定义名称强制使用特定的大小
    'vue/attributes-order': 'warn',
    'vue/one-component-per-file': 0,
    'vue/max-attributes-per-line': 0,
    'vue/multiline-html-element-content-newline': 0,
    'vue/singleline-html-element-content-newline': 0,
    'vue/attribute-hyphenation': 0,
    'vue/require-default-prop': 0,
    'vue/html-closing-bracket-spacing': 0,
    'vue/html-closing-bracket-newline': 0,
    'vue/html-self-closing': [
      'error',
      {
        html: {
          void: 'always',
          normal: 'never',
          component: 'always'
        },
        svg: 'always',
        math: 'always'
      }
    ],
    //关闭未使用的变量字段警告
    '@typescript-eslint/no-unused-vars': [0],

    // 关闭eslint !强制断言警告
    '@typescript-eslint/no-non-null-assertion': 0,

    // 'vue/html-self-closing':0,
    // 'vue/script-setup-uses-vars': 0,

    //关闭 prop默认值检测
    'vue/require-default-prop': 0,

    // 关闭多单词报错
    'vue/multi-word-component-names': 0,

    // 关闭{}检测
    '@typescript-eslint/no-empty-function': 0,

    // 关闭any检测
    '@typescript-eslint/no-explicit-any': 0,
    '@typescript-eslint/explicit-module-boundary-types': 0
  }
}
