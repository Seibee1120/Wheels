 ANOMALY: use of REX.w is meaningless (default operand size is 64)

 解决方法：
 regedit，打开注册表；
 \HKEY_LOCAL_MACHINE\SOFTWARE\TEC\Ocular.3\agent\config下，新建[字符串值]hookapi_disins
 ，数值数据1
 \HKEY_LOCAL_MACHINE\SOFTWARE\TEC\Ocular.3\agent\config 下，新建 [字符串值]hookapi_filterproc_external，数值数据: cmd.exe;powershell.exe;git.exe;idea64.exe