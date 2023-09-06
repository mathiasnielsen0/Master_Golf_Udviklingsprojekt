@echo off

REM Container and image name for InfluxDB
set CONTAINER_NAME=Golf_influxdb
set IMAGE_NAME=influxdb:latest

echo Stopping and removing existing container if it exists...
docker stop %CONTAINER_NAME%
docker rm %CONTAINER_NAME%

echo Starting a new InfluxDB container...
docker run -d --name %CONTAINER_NAME% -p 8086:8086 -e INFLUXDB_REPORTING_DISABLED=true %IMAGE_NAME%

REM Give InfluxDB a moment to initialize. Adjust this if necessary.
timeout 15

REM Initial InfluxDB setup
echo Setting up initial user, organization, and bucket...
set USERNAME=InfluxTestUser
set PASSWORD=Influx4¤
set ORG=Sparinvest
set BUCKET=Holdings

curl -X POST "http://localhost:8086/api/v2/setup" -H "accept: application/json" -d "{\"username\":\"%USERNAME%\",\"password\":\"%PASSWORD%\",\"org\":\"%ORG%\",\"bucket\":\"%BUCKET%\",\"token\":\"mytoken\"}"

echo Setup complete.
