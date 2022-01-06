# 文件对比
* 参数1为原始文件
* 参数2为更新文件
* 参数3~...为需要的文件类型（后缀），默认`["bnk", "wem"]`，添加all为全部文件
```angular2html
FileCompare.exe --path-old path_old --path-new path_new --extension all --output output_path
```