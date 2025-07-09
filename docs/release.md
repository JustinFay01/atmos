# Release

Release will be done via github actions. Soon their will be a script to automate this process. Then, a CI/CD pipeline will be created to automate the release process.

# Steps to release

**Mac: osx-arm64**
```bash
cd ./API/atmos-client && npm run build && cd ../.. && dotnet publish -c Release -r osx-arm64 --self-contained true -o ./publish/<build-version>-osx
```
**Windows: win-x64**
```bash
cd ./API/atmos-client && npm run build && cd ../.. && dotnet publish -c Release -r win-x64 --self-contained true -o ./publish/<build-version>-win
```

