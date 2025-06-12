#!/bin/bash

# Run dotnet format with passed arguments
dotnet format --no-restore --verbosity detailed --include "$@"

# We can do git add . since pre-commit already stashed any thing that we 
# don't want to commit
git add .