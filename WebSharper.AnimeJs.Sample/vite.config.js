const { resolve } = require('path')

module.exports = {
  build: {
    rollupOptions: {
      input: {
        main: resolve(__dirname, 'index.html'),
            svgboxopen: resolve(__dirname, 'svgboxopen.html'),
            svgmenu: resolve(__dirname, 'svgmenu.html')
      }
    }
  }
}