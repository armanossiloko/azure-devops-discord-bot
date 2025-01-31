# Get the current directory path
$currentDirectory = Get-Location

# Function to remove bin/ and obj/ folders recursively
function Remove-BinObjFolders {
    param (
        [string]$folderPath
    )

    # Get all subdirectories in the current folder
    $subdirectories = Get-ChildItem -Path $folderPath -Directory

    # Loop through each subdirectory
    foreach ($subdirectory in $subdirectories) {
        $subdirectoryPath = $subdirectory.FullName

        # Check if the subdirectory ends with '\bin' or '\obj' and remove it
        if ($subdirectory.Name -eq 'bin' -or $subdirectory.Name -eq 'obj') {
            Remove-Item -Path $subdirectoryPath -Recurse -Force
            Write-Host "Removed: $subdirectoryPath"
        }
        else {
            # Recursively call the function for subdirectories
            Remove-BinObjFolders -folderPath $subdirectoryPath
        }
    }
}

# Call the function to remove bin/ and obj/ folders recursively in the current directory
Remove-BinObjFolders -folderPath $currentDirectory
