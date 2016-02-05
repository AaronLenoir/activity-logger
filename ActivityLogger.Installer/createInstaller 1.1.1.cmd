@echo off
set version=1.1.1
candle "ActivityLogger %version%.wxs" -ext WixUtilExtension
light "ActivityLogger %version%.wixobj" -ext WixUIExtension -ext WixUtilExtension -dWixUILicenseRtf="License 1.1.1.rtf" -out "ActivityLogger %version%.msi"
del "ActivityLogger %version%.wixobj"
del "ActivityLogger %version%.wixpdb"
pause
