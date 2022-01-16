# redis-sentinel-setup
Setup and test a redis sentinel based high availability solution

This repo will help you create a 3 node Redis High availability solution using docker containers and use redis sentinel as the failover solution.

The containers are built on the very convenient bitnami redis and redis sentinel images
https://github.com/bitnami/bitnami-docker-redis
https://github.com/bitnami/bitnami-docker-redis-sentinel

this method uses 3 different VM's on which redis running as a container, the sentinels are started as another container on these nodes.

Recommend to change the password and the Redis port numbers for better security

### Node - 1 starts as the master  (assume IP as 192.168.1.5)

`
docker run --name redis-node-1 -e REDIS_REPLICATION_MODE=master -e REDIS_MASTER_PASSWORD=Complex-Password-Goes-Here -e REDIS_PASSWORD=Complex-Password-Goes-Here -e REDIS_REPLICA_IP=192.168.1.5 -e REDIS_MASTER_PORT_NUMBER=6379 -e REDIS_PORT_NUMBER=6379 -v redis:/bitnami -p 6379:6379 -d  --restart always bitnami/redis:latest
`

### Node - 2 starts as the slave  (assume IP as 192.168.1.6)

`
docker run --name redis-node-2 -e REDIS_REPLICATION_MODE=slave -e REDIS_MASTER_HOST=192.168.1.5 -e REDIS_MASTER_PASSWORD=Complex-Password-Goes-Here -e REDIS_PASSWORD=Complex-Password-Goes-Here -e REDIS_MASTER_PORT_NUMBER=6379 -e REDIS_PORT_NUMBER=6379 -e REDIS_REPLICA_IP=192.168.1.6 -v redis:/bitnami -p 6379:6379 -d  --restart always bitnami/redis:latest
`

### Node - 3 starts as the slave  (assume IP as 192.168.1.7)

`
docker run --name redis-node-3 -e REDIS_REPLICATION_MODE=slave -e REDIS_MASTER_HOST=192.168.1.5 -e REDIS_MASTER_PASSWORD=Complex-Password-Goes-Here -e REDIS_PASSWORD=Complex-Password-Goes-Here -e REDIS_MASTER_PORT_NUMBER=6379 -e REDIS_PORT_NUMBER=6379 -e REDIS_REPLICA_IP=192.168.1.7 -v redis:/bitnami -p 6379:6379 -d  --restart always bitnami/redis:latest
`

once the containers are started on the 3 nodes, you can observe the docker logs of the containers to see if the servers are contacting each other and set themselves up as the replicas of the master server

next, start the redis sentinels as another container on the same nodes

### redis sentinel -- create one container on each of the nodes
`
docker run --name sentinel -e REDIS_SENTINEL_PASSWORD=Complex-Password-Goes-Here -e REDIS_MASTER_PASSWORD=Complex-Password-Goes-Here -e REDIS_MASTER_HOST=192.168.1.5 -e REDIS_MASTER_PORT_NUMBER=6379 -e REDIS_SENTINEL_DOWN_AFTER_MILLISECONDS=5000 -p 26379:26379 -d  --restart always bitnami/redis-sentinel:latest
`

we are pointing the sentinels to the master server, setup the failover duration as 5000 milliseconds, which means the sentinels will start the failover process if the master does not respond for 5 seconds.

the other C# file uses demonstrates testing out this Redis HA solution 
the C# code uses the StackExchange.Redis library, documented here https://stackexchange.github.io/StackExchange.Redis/
