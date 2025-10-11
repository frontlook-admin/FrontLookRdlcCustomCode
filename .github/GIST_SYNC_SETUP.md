# GitHub Actions Setup for Gist Sync

This repository automatically syncs to the Gist: https://gist.github.com/frontlook-admin/f8962078e18b40b958410cfb48db145b

## Setup Instructions

To enable automatic Gist synchronization, you need to create a GitHub Personal Access Token (PAT) and add it as a repository secret.

### Step 1: Create a Personal Access Token

1. Go to GitHub Settings → Developer settings → Personal access tokens → Tokens (classic)
   - Direct link: https://github.com/settings/tokens
2. Click "Generate new token" → "Generate new token (classic)"
3. Give it a descriptive name: `Gist Sync Token for FrontLookRdlcCustomCode`
4. Set expiration: Choose your preference (recommended: 1 year or no expiration)
5. Select scopes:
   - ✅ **gist** (required for updating gists)
6. Click "Generate token"
7. **Copy the token immediately** (you won't be able to see it again!)

### Step 2: Add Token as Repository Secret

1. Go to this repository's Settings → Secrets and variables → Actions
   - Direct link: https://github.com/frontlook-admin/FrontLookRdlcCustomCode/settings/secrets/actions
2. Click "New repository secret"
3. Name: `GIST_TOKEN`
4. Value: Paste the token you copied
5. Click "Add secret"

### Step 3: Test the Workflow

Once the secret is added, the workflow will automatically run on every push to the `main` branch when any of these files change:
- RdlcReportCode.vb
- RdlcReportCode_WithComments.vb
- RdlcVBCode_Usage
- Readme.md

To manually test it, push any change to these files or go to the Actions tab and manually trigger the workflow.

## How It Works

The GitHub Actions workflow (`.github/workflows/sync-gist.yml`) uses the `exuanbo/actions-deploy-gist` action to:
1. Checkout the repository
2. Read each specified file
3. Update the corresponding file in the Gist

## Gist Information

- **Gist ID:** f8962078e18b40b958410cfb48db145b
- **Gist URL:** https://gist.github.com/frontlook-admin/f8962078e18b40b958410cfb48db145b
- **Files synced:** 4 files from this repository

## Troubleshooting

If the sync fails:
1. Check that the `GIST_TOKEN` secret is set correctly
2. Verify the token has `gist` scope
3. Check the Actions tab for error messages
4. Ensure the Gist ID is correct: `f8962078e18b40b958410cfb48db145b`
