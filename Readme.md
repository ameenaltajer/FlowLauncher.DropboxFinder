### Description

DropboxFinder is plugin for Flow Launcher to search files directly within Dropbox, this makes finding files super easy from the launcher directly, I've found this super helpful when I was trying to compare Dropbox search results against Windows search (indexing), this proves to be super helpful when you have Dropbox setup in the online mode only, since the Windows indexer will not be able to find the files on your local local machine.

![Plugin Demo](https://i.imgur.com/44x1RsI.gif)

### Usage

    df <name of the file you're looking for>
    
    
### Fast testing

I have written a quick testing utility named debug.ps1 to easy test my changes but closing Flow Launcher and loading the plugin automatically during development.
    
    
### Contribution

If you like where the project is going please send me PRs of any valuable change you make, thanks in advance!

### Todo

- OAuth authorization
- Error handling
- Icon & thumbnail loading
- Caching
- Settings menu and presistant options
- Setup Github workflow for CI/CD
- Publish a PR to Flow Launcher's plugin manifest
