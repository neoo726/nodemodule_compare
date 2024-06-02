# nodemodule_compare
## 运行环境
64位Windows系统

.Net framework 4.7.2+

## 背景
部署环境没有网络，也没有条件搭建本地仓库。无法执行npm install.

Nodejs后端项目部署，需要将dist和node_modules文件夹都拷贝过去的话，耗时很久。

## 解决方案
1. 通过对比新老版本的package-lock.json文件，获取版本变化和新增的module。

2. 提取有变化的module文件夹，复制到目标路径下。

3. 将文件夹压缩后，复制到目标服务器，并解压覆盖同名文件。

4. 重启后端服务

nodemodule_compare软件可以用来处理第1.2步。后面两步（3.4）手动完成。