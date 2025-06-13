#!/bin/sh
set -e
LC_ALL=C

echo "🚀 Running pre-commit dotnet format"

# Get the list of staged MAUI-related files
FILES=$(git diff --cached --name-only --diff-filter=ACM "*.cs" | sed 's| |\\ |g')

if [ -z "$FILES" ]; then
  echo "🤖 No staged C#, XAML, or Razor files to format. Committing Changes."
  exit 0
fi

# Run dotnet format
echo "🔍 Running dotnet format..."
echo "$FILES" | cat | xargs | sed -e 's/ /,/g' | xargs dotnet format Atmos/ --no-restore --include

# Re-add potentially modified files
echo "$FILES" | while read -r line; do
    git add "$line"
done

echo "✅ dotnet format done. Committing changes."