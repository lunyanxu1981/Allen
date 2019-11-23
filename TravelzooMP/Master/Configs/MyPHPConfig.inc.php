<?php
/**
 +------------------------------------------------------------------------------
 * MyPHP框架的核心配置文件
 +------------------------------------------------------------------------------
 * 该文件名和文件所有在目录名 Configs 都不能改名
 +------------------------------------------------------------------------------ 
 */

// 框架调试开关(Het和SQL),开发时推荐打开
$my_debug = (strpos ( $_SERVER ['HTTP_USER_AGENT'], 'Chrome' )) ? true : false;
// define('MY_DEBUG', $my_debug);
define ( 'MY_DEBUG', false );

/**
 * +------------------------------------------------------------------------------
 * 数据库设置
 * +------------------------------------------------------------------------------
 * 暂时只支持 Mysql 和 Oracle 数据库
 * 支持同时连接多个数据库(包括同类型的多个数据库)
 * +------------------------------------------------------------------------------
 */
define ( 'MY_DB_CACHE_ENABLE', false ); // 查询缓存是否开启
define ( 'MY_DB_CACHE_TYPE', 'file' ); // 查询缓存类型,推荐使用 memcache 详细请参考
                                         // "缓存Cache组件常量配置"
define ( 'MY_DB_DEFAULT_TYPE', 'MYSQL' ); // 数据库类型 'MYSQL' 或 'ORACLE'
define ( 'MY_DB_DEFAULT_HOST', 'rds3n929rdo4508mjlw9.mysql.rds.aliyuncs.com' ); // 主机或IP
define ( 'MY_DB_DEFAULT_USER', 'app_rw' ); // 用户名
define ( 'MY_DB_DEFAULT_PASSWORD', 'va9muVw3dEchUHrb' ); // 密码
define ( 'MY_DB_DEFAULT_DATABASE', 'travelzootest' ); // 数据库名
define ( 'MDP', '' ); // 数据库的默认前缀 MY_DB_DEFAULT_PREFIX
define ( 'MP', '' ); // 数据库表的默认前缀 MY_DB_DEFAULT_PREFIX
define ( 'MY_DB_DEFAULT_PORT', '' ); // 数据库的连接端口,(空)表示采用默认值端口
define ( 'MY_DB_DEFAULT_CHARSET', 'utf8' ); // 数据库的编码(推荐UTF8)
define ( 'MY_DB_DEFAULT_PCONNECT', false ); // 是否采用常连接(在有事务处理时不推荐使用)
define ( 'MY_DB_DEFAULT_DRIVER', 'mysql' );

// MY_DB_DEFAULT_DRIVER (驱动程序接口)
// 'mysql':原始PHP-MySql驱动,即'mysql_connect'系列函数.(仅数据库类型为'MYSQL'时有效)
// 'mysqli':PHP-MySqli驱动,即'mysqli_connect'系列函数.(仅数据库类型为'MYSQL'时有效)
// 'pdo':PHP全新的PDO驱动,支持更好高级数据库操作,如储存过程,事务等(推荐使用),如果数据库类型为 'ORACLE' 时则一定要选择 'pdo'
// 驱动程序

// [多数据库设置]
// 以下 'mysql2'和 'oracle'可以看做是数据库的"连接标识".你可以更改为自己喜欢的名称.
// 使用方法: Factory::loadDB('mysql2') 返回的是 Model() 组件实例对象

define ( 'MY_MORE_DBS', serialize ( array (

'master' => array ('type' => 'MYSQL', 'host' => 'rds3n929rdo4508mjlw9.mysql.rds.aliyuncs.com', 'user' => 'app_rw', 'password' => 'va9muVw3dEchUHrb', 'database' => 'travelzootest', 'port' => '', 'charset' => 'utf8', 'pconnect' => false, 'driver' => 'mysql' ) ) ) );

