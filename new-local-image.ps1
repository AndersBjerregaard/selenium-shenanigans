$TARGET_DIR = ".\"
$TARGET_TAG = ".\tags.txt"
$TAG_VALUE
$DOCKERFILE = ".\tests.Dockerfile"

Write-Output "Generating build tag..."

if (!(Test-Path -Path $TARGET_TAG -PathType leaf)) {
    Write-Output "Generating tags file..."
    New-Item -path ".\" -name "tags.txt" -type "file" -value "v0.1"
    $TAG_VALUE = "v0.1"
} else {
    # Read the content of the file
    $tags = Get-Content -Path $TARGET_TAG
    $latestTag
    $remainder
    if ($tags.GetType().Name -eq "String") {
        # Only one line
        $latestTag = $tags
    } else {
        $latestTag = $tags[0]
        $remainder = $tags[1..($tags.Length - 1)]
    }
    $version = $latestTag.Split(".")
    $oldVersion = $version[-1]
    $newVersion = [int]$oldVersion + 1
    $newValue = "v0.$newVersion"
    $tags = $newValue, $latestTag + $remainder

    Set-Content -Path $TARGET_TAG -Value $tags
    $TAG_VALUE = $newValue
}

Write-Output "Building docker image..."

docker build --tag "se-script-app:$TAG_VALUE" --tag "se-script-app:latest" --file $DOCKERFILE .

Write-Output "Complete! New docker image is tagged as 'se-script-app:$TAG_VALUE' and 'se-script-app:latest'"
Exit 0
