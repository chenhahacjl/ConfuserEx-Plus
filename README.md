# ConfuserEx 汉化增强版

本软件基于 ConfuserEx 二次编辑，增加一些保护插件，并且对界面进行汉化。

## 参考代码链接

ConfuserEx 代码：https://github.com/yck1509/ConfuserEx

dnlib 代码：https://github.com/yck1509/dnlib/tree/532c767a9a4f6af51cd4eb5d1c0af750c8509c5d

插件库1：https://github.com/RivaTesu/ConfuserEx-Additions/

插件库2：https://github.com/ForlaxPy/ConfuserEx-Additions-v2

---

==以下为原 README.md 内容==

---

ConfuserEx
========

ConfuserEx is a open-source protector for .NET applications.
It is the successor of [Confuser](http://confuser.codeplex.com) project.

NOTICE
======
This project is discontinued and unmaintained. Alternative forked projects can be found in [this issue](https://github.com/yck1509/ConfuserEx/issues/671).

Features
--------
* Supports .NET Framework 2.0/3.0/3.5/4.0/4.5
* Symbol renaming (Support WPF/BAML)
* Protection against debuggers/profilers
* Protection against memory dumping
* Protection against tampering (method encryption)
* Control flow obfuscation
* Constant/resources encryption
* Reference hiding proxies
* Disable decompilers
* Embedding dependency
* Compressing output
* Extensible plugin API
* Many more are coming!

Usage
-----
`Confuser.CLI <path to project file>`

The project file is a ConfuserEx Project (*.crproj).
The format of project file can be found in docs\ProjectFormat.md

Bug Report
----------
See the [Issues Report](http://yck1509.github.io/ConfuserEx/issues/) section of website.


License
-------
See LICENSE file for details.

Credits
-------
**[0xd4d](https://github.com/0xd4d)** for his awesome work and extensive knowledge!  
Members of **[Black Storm Forum](http://board.b-at-s.info/)** for their help!
