REG ADD HKLM\Software\Informix
REG ADD HKLM\Software\Informix\Environment
REG ADD HKLM\Software\Informix\Environment /v Data /t REG_SZ /d %1
SET INFORMIXDIR=%CD%
SETX /U PATH "%CD%;%PATH%"
