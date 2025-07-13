# Release

Release will be done via github actions. Soon there will be a script to automate this process. Then, a CI/CD pipeline will be created to automate the release process.

## Release Zip Structure (For Installer Compatibility)

```md
/atmos-launcher.exe         ← Your compiled, self-contained launcher
/docker-compose.yml         ← The docker-compose file for Postgres
/app/                       ← A folder containing the main application
    /atmos.exe
    /atmos-migrate.exe
    /wwwroot/
    /... (all other required DLLs and files)
```

# Steps to release

Set the target OS (`osx-arm64` for Mac or `win-x64` for Windows)
```bash
export TARGET_OS=osx-arm64
```

Set the build version

```bash
export BUILD_VERSION=1.0.0 && echo $BUILD_VERSION
```

```bash
mkdir release-zips && mkdir release-zips/$BUILD_VERSION
```

## Step 1: Build the Atmos Launcher

```bash
cd ./Atmos/Launcher && dotnet publish -c Release -r $TARGET_OS --self-contained true -o ../../release-zips/$BUILD_VERSION && cd ../..
```

## Step 2: Build the React App

```bash
cd ./Atmos/API/atmos-client && npm install && npm run build && cd ../../..
```


## Step 3: Build the Atmos Migration Tool

```bash
cd ./Atmos/Migrations && dotnet publish -c Release -r $TARGET_OS --self-contained true -o ../../release-zips/$BUILD_VERSION/app && cd ../..
```

## Step 4: Build the Atmos API

```bash
cd ./Atmos/API && dotnet publish -c Release -r $TARGET_OS -o ../../release-zips/$BUILD_VERSION/app && cd ../..
```

## Step 5: Copy the Docker Compose File

```bash
cp ./Atmos/compose.yaml ./release-zips/$BUILD_VERSION
```

## Step 6: Zip the Release Directory

```bash
cd ./release-zips && zip -r $BUILD_VERSION.zip $BUILD_VERSION/$BUILD_VERSION && cd ..
```


### One liner to do all steps

```bash
cd ./Atmos/Launcher && dotnet publish -c Release -r $TARGET_OS --self-contained true -o ../../release-zips/$BUILD_VERSION && cd ../.. && cd ./Atmos/API/atmos-client && npm install && npm run build && cd ../../.. && cd ./Atmos/Migrations && dotnet publish -c Release -r $TARGET_OS --self-contained true -o ../../release-zips/$BUILD_VERSION/app && cd ../.. && cd ./Atmos/API && dotnet publish -c Release -r $TARGET_OS -o ../../release-zips/$BUILD_VERSION/app && cd ../.. && cp ./Atmos/compose.yaml ./release-zips/$BUILD_VERSION && cd ./release-zips && zip -r $BUILD_VERSION.zip $BUILD_VERSION && cd ..
```