/**
 * +------------------------------------------------------------------------------
 * MEMCACHE 或 MEMCACHEDB 相关设置,你要保证你的PHP上安装了MEMCACHE扩展的
 * +------------------------------------------------------------------------------
 * 注意:如果你的网站有其它二级域名子站的情况时,请设置这个值,否则可以为空.
 * [注意]
 * 1:如果在以 SESSION 或 CACHE 的设置中要用到 MEMCACHE 时必需设置以下值
 * 2:由于 MEMCACHE 与 MEMCACHEDB 两者所使用的协议相同,所以以下的 HOST 设置是兼容两者的
 * 3:以下 HOST 设置时不能混合两种服务器,即 host1 设置为 MEMCACHE的地址,host2 却设置为 MEMCACHEDB的址
 * +------------------------------------------------------------------------------
 */
define ( 'MY_MEMCACHE_PCONNECT', false ); // 是否打开常连接
define ( 'MY_MEMCACHE_HOSTS', serialize ( array ('host1' => array ('host' => 'localhost', // 主机IP
'port' => '11211' )// 服务打开的端口号
 ) ) );

/**
 * +------------------------------------------------------------------------------
 * 网站的根域名,为空则使用当前域名全称
 * +------------------------------------------------------------------------------
 * 注意:如果你的网站有其它二级域名子站的情况时,请设置这个值,否则可以为空.
 * 如主站:www.XXXXX.com 子站:bbs.XXXXX.com 想要两个网站的会话共享时请一定要设置这里.
 * 格式:如果你的网站域名有 www.XXXX.com 和 bbs.XXXX.com 则将此设置为 XXXX.com
 * van domain 是指网站的图片域名
 * +------------------------------------------------------------------------------
 */
define ( 'MY_ROOT_DOMAIN', 'm-int.cn' );
define ( 'MY_APP_DOMAIN', 'devmtravelzoo.m-int.cn' );
define ( 'MY_VAN_DOMAIN', 'devtravelzoo.m-int.cn' );
define ( 'MY_FRONT_DOMAIN', 'devtravelzoo.m-int.cn' );

/**
 * +------------------------------------------------------------------------------
 * 所有页面是否强制浏览器不做缓存
 * +------------------------------------------------------------------------------
 * 值: TRUE:强制浏览器不做缓存 FALSE:浏览器默认
 * +------------------------------------------------------------------------------
 */

define ( 'MY_BROWSE_NO_CACHE', FALSE );

/**
 * +------------------------------------------------------------------------------
 * 自动防刷新提交功能
 * +------------------------------------------------------------------------------
 */

define ( 'MY_POST_NO_REFRESH', false ); // true:不允许 false:允许刷新提交(默认)
define ( 'MY_POST_INTERVAL_TIME', 3 ); // 定义同一页面两次提交的时间间隔.单位:秒

/**
 * +------------------------------------------------------------------------------
 * 是否启用验证码
 * +------------------------------------------------------------------------------
 */
define ( 'PHOTO_VERIFY', 1 ); // 1 启用 0 不启用

/**
 * +------------------------------------------------------------------------------
 * 魔术引用字符控制,可以防SQL注入
 * +------------------------------------------------------------------------------
 */

define ( 'MY_MAGIC_QUOTES', true ); // true:对REQUEST字符中引号加 "\",如用户输入 "a'b" 则自动转为
                                   // "a\'b"
                                   // false:去掉自动加上的 "\"(如果系统默认打开了
                                   // magic_quotes_gpc = On )

/**
 * +-----------------------------------------------------------------------------------------------------------------------------------
 * URL 模式配置
 * +-----------------------------------------------------------------------------------------------------------------------------------
 * - STANDARD - 标准模式，例如 index.php?c=main&a=Login&uid=1
 * - PATHINFO - PATHINFO 模式（默认）(兼容STANDARD模式)，例如 index.php/main/Login/uid/1
 * - REWRITE - URL 重写模式(兼容STANDARD模式)，例如 /main/Login/uid/1
 * - SEO - URL 重写模式,例如/main_Login_uid_1.html
 * +-----------------------------------------------------------------------------------------------------------------------------------
 * 使用: 强烈建议开发者在程序中(包括模板)都应该使用 url() 函数来得到url地址
 * 注意: REWRITE模式需要对环境进行初始
 * +-----------------------------------------------------------------------------------------------------------------------------------
 */

define ( 'MY_URL_METHOD', 'SEO' ); // 请求URL模式,取以上三种值之一.

