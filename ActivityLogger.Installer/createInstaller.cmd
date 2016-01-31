@echo off
set version=1.0.11
candle "ActivityLogger %version%.wxs" -ext WixUtilExtension
light "ActivityLogger %version%.wixobj" -ext WixUIExtension -ext WixUtilExtension -dWixUILicenseRtf=License.rtf -out "ActivityLogger %version%.msi"
del "ActivityLogger %version%.wixobj"
del "ActivityLogger %version%.wixpdb"
pause
