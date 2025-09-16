# SimpleBlog with Kafka Activity Logging

This application uses Kafka for activity logging with fault tolerance to prevent data loss during power cuts or server failures.

## Prerequisites

1. Docker and Docker Compose
2. .NET 9.0 SDK
3. SQL Server (for main application database)
4. ClickHouse (for activity log storage)

## Setup Instructions

### 1. Start Kafka Cluster

```bash
# Navigate to the project root directory
cd SimpleBlog

# Start the Kafka cluster using the helper script
start-kafka.bat  # On Windows
```

Or manually:
```bash
# Start the Kafka cluster (single broker setup)
docker-compose up -d

# Wait for Kafka to be ready (about 30 seconds)
# Then create the topic
docker exec kafka kafka-topics --create --topic user-activity-logs --partitions 3 --replication-factor 1 --if-not-exists --bootstrap-server localhost:9092
```

### 2. Configure Application

1. Update the ClickHouse connection string in `SimpleBlog.Infrastructure\Services\UserActivityService.cs`:
   ```csharp
   _connStr = "Host=your-clickhouse-host;Port=8123;Username=your-user;Password=your-password;Database=default";
   ```

2. Update the SQL Server connection string in `appsettings.json` if needed.

### 3. Run the Application

```bash
# Navigate to the Presentation layer
cd SimpleBlog.Presentation

# Run the application
dotnet run
```

## Architecture

```
[Application Server] --(Network)--> [Kafka Docker (Single Broker)] --(Network)--> [ClickHouse Server]
```

## Fault Tolerance Features

1. **Persistent Queue**: Activities are stored on disk until successfully delivered to Kafka
2. **Graceful Shutdown**: All pending activities are flushed to disk during shutdown
3. **Automatic Recovery**: The system automatically recovers from crashes and continues processing
4. **Fallback Mechanism**: If Kafka is unavailable, the system continues to work using local storage

## How It Works

1. User activities are captured by the `EnhancedActivityLogAttribute`
2. Activities are immediately sent to Kafka (single broker)
3. If Kafka is unavailable, activities are stored in a local persistent queue
4. The `KafkaActivityConsumer` consumes messages and stores them in ClickHouse
5. The `PersistentQueueProcessor` periodically retries sending any queued activities

## Managing the System

### Start Kafka Cluster
```bash
start-kafka.bat  # On Windows
# or
docker-compose up -d
```

### Stop Kafka Cluster
```bash
stop-kafka.bat  # On Windows
# or
docker-compose down
```

### View Kafka Logs
```bash
docker-compose logs kafka
```

### Check Kafka Status
```bash
docker-compose ps
```

## Troubleshooting Kafka Issues

### Common Issues and Solutions

1. **Kafka container starts but immediately stops**
   - Make sure Docker has enough resources (at least 2GB RAM allocated)
   - Check that ports 9092 and 2181 are not already in use
   - Try restarting Docker daemon

2. **Connection refused errors**
   - Wait longer for Kafka to fully start (can take 30-60 seconds)
   - Check if containers are running with `docker-compose ps`
   - Verify port mappings with `netstat -an | grep 9092`

3. **Topic creation fails**
   - Ensure Kafka broker is fully started before creating topics
   - Check that ZooKeeper is running properly

### Debugging Steps

1. Check container status:
   ```bash
   docker-compose ps
   ```

2. Check container logs:
   ```bash
   docker-compose logs zookeeper
   docker-compose logs kafka
   ```

3. Test Kafka connectivity:
   ```bash
   # List topics
   docker exec kafka kafka-topics --list --bootstrap-server localhost:9092
   
   # Create topic manually
   docker exec kafka kafka-topics --create --topic test-topic --partitions 1 --replication-factor 1 --bootstrap-server localhost:9092
   ```

### Resource Requirements

- **RAM**: Minimum 2GB dedicated to Docker
- **CPU**: Minimum 1 core
- **Disk**: 50MB for Docker images, additional space for logs

## Production Considerations

1. **Separate ClickHouse Server**: Run ClickHouse on a dedicated server
2. **Kafka Replication**: For production, use a 3-broker cluster with replication
3. **Network Security**: Use SSL/TLS for Kafka connections
4. **Monitoring**: Set up monitoring for all components
5. **Backup**: Regular backups of persistent queue data