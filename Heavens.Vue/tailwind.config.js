const colors = require('tailwindcss/colors')
module.exports = {
  purge: ['./index.html', './src/**/*.{vue,js,ts,jsx,tsx}'],
  theme: {
    extend: {},
    boxShadow: {
      sm: '0 1px 2px 0 rgba(0, 0, 0, 0.05)',
      DEFAULT: '0 1px 3px 0 rgba(0, 0, 0, 0.1), 0 1px 2px 0 rgba(0, 0, 0, 0.06)',
      md: '0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06)',
      lg: '0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05)',
      xl: '0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)',
      '2xl': '0 25px 50px -12px rgba(0, 0, 0, 0.25)',
      '3xl': '0 35px 60px -15px rgba(0, 0, 0, 0.3)',
      inner: 'inset 0 2px 4px 0 rgba(0, 0, 0, 0.06)',
      outline: '0 0 0 3px rgba(66, 153, 225, 0.5)',
      focus: '0 0 0 3px rgba(66, 153, 225, 0.5)',
      none: 'none',
      max: '0 0 10px 8px rgba(8, 8, 8, 0.3)',
    },
    colors: {
      ...colors,
      // 在这里添加完自定义颜色，最好在quasar.conf里也添加，否则可能会出现重名问题
      primary: '#1890FF',
      'light-primary': '#40A9FF',
      danger: '#FF4D4F',
      'light-danger': '#FF7875',
      'light-success': '#6EE7B7',
      success: '#4CAF50',
    },
  },
  corePlugins: {
    preflight: false,
  },
  variants: {
    extend: {
      backgroundColor: ['active'],
      transitionDuration: ['hover', 'focus', 'active'],
    },
  },
  plugins: [],
}