/**
 * +------------------------------------------------------------------------------
 * URL伪静态扩展名,主要是对SEO有用.建议使用,最佳建议还是采用rewrite进进SEO优化
 * +------------------------------------------------------------------------------
 * MY_URL_METHOD 模式为 PATHINFO 或 REWRITE 时才有用,该伪静态扩展名是兼容这两种模式普通方法的
 * - PATHINFO - PATHINFO 模式（默认）(兼容STANDARD模式)，例如 index.php/main/Login/uid/1
 * - REWRITE - URL 重写模式，例如 /main/Login/uid/1　
 * 两个请求执行的结果是一样的
 * +------------------------------------------------------------------------------
 */

define ( 'MY_URL_EXT', '.html' ); // 伪静态扩展名,可以定义为空
define ( 'MY_HTTP_404_PAGE', '' ); // HTTP 404
                                                                     // 找不到指定页面时跳转的页面或URL址址

/**
 * +------------------------------------------------------------------------------
 * 应用程序常量配置
 * +------------------------------------------------------------------------------
 */

define ( 'MY_CONTROLLER_PATH', MY_APP_ROOT . 'Controller/' ); // 控制器的根目录
define ( 'MY_MODEL_PATH', MY_PUBLIC_ROOT . 'Models/' ); // 模型的根目录
                                                       // define('MY_MODEL_PATH'
                                                       // ,MY_APP_ROOT.'Models/');
                                                       // // 模型的根目录
define ( 'MY_VIEW_PATH', MY_APP_ROOT . 'Views/' ); // 视图,即Het的模板文件根目录

define ( 'MY_COMMON_PATH', MY_APP_ROOT . 'Common/' ); // 自定义类库的根目录(如果有则一定要定义它)
define ( 'MY_APP_CFG_PATH', MY_APP_ROOT . 'Configs/' ); // 项目所有配置文件路径,如项目中专有的常量定义等
define ( 'MY_APP_LIB_PATH', MY_APP_ROOT . 'Libs/' ); // 项目专有函数库路径
define ( 'MY_APP_LANG_PATH', MY_APP_ROOT . 'Lang/' ); // 项目专有语言文件路径(如果项目是多语言版本可能用到)

define ( 'MY_APP_LOAD_CFG', MY_APP_CFG_PATH . 'AppConfig.inc.php' ); // 自动载入的配置文件,如果没有则定义为空
define ( 'MY_APP_LOAD_LIB', MY_APP_LIB_PATH . 'appFunction.inc.php' ); // 自动载入的项目函数库,如果没有则定义为空

define ( 'MY_SHOWMSG_TPL', '' ); // 页面跳转时的页面,文件路径是相关于视图的根目录;如果没有定义则使用框架自带的页面
define ( 'MY_SAFE', '&*GHdc!P65hu12Ih' ); // 安全码,可以随意设置，比如
                                       // S1$t8|安全码abc|897864|T飞s!|M6TDQ3T9HT 等

/**
 * +------------------------------------------------------------------------------
 * 框架运行状态标识文件目录 和 系统Log日志 常量配置
 * +------------------------------------------------------------------------------
 */
define ( 'MY_RUNSTAT_PATH', MY_APP_ROOT . 'Cache/Data/' ); // 框架运行状态标识文件目录,这个目录下的文件最好不要随意删除.
define ( 'MY_LOGS_PATH', MY_APP_ROOT . 'Cache/Logs/' ); // Logs日志文件路径,使用
                                                       // Tools\Log.php 工具时用到

/**
 * +------------------------------------------------------------------------------
 * 缓存Cache常量配置
 * +------------------------------------------------------------------------------
 */
define ( 'MY_CACHE_ENABLE', false ); // 缓存是否有效
define ( 'MY_CACHE_DEFAULT_TYPE', 'file' ); // 缓存默认类型,可选项有: apc , file , memcache
define ( 'MY_CACHE_FILE_PATH', MY_APP_ROOT . 'Cache/File_Cache/' ); // file
                                                              // 缓存类型时保存文件的存路径,要有可写属性.
define ( 'MY_CACHE_DEFAULT_LIFETIME', 60 ); // 默认缓存时间,单位为秒

