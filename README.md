# Niverobot
A telegram bot with random features

## Current commands:
 - .dadjoke/.vadergrap: Tells you a dadjoke
 
### Updating Grpc proto 
 - C# add the (new) protofile to the csproj.
 - Python: `python -m grpc_tools.protoc -I../Niverobot.Protos --python_out=. --grpc_python_out=. ../Niverobot.Protos/dateparser.proto`
