# System Design Kata

## Add user secrets:
```json
"ServiceBus": {
        "ConnectionString": "SERVICE BUS CONNECTION STRING",
        "TopicName": "SERVICE BUS TOPIC NAME"
    }
```

## Docker-compose
### Build docker image
From repository root, run:
```powershell
docker build -f .\be\docker\EdgeDevice.dockerfile -t system-design-kata-edge-device:0.0.1 .
```

### Add .env file:
```powershell
ServiceBus__ConnectionString={SERVICE BUS CONNECTION STRING}
ServiceBus__TopicName={SERVICE BUS TOPIC NAME}
```

