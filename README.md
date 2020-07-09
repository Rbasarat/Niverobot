# Niverobot
A telegram bot with random features

## Current commands:
 - .dadjoke/.vadergrap: Tells you a dadjoke
 
### Updating Grpc proto 
 - C# add the (new) protofile to the csproj.
 - Python: `python -m grpc_tools.protoc -I../Niverobot.Protos --python_out=. --grpc_python_out=. ../Niverobot.Protos/dateparser.proto`
### Docker mssql command for prod:
`docker run --name SQL19 -p 1433:1433 -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=password" -v /home/niverhawk/niverobot/Db/data:/var/opt/mssql/data -v /home/niverhawk/niverobot/Db/log:/var/opt/mssql/log -d mcr.microsoft.com/mssql/server:2019-latest`