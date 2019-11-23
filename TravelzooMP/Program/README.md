# 注意事项

* 生产环境和开发环境共用一套代码
* 不同的环境对应不同的文件，取其中的字段替换原文件中的字段，对应文件如下：  
| 原文件              | dev环境                 | prod环境                 |
| ------------------- | ----------------------- | ------------------------ |
| app.js              | app.dev.js              | app.prod.js              |
| app.json            | app.dev.json            | app.prod.json            |
| project.config.json | project.config.dev.json | project.config.prod.json |  |
 
#### 替换字段 
| 文件                | 字段                                 | dev对应值                     | prod对应值                 |
| ------------------- | ------------------------------------ | ----------------------------- | -------------------------- |
| app.js              | servsers                             | https://devtravelzoo.m-int.cn | https://travelzoo.m-int.cn |
| app.json            | debug                                | true                          | false                      |
| app.json            | pages新增页面和tabBar.list的入口配置 |                               |                            |
| project.config.json | appid                                | wx7949b23c7b450278            | wx8332c5c050be0e3d         |
| project.config.json | projectname                          | travelzoo-test                | travelzoo%2B               |


### PS:
> 新加统计代码也要更换配置，文件里有详细说明，文件 utils/ald-stat-conf.js