@echo off
candle "ActivityLogger 1.0.0.wxs" "WixUI_ActivityLogger.wxs"
light "ActivityLogger 1.0.0.wixobj" "WixUI_ActivityLogger.wixobj" -ext WixUIExtension -out "ActivityLogger 1.0.0.msi"
del "ActivityLogger 1.0.0.wixobj"
del "WixUI_ActivityLogger.wixobj"
del "ActivityLogger 1.0.0.wixpdb"