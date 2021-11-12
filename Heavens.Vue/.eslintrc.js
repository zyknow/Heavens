// const { resolve } = require('path');
// module.exports = {
//   // https://eslint.org/docs/user-guide/configuring#configuration-cascading-and-hierarchy
//   // This option interrupts the configuration hierarchy at this file
//   // Remove this if you have an higher level ESLint config file (it usually happens into a monorepos)
//   root: true,

//   // https://eslint.vuejs.org/user-guide/#how-to-use-custom-parser
//   // Must use parserOptions instead of "parser" to allow vue-eslint-parser to keep working
//   // `parser: 'vue-eslint-parser'` is already included with any 'plugin:vue/**' config and should be omitted
//   parserOptions: {
//     // https://github.com/typescript-eslint/typescript-eslint/tree/master/packages/parser#configuration
//     // https://github.com/TypeStrong/fork-ts-checker-webpack-plugin#eslint
//     // Needed to make the parser take into account 'vue' files
//     extraFileExtensions: ['.vue'],
//     parser: '@typescript-eslint/parser',
//     project: resolve(__dirname, './tsconfig.json'),
//     tsconfigRootDir: __dirname,
//     ecmaVersion: 2018, // Allows for the parsing of modern ECMAScript features
//     sourceType: 'module' // Allows for the use of imports
//   },

//   env: {
//     browser: true
//   },

//   // Rules order is important, please avoid shuffling them
//   extends: [
//     // Base ESLint recommended rules
//     // 'eslint:recommended',

//     // https://github.com/typescript-eslint/typescript-eslint/tree/master/packages/eslint-plugin#usage
//     // ESLint typescript rules
//     'plugin:@typescript-eslint/recommended',
//     // consider disabling this class of rules if linting takes too long
//     'plugin:@typescript-eslint/recommended-requiring-type-checking',

//     // Uncomment any of the lines below to choose desired strictness,
//     // but leave only one uncommented!
//     // See https://eslint.vuejs.org/rules/#available-rules
//     'plugin:vue/vue3-essential', // Priority A: Essential (Error Prevention)
//     // 'plugin:vue/vue3-strongly-recommended', // Priority B: Strongly Recommended (Improving Readability)
//     // 'plugin:vue/vue3-recommended', // Priority C: Recommended (Minimizing Arbitrary Choices and Cognitive Overhead)

//     // https://github.com/prettier/eslint-config-prettier#installation
//     // usage with Prettier, provided by 'eslint-config-prettier'.
//     'prettier'
//   ],

//   plugins: [
//     // required to apply rules which need type information
//     '@typescript-eslint',

//     // https://eslint.vuejs.org/user-guide/#why-doesn-t-it-work-on-vue-file
//     // required to lint *.vue files
//     'vue',

//     // https://github.com/typescript-eslint/typescript-eslint/issues/389#issuecomment-509292674
//     // Prettier has not been included as plugin to avoid performance impact
//     // add it as an extension for your IDE
//   ],

//   globals: {
//     ga: 'readonly', // Google Analytics
//     cordova: 'readonly',
//     __statics: 'readonly',
//     __QUASAR_SSR__: 'readonly',
//     __QUASAR_SSR_SERVER__: 'readonly',
//     __QUASAR_SSR_CLIENT__: 'readonly',
//     __QUASAR_SSR_PWA__: 'readonly',
//     process: 'readonly',
//     Capacitor: 'readonly',
//     chrome: 'readonly'
//   },

//   // add your custom rules here
//   rules: {
//     // 禁止未使用过的变量 default: ['error', { vars: 'local' }]
//     "no-unused-vars": "off",

//     // enUS: all rules docs https://eslint.org/docs/rules/
//     // zhCN: 所有规则文档 https://eslint.bootcss.com/docs/rules/
//     // 基础规则 全部 ES 项目通用
//     'no-console': process.env.NODE_ENV === 'production' ? 'warn' : 'off',
//     'no-debugger': process.env.NODE_ENV === 'production' ? 'warn' : 'off',
//     'quote-props': 'off',
//     // 结尾必须有逗号(主要缓解增加一行对象属性，导致 git 变更记录是两行的情况)
//     'comma-dangle': ['error', 'always-multiline'],
//     // 逗号必须在一行的结尾
//     'comma-style': ['error', 'last'],
//     // 禁止混合使用不同的操作符 'error','off'
//     'no-mixed-operators': 'off',
//     // 禁止未使用过的变量 default: ['error', { vars: 'local' }]
//     "no-unused-vars": "off",
//     // 强制在代码块中开括号前和闭括号后有空格
//     'block-spacing': ['error', 'always'],
//     'object-curly-spacing': ['error', 'always'],
//     // 要求使用分号代替 ASI (semi)
//     "semi": ["error", "never"],

//     quotes: [
//       2,
//       'single',
//       {
//         avoidEscape: true,
//         allowTemplateLiterals: true,
//       },
//     ],

//     /* vue 项目专用 */
//     'vue/require-default-prop': 'off',
//     'vue/singleline-html-element-content-newline': ['off'],
//     // 模板中组件名称使用 kebab-case 模式
//     'vue/component-name-in-template-casing': [
//       'error',
//       'kebab-case',
//       {
//         registeredComponentsOnly: true,
//         ignores: [],
//       },
//     ],
//     'vue/custom-event-name-casing': 'off',
//     /* typescript */
//     '@typescript-eslint/ban-ts-ignore': 'off',
//     '@typescript-eslint/no-var-requires': 'off',
//     '@typescript-eslint/no-explicit-any': 'off',
//     // disable `function-return` the rule for all files
//     '@typescript-eslint/explicit-function-return-type': 'off',
//     '@typescript-eslint/no-empty-function': 'off',
//     '@typescript-eslint/no-non-null-assertion': 'off',
//     "@typescript-eslint/no-unused-vars": ["off"],
//     // bug fix
//     'template-curly-spacing': 'off',
//     'vue/experimental-script-setup-vars': 'off',
//     '@typescript-eslint/explicit-module-boundary-types': 'off',
//     '@typescript-eslint/no-unsafe-member-access':"off",
//     "@typescript-eslint/no-unsafe-return":'off',
//     "@typescript-eslint/restrict-template-expressions":"off"
//   }
// }

module.exports = {
  root: true,
  env: {
    node: true,
  },
  extends: [
    'plugin:vue/vue3-essential',
    'eslint:recommended',
    '@vue/typescript/recommended',
    '@vue/prettier',
    '@vue/prettier/@typescript-eslint',
  ],
  parserOptions: {
    ecmaVersion: 2020,
  },
  rules: {
    // enUS: all rules docs https://eslint.org/docs/rules/
    // zhCN: 所有规则文档 https://eslint.bootcss.com/docs/rules/
    // 基础规则 全部 ES 项目通用
    'no-console': process.env.NODE_ENV === 'production' ? 'warn' : 'off',
    'no-debugger': process.env.NODE_ENV === 'production' ? 'warn' : 'off',
    'quote-props': 'off',
    // 结尾必须有逗号(主要缓解增加一行对象属性，导致 git 变更记录是两行的情况)
    'comma-dangle': ['error', 'always-multiline'],
    // 逗号必须在一行的结尾
    'comma-style': ['error', 'last'],
    // 禁止混合使用不同的操作符 'error','off'
    'no-mixed-operators': 'off',
    // 禁止未使用过的变量 default: ['error', { vars: 'local' }]
    "no-unused-vars": "off",
    // 强制在代码块中开括号前和闭括号后有空格
    'block-spacing': ['error', 'always'],
    'object-curly-spacing': ['error', 'always'],
    // 要求使用分号代替 ASI (semi)
    "semi": ["error", "never"],
    "no-empty":'off',
    quotes: [
      2,
      'single',
      {
        avoidEscape: true,
        allowTemplateLiterals: true,
      },
    ],

    /* vue 项目专用 */
    'vue/require-default-prop': 'off',
    'vue/singleline-html-element-content-newline': ['off'],
    // 模板中组件名称使用 kebab-case 模式
    'vue/component-name-in-template-casing': [
      'error',
      'kebab-case',
      {
        registeredComponentsOnly: true,
        ignores: [],
      },
    ],
    'vue/custom-event-name-casing': 'off',
    /* typescript */
    '@typescript-eslint/ban-ts-ignore': 'off',
    '@typescript-eslint/no-var-requires': 'off',
    '@typescript-eslint/no-explicit-any': 'off',
    // disable `function-return` the rule for all files
    '@typescript-eslint/explicit-function-return-type': 'off',
    '@typescript-eslint/no-empty-function': 'off',
    '@typescript-eslint/no-non-null-assertion': 'off',
    "@typescript-eslint/no-unused-vars": ["off"],
    // bug fix
    'template-curly-spacing': 'off',
    'vue/experimental-script-setup-vars': 'off',
    '@typescript-eslint/explicit-module-boundary-types': 'off',
  },
};
