@echo off
candle "ActivityLogger 1.0.0.wxs"
light "ActivityLogger 1.0.0.wixobj" -ext WixUIExtension -dWixUILicenseRtf=License.rtf -out "ActivityLogger 1.0.0.msi"
del "ActivityLogger 1.0.0.wixobj"
del "ActivityLogger 1.0.0.wixpdb"