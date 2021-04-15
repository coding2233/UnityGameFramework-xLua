# XLua 全热更实现

## 开发步骤
* 更新子模块 `git submodule update --init --recursive`
* Unity工具栏`XLua/Generate Code`生成XLua的适配文件
* `UniTask`启动对`DoTween`的支持，需要添加`UNITASK_DOTWEEN_SUPPORT`的宏
* 测试服需要添加`TEST`宏
* 打开场景文件`Game/Scenes/main`运行
* 主工程逻辑入口`LaunchOverStartState.cs`
* `lua`逻辑入口`main.lua.txt`
* 资源打包参考脚本`CommandBuild.cs`，同样可参考`.drone.yml`
* 菜单栏`Tools`有一些辅助小工具
* 涉及到账号密码的地方，暂时屏蔽为`*`，体验完整工具链，需要自己修改部分代码和配置文件
* 如果条件支持，可以考虑提供阿里云OSS文件服务器测试地址
* 示例安装地址: [https://wanderer-x.itch.io/primitive](https://wanderer-x.itch.io/primitive)
* 详细开发文档后续更新。。。