# scripts/release.py
import argparse
import subprocess
import sys
from pathlib import Path
import shutil

# --- Configuration ---
OS_TARGETS = {
    "win": "win-x64",
    "mac": "osx-arm64",
}

def run_command(command, cwd=None):
    """Runs a command in the shell, checks for errors, and prints the command."""
    command_list = command.split()
    print(f"\n▶️  Running command: '{command}'")
    if cwd:
        print(f"   in directory: '{cwd}'")

    try:
        subprocess.run(command_list, check=True, cwd=cwd, text=True, capture_output=False)
        print(f"✅  Successfully executed: '{command_list[0]}'")
    except FileNotFoundError:
        print(f"❌ ERROR: Command '{command_list[0]}' not found. Is it installed and in your PATH?")
        sys.exit(1)
    except subprocess.CalledProcessError as e:
        print(f"❌ ERROR: Command failed with exit code {e.returncode}.")
        sys.exit(1)

def main():
    """Main function to orchestrate the release build process."""
    parser = argparse.ArgumentParser(
        description="Automated build and release script for the Atmos project."
    )
    parser.add_argument(
        "--os",
        required=True,
        choices=OS_TARGETS.keys(),
        help=f"The target operating system. Choices: {list(OS_TARGETS.keys())}",
    )
    parser.add_argument(
        "--version",
        required=True,
        type=str,
        help="The build version for the release (e.g., 1.0.0)."
    )
    args = parser.parse_args()

    # --- Variables ---
    build_version = args.version
    target_os = OS_TARGETS[args.os]

    # --- Path Definitions ---
    root_dir = Path(__file__).resolve().parent.parent

    release_dir = root_dir / "release-zips"
    version_dir = release_dir / build_version
    app_dir = version_dir / "app"

    print("🚀 Starting Atmos Release Process 🚀")
    print(f"   Version: {build_version}")
    print(f"   Target OS: {target_os}")
    print(f"   Project Root: {root_dir}")
    print(f"   Release Directory: {version_dir}")
    print("-" * 40)    

    # --- Step 0: Create release directories ---
    print("▶️  Step 0: Creating release directories...")
    version_dir.mkdir(parents=True, exist_ok=True)
    app_dir.mkdir(exist_ok=True)
    print(f"✅  Directories '{version_dir}' and '{app_dir}' are ready.")

    # --- Step 1: Build the Atmos Launcher ---
    run_command(
        f"dotnet publish -c Release -r {target_os} --self-contained true -o {version_dir}",
        cwd=root_dir / "Atmos" / "Launcher"
    )

    # --- Step 2: Build the React App ---
    react_app_dir = root_dir / "Atmos" / "API" / "atmos-client"
    run_command("npm install", cwd=react_app_dir)
    run_command("npm run build", cwd=react_app_dir)

    # --- Step 3: Build the Atmos Migration Tool ---
    run_command(
        f"dotnet publish -c Release -r {target_os} --self-contained true -o {app_dir}",
        cwd=root_dir / "Atmos" / "Migrations"
    )

    # --- Step 4: Build the Atmos API ---
    run_command(
        f"dotnet publish -c Release -r {target_os} -o {app_dir}",
        cwd=root_dir / "Atmos" / "API"
    )

    # --- Step 5: Copy the Docker Compose File ---
    print("\n▶️  Step 5: Copying Docker Compose file...")
    docker_compose_src = root_dir / "Atmos" / "compose.yaml"
    docker_compose_dest = version_dir / "docker-compose.yml"
    shutil.copy(docker_compose_src, docker_compose_dest)
    print(f"✅  Copied '{docker_compose_src.name}' to '{version_dir}'.")

    # --- Step 6: Zip the Release Directory ---
    print("\n▶️  Step 6: Zipping the release directory...")
    # This creates a zip file containing the *contents* of the version_dir.
    # The result matches the desired 'Release Zip Structure' from your docs.
    archive_base_name = release_dir / build_version
    try:
        shutil.make_archive(
            base_name=archive_base_name,
            format='zip',
            root_dir=version_dir
        )
        print(f"✅  Successfully created release archive: '{archive_base_name}.zip'")
    except Exception as e:
        print(f"❌ ERROR: Failed to create zip archive. {e}")
        sys.exit(1)

    print("-" * 40)
    print("🎉 Release process completed successfully! 🎉")

if __name__ == "__main__":
    main()