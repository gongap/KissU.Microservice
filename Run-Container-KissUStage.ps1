docker run --name kissustage -p 91:91 -p 9180:9180 -p 9443:9443 -p 9101:9101 -p 9102:9102 -p 9103:9103 -p 9104:9104 -p 9105:9105 --env WanIp=192.168.2.76 --env Mapping_ip=127.17.0.1 --env Mapping_Port=91 --env Register_Conn=172.17.0.2:8500 --env EventBusConnection=172.17.0.3 kissustage:linux-latest
