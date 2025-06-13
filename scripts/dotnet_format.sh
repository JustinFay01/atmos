#!/bin/bash
LC_ALL=C

# Select files to format
[ -f .git/index.lock ] && rm .git/index.lock

if [ "$#" -gt 0 ]; then
  FILES="$@"
  echo "Formatting files: $FILES"
else
  FILES=$(git diff --cached --name-only --diff-filter=ACM "*.cs" | sed 's| |\\ |g')
fi

[ -z "$FILES" ] && exit 0

# Format all selected files
echo "$FILES" | cat | xargs | sed -e 's/ /,/g' | xargs dotnet format Atmos/ --include

# Add back the modified files to staging
echo "$FILES" | xargs git add

exit 0