@echo off
candle "ActivityLogger 1.0.5.wxs"
light "ActivityLogger 1.0.5.wixobj" -ext WixUIExtension -ext WixUtilExtension -dWixUILicenseRtf=License.rtf -out "ActivityLogger 1.0.5.msi"
del "ActivityLogger 1.0.5.wixobj"
del "ActivityLogger 1.0.5.wixpdb"
pause