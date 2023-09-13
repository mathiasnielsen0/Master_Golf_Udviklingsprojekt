# Container and image name for InfluxDB
$CONTAINER_NAME = "Golf_influxdb"
$IMAGE_NAME = "influxdb:latest"

Write-Host "Stopping and removing existing container if it exists..."
docker stop $CONTAINER_NAME
docker rm $CONTAINER_NAME

Write-Host "Starting a new InfluxDB container..."
docker run -d --name $CONTAINER_NAME -p 8086:8086 -e INFLUXDB_REPORTING_DISABLED=true $IMAGE_NAME

# Give InfluxDB a moment to initialize. Adjust this if necessary.
Start-Sleep -Seconds 15

# Initial InfluxDB setup
Write-Host "Setting up initial user, organization, and bucket..."
$USERNAME = "InfluxTestUser"
$PASSWORD = "Influx44!"
$ORG = "Sparinvest"
$BUCKET = "Holdings"

$body = @{
    username = $USERNAME
    password = $PASSWORD
    org      = $ORG
    bucket   = $BUCKET
    token    = "mytoken"
} | ConvertTo-Json

Invoke-RestMethod -Method Post -Uri "http://localhost:8086/api/v2/setup" -Headers @{ "accept"="application/json" } -Body $body

Write-Host "Setup complete."

# Get authorization token
Write-Host "Getting authorization token..."
$creds = [System.Text.Encoding]::ASCII.GetBytes("${USERNAME}:${PASSWORD}")
$base64 = [System.Convert]::ToBase64String($creds)
$headers = @{ Authorization=("Basic " + $base64) }

try {
    $response = Invoke-RestMethod -Method Post -Headers $headers -Uri 'http://localhost:8086/api/v2/signin'
    $SESSION_TOKEN = $response.token
    Write-Host "SESSION TOKEN: $SESSION_TOKEN"
} catch {
    Write-Host "Error retrieving SESSION TOKEN: $_"
    Write-Host $_.Exception.Response
    Write-Host $_.Exception.Response.StatusDescription
}

# Create API token
Write-Host "Creating API token..."
$headers = @{ Authorization=("Token " + $SESSION_TOKEN) }
$body = @{
    org          = $ORG
    permissions  = @('read','write')
    description  = 'My API Token'
} | ConvertTo-Json

try {
    $NEW_API_TOKEN = (Invoke-RestMethod -Method Post -Headers $headers -Uri 'http://localhost:8086/api/v2/authorizations' -Body $body).token.id
    # Setting the API token as an environment variable
    [Environment]::SetEnvironmentVariable("INFLUX_API_TOKEN", $NEW_API_TOKEN, [System.EnvironmentVariableTarget]::User)
    Write-Host $NEW_API_TOKEN
} catch {
    Write-Host $_.Exception.Message
}