/**
 * +------------------------------------------------------------------------------
 * cache_lite缓存配置
 * +------------------------------------------------------------------------------
 * 调用时用unserialize来恢复
 * $cache = new Cache_Lite(unserialize(CACHE_OPTIONS));
 * +------------------------------------------------------------------------------
 */
define ( 'CACHE_OPTIONS', serialize ( array ('cacheDir' => MY_APP_ROOT . 'Cache/File_Cache/', 'fileLocking' => true, // 启用 / 禁用 文件锁定
'writeControl' => true, // 启用 / 禁用 写入控制
'readControl' => false, // 启用 / 禁用 读取控制
'fileNameProtection' => false, // 关闭文件名安全模式。cache id和组名将直接应用到
                             // cache文件的文件名，所以要小心使用特殊字符.
'automaticSerialization' => false, // 关闭自动序列
'hashedDirectoryLevel' => 2, // 设置两级缓存路径，缓存较多可以多以设置成1或2
'lifeTime' => 3600 )// 　根据程序需要可以在调用时设定具体时间
 ) );

/**
 * +------------------------------------------------------------------------------
 * 核心框架常量配置
 * +------------------------------------------------------------------------------
 */

define ( 'MY_EXT', '.php' ); // 文件扩展名
define ( 'MY_CONTROLLER_NAME', 'c' ); // 执行的PHP控制器文件,URL中的参数
define ( 'MY_ACTION_NAME', 'a' ); // 执行的控制器文件中的那个方法动作,URL中的参数
define ( 'MY_DEFAULT_CONTROLLER', 'main' ); // 如果没有定义URL中 c 则执行定义的这个默认的控制器文件
define ( 'MY_DEFAULT_ACTION', 'index' ); // 如果没有指定 &a 则执行这个默认的方法
define ( 'MY_EMPTY_ACTION', '__error' ); // 控制器中自定义空操作的动作,如果访问没有定义的动作时自动调用该方法,否则框架做默认处理

/**
 * +------------------------------------------------------------------------------
 * 视图相关定义
 * +------------------------------------------------------------------------------
 */

define ( 'MY_FORCE_CHARSET', true ); // 是否强制使用字符集,如果模板不是UTF=8请关闭该选择
define ( 'MY_CHARSET', 'utf-8' ); // 模板编码
define ( 'MY_LANG', 'zh-cn' ); // 语言
define ( 'MY_TEMPLATE', 'default' ); // 默认模板
define ( 'MY_TPL_ENGINE', 'Het' ); // 定义模板引擎,默认Het

/**
 * +------------------------------------------------------------------------------
 * 模板控制选项
 * +------------------------------------------------------------------------------
 */
define ( 'MY_CACHE_DEFAULT', false ); // 是否开启页面缓存

/**
 * +------------------------------------------------------------------------------
 * Het 控制选项
 * +------------------------------------------------------------------------------
 */
define ( 'MY_HET_COMPILE_DIR', MY_APP_ROOT . 'Cache/Het_Compile/' ); // HET編譯目錄(一定要可写)
define ( 'MY_HET_CACHE_DIR', MY_APP_ROOT . 'Cache/Het_Cache/' ); // HET緩存目錄(一定要可写)
define ( 'MY_HET_CACHE_SAFE', false ); // 是否啟用緩存安全模式
define ( 'MY_HET_SINGLE_DIR', false ); // 是否單目錄模式
define ( 'MY_HET_FN', serialize ( array ('date', 'gmdate', 'strtotime', 'empty', 'time', 'include_parse', 'include_tpl', 'response_get', 'msubstr', 'substr' ) ) ); // 默認模板支持函數

/**
 * +------------------------------------------------------------------------------
 * Valid 表单验证相关配置(
 * 证规则失败时默认调用的处理函数(如果用到 Valid 组件时要定义它)
 * +------------------------------------------------------------------------------
 * - 'alert' :调用 javascript 自身的 alert() 函数显示错误消息
 * - '_default_' :调用框架自定义的处理函数显示错误消息
 * +------------------------------------------------------------------------------
 */
define ( 'MY_VALID_DEFAULT_FUN', '_default_' );

/**
 * +------------------------------------------------------------------------------
 * ACL 权限相关配置
 * +------------------------------------------------------------------------------
 */

/**
 * [说明]:
 *
 * 框架中对资源的定义是控制器加其方法组合,如 "/?c=admin_member&a=add" 对应的资源为 "admin_member_add"
 *
 * 框架对权限的检查判断有以下两种工作方式:
 *
 * 1:框架自动判断:这种方式的优点是:全自动,所有权限的判断在用户程序执行前已完成,所以用户不需要关系访问权限问题.
 * 但缺点是:不灵活,即用户不能自由控制权限的相关流程;
 *
 * 2:用户自行判断:这种方式的优点是:非常录活,即用户程序可以自已处理权限检查的所有流程,
 *
 * 框架中权限的控制是基于数据库的方法,使用前(如果使用权限机制)请初始化其数据表,具体请看手册:
 */

define ( 'MY_ACL_ACCESS', false ); // 是否使用权限检测机制, false:表示系统中不使用权限检测,所有资源都可以执行
define ( 'MY_ACL_AUTO_CHECK', false ); // 框架是否自动检测权限,否则权限交给用户自行判断
define ( 'MY_ACL_CACHE', false ); // 是否将用户权限做绶存.(缓存可减少数据库资源,但不能及时更新)
define ( 'MY_ACL_CACHE_METHOD', 'session' ); // 权限缓存方式,可选session,memcache
define ( 'MY_DEFAULT_PAGE', '' ); // 当无权限时跳转到这个默认URL页面,定义为空则返回到上一个页面

/**
 * 绝对允许资源列表,即这些资源不受权限机制的控制,永远可以执行
 * 注意:各资源间用 "," 进行连接,如"member_index_add,school_index_*" 其中 "*" 表示该控件器的所有方法
 */
define ( 'MY_ACL_ACCESS_ALLOW', '' );

/**
 * +------------------------------------------------------------------------------
 * 权限相关表常量定义
 * +------------------------------------------------------------------------------
 */

define ( 'MY_ACL_TAB_ENGINE', 'MyISAM' ); // ACL相关表类型
define ( 'MY_ACL_TAB_CHARSET', MY_DB_DEFAULT_CHARSET ); // ACL相关表字符编码
define ( 'MY_ACL_TAB_PREFIX', 'my_' ); // ACL相关表前缀

/**
 * +------------------------------------------------------------------------------
 * SESSION 相关配置
 * +------------------------------------------------------------------------------
 * SESSION管理方式 ,如果要做多台服务器之间SESSION的共享话则可以选择 "file" 或 "db" 方式
 * 'system':PHP系统默认
 * 'memcache':自定义 Memcache 管理方式,需要设置 MY_MEMCACHE_HOSTS 常量.
 * 'file':自定义文件方式,需要指定 session 文件的保存目录,一定要可写权限.
 * 'db':自定义数据库方式,要建立一张专有的SESSION表.具体可调用用静态类方法 SESSION:init() 来初始化,
 * 详细方法请看以下说明.
 * 以上 'file','db' 自定义方式都可以调用 SESSION::NUM() 得到在 session.gc_maxlifetime
 * 时间内当前在线人数(概数)
 * +------------------------------------------------------------------------------
 */
define ( 'MY_SESSION_HANDLER', 'system' ); // system , file , db , memcache
define ( 'MY_SESSION_LIFETIME', 0 ); // SESSION有效时间单位秒. 0:使用系统默认时间,一般为:1440秒=24分钟
                                  
// 'file'方式时必需要定义以下常量
define ( 'MY_SESSION_SAVE_PATH', MY_APP_ROOT . 'Cache/Session/' ); // SESSION文件保存的目录
                                                             // 'db'方式时要定义以下常量
define ( 'MY_SESSION_TAB_ENGINE', 'MyISAM' ); // 表类型
define ( 'MY_SESSION_TAB_CHARSET', MY_DB_DEFAULT_CHARSET ); // 表字符集编码
define ( 'MY_SESSION_TAB_PREFIX', 'MY_' ); // 表名前缀

/**
 * +------------------------------------------------------------------------------
 * 框架钩子函数定义,每个函数具体的定义请看 AppHooks.inc.php 中说明
 * +------------------------------------------------------------------------------
 */

define ( 'MY_CORE_HOOK_FILE', MY_APP_ROOT . 'Hook/AppHooks.inc.php' );
?>