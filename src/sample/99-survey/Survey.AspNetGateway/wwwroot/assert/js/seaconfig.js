seajs.config({
  alias: {
    'jquery': 'lib/jquery/1.7.2/jquery.min',
    'easing': 'lib/jquery.easing.1.3',
    'tabpanel': 'plugin/jquery.tabpanel',
    'flexigrid': 'plugin/jquery.flexigrid',
    'dailog': 'plugin/jquery.ifrmdailog',  
    'tree': 'plugin_complie/jquery.tree'
  },
  preload: [
   'jquery'  
  ],
  debug: true,
  map: [
    [/^(.*\/assert\/js\/page\/.*\.(?:css|js))(?:.*)$/i, '$1?_=2013092401'],   
    [/^(.*\/assert\/js\/plugin\/.*\.(?:css|js))(?:.*)$/i, '$1?_=2013092401']
  ],
  //base: 'http://example.com/path/to/libs/',
  charset: 'utf-8',
  timeout: 20000
});
