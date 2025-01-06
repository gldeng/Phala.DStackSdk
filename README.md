# Phala DStack Example App

This is a simple example app that demonstrates how to use the Phala DStack SDK.

## Phala Tappd Simulator

```bash
docker pull phalanetwork/tappd-simulator:latest
docker run --rm -p 8090:8090 phalanetwork/tappd-simulator:latest
```

## Building the app

```bash
docker build -t dstack-example-app:latest -f ExampleApp/Dockerfile .
```

## Running the app

```bash
docker run --rm -p 8080:8080 -p 8081:8081 dstack-example-app:latest
```

## Check the Swagger UI

```bash
http://localhost:8080/swagger/index.html
```

## API Endpoints

http://localhost:8080/api/derivekey

http://localhost:8080/api/tdxquote

## Reference

https://docs.phala.network/dstack/getting-started/start-from-scratch    