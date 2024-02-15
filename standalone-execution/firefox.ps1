Write-Host "Stopping existing service(s)..."

docker compose --file ".\firefox-compose.yaml" down

Write-Host "Starting service(s)..."

docker compose --file ".\firefox-compose.yaml" up -d

Write-Host "Done!"