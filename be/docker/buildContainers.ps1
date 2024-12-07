$originalPath = Get-Location
Set-Location -Path (Resolve-Path ..\..\)

$tag = "0.0.1"

docker build -f .\be\docker\MessageProcessor.dockerfile -t system-design-kata-message-processor:$tag .
docker build -f .\be\docker\EdgeDevice.dockerfile -t system-design-kata-edge-device:$tag .

Set-Location -Path $originalPath
