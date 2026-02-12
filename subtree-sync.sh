#!/bin/bash
# run with git and command $ bash subtree-sync.sh
# ==== CONFIG ====
PREFIX="Helpers"
REMOTE="https://github.com/Utility-530/Utility.Helpers.git"
BRANCH="master"
SQUASH=""   # replace with "--squash" if you want to use squash
# =================

set -e

echo "Checking if there are local commits to push..."

# Fetch latest remote state
git fetch "$REMOTE" "$BRANCH"

LOCAL=$(git rev-parse HEAD)
REMOTE_HASH=$(git rev-parse FETCH_HEAD)

if [ "$LOCAL" != "$REMOTE_HASH" ]; then
    echo There are local commits that need to be pushed.
    echo Aborting subtree sync.
    exit 0
fi

echo "No local commits to push."
echo "Running subtree pull..."

git subtree pull --prefix=$PREFIX $REMOTE $BRANCH $SQUASH

echo "Running subtree push..."

git subtree push --prefix=$PREFIX $REMOTE $BRANCH

echo "Subtree sync complete."
