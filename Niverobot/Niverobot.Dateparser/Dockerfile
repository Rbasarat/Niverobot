FROM python:3.8 as base

WORKDIR /usr/src/app

COPY Niverobot.Dateparser .
Copy Niverobot.Protos ./protos

run python -m pip install --upgrade pip
run python -m pip install grpcio
run python -m pip install grpcio-tools
run python -m grpc_tools.protoc -I./protos --python_out=. --grpc_python_out=. ./protos/dateparser.proto

CMD [ "python", "./dateparser.py" ]