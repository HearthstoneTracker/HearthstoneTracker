@echo off

cd /d "%~p0"

call buildrelease.cmd

"C:\Program Files (x86)\NSIS\makensis.exe" ".\installer\HearthstoneTracker.nsi"