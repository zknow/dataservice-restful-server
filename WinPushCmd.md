// 不包含Dotnet SDK(部署環境需要安裝)
dotnet publish -r win-x64 -o ./publish-win-x64 --self-contained false

// 包含Dotnet SDK，容量較大50M Up
dotnet publish -r win-x64 -o ./publish-